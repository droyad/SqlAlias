using System;
#if !NET40
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;
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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                RuntimeInformation.FrameworkDescription.StartsWith(".NET Core")) // netstandard library may be used from NetFX runtime
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
