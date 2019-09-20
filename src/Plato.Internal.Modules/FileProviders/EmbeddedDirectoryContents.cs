using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;

namespace Plato.Internal.Modules.FileProviders
{

    public class EmbeddedDirectoryContents : IDirectoryContents
    {
        private readonly IList<IFileInfo> _entries;

        public EmbeddedDirectoryContents(IEnumerable<IFileInfo> entries)
        {
            if (entries == null)
            {
                throw new ArgumentNullException(nameof(entries));
            }

            _entries = entries.ToList();
        }

        public bool Exists
        {
            get { return _entries.Any(); }
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entries.GetEnumerator();
        }
    }
}
