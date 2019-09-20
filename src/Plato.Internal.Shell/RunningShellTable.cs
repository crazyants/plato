using System;
using System.Collections.Generic;
using System.Threading;
using Plato.Internal.Models.Shell;
using Plato.Internal.Shell.Abstractions;

namespace Plato.Internal.Shell
{
    public class RunningShellTable : IRunningShellTable
    {
        private readonly Dictionary<string, IShellSettings> _shellsByHostAndPrefix =
            new Dictionary<string, IShellSettings>(StringComparer.OrdinalIgnoreCase);

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private IShellSettings _single;
        private IShellSettings _default;

        public void Add(IShellSettings settings)
        {
            _lock.EnterWriteLock();
            try
            {
                // _single is set when there is only a single tenant
                if (_single != null)
                {
                    _single = null;
                }
                else
                {
                    _single = settings;
                }

                if (ShellHelper.DefaultShellName == settings.Name)
                {
                    _default = settings;
                }

                var hostAndPrefix = GetHostAndPrefix(settings);
                _shellsByHostAndPrefix[hostAndPrefix] = settings;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Remove(IShellSettings settings)
        {
            _lock.EnterWriteLock();
            try
            {
                var hostAndPrefix = GetHostAndPrefix(settings);
                _shellsByHostAndPrefix.Remove(hostAndPrefix);

                if (_default == settings)
                {
                    _default = null;
                }

                if (_single == settings)
                {
                    _single = null;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IShellSettings Match(string host, string appRelativePath)
        {
            _lock.EnterReadLock();
            try
            {
                if (_single != null)
                {
                    return _single;
                }

                var hostAndPrefix = GetHostAndPrefix(host, appRelativePath);
                if (!_shellsByHostAndPrefix.TryGetValue(hostAndPrefix, out var result))
                {
                    var noHostAndPrefix = GetHostAndPrefix("", appRelativePath);
                    if (!_shellsByHostAndPrefix.TryGetValue(noHostAndPrefix, out result))
                    {
                        result = _default;
                    }
                }

                return result;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public IDictionary<string, IShellSettings> ShellsByHostAndPrefix
        {
            get
            {
                return _shellsByHostAndPrefix;
            }
        }

        private string GetHostAndPrefix(string host, string appRelativePath)
        {
            // removing the port from the host
            var hostLength = host.IndexOf(':');
            if (hostLength != -1)            
                host = host.Substring(0, hostLength);
            
            // appRelativePath starts with /
            int firstSegmentIndex = appRelativePath.IndexOf('/', 1);
            if (firstSegmentIndex > -1)            
                return host + appRelativePath.Substring(0, firstSegmentIndex);            
            else            
                return host + appRelativePath;            

        }

        private string GetHostAndPrefix(IShellSettings shellSettings)
        {
            return shellSettings.RequestedUrlHost + "/" + shellSettings.RequestedUrlPrefix;
        }

    }

}
