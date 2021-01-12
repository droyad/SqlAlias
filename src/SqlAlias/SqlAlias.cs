using System;
#if !NET40
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
#endif

namespace SqlAlias
{
    public static class Aliases
    {
        public static string Map(string connectionString)
        {
#if NET40
            return connectionString;
#else
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return connectionString;

            Match match = Regex.Match(RuntimeInformation.FrameworkDescription, @"^(?<product>.NET(?: Core)?) (?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+).*$");
            if (match.Success
                && (string.Equals(match.Groups["product"].Value, ".NET Core", StringComparison.OrdinalIgnoreCase) // netstandard library may be used from NetFX runtime
                    || string.Equals(match.Groups["product"].Value, ".NET", StringComparison.OrdinalIgnoreCase) && int.TryParse(match.Groups["major"].Value, out int major) && major >= 5)
            )
            {

                try
                {
                    var builder = new SqlConnectionStringBuilder(connectionString);

                    var newSource = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSSQLServer\Client\ConnectTo", builder.DataSource, null);
                    if (newSource != null)
                        builder.DataSource = newSource.Substring(newSource.IndexOf(',') + 1);

                    return builder.ConnectionString;
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to map the SQL Server alias", ex);
                }
            }

            return connectionString;
#endif
        }
    }
}
