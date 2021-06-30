using System;
#if !NET40
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;

#endif
namespace SqlAlias
{
    public static class Aliases
    {
#if NET40
        public static string Map(string connectionString) => connectionString;
        internal static bool ShouldSubstitute() => false;
#else
        public static string Map(string connectionString)
        {
            if (!ShouldSubstitute())
                return connectionString;

            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString);
                //select registry key for 64/32 bit systems
                var key = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") != "x86"
                    ? @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\MSSQLServer\Client\ConnectTo"
                    : @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\MSSQLServer\Client\ConnectTo";
                if(Microsoft.Win32.Registry.GetValue(key, builder.DataSource, null) is string newSource) 
                {
                    builder.DataSource = newSource.Substring(newSource.IndexOf(',') + 1);
                }
                return builder.ConnectionString;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to map the SQL Server alias", ex);
            }
        }

        internal static bool ShouldSubstitute()
            => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
               !RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework");

#endif
    }
}
