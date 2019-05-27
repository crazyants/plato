using System;
using System.Collections.Generic;

namespace Plato.Internal.Abstractions.Extensions
{

    public static class VersionExtensions
    {

        public static IEnumerable<Version> GetVersionsBetween(this Version from, Version to)
        {

            if (from == null)
            {
                throw new ArgumentNullException(nameof(from));
            }

            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }

            // Ensure the "from" is before and not equal to "to"
            if (from >= to)
            {
                return null;
            }

            // Build all versions between the two supplied versions

            var versions = new List<Version>();
            var version = from;
            while (version <= to)
            {
                var major = version.Major;
                var minor = version.Minor;
                var build = version.Build;
                if (build <= 9)
                    build = build + 1;
                if (build == 10)
                    build = 0;
                if (build == 0)
                    minor = (minor + 1);
                if (minor <= 9 && build == 10)
                    minor = minor + 1;
                if (minor == 10)
                    minor = 0;
                if (minor == 0 && build == 0)
                    major = (major + 1);

                if (version >= to)
                {
                    break;
                }

                version = new Version(major, minor, build);
                versions.Add(version);

            }

            return versions;

        }

    }
}
