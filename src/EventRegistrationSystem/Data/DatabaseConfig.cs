using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.IO;
using EventRegistrationSystem.Utils;

namespace EventRegistrationSystem.Data
{
    public static class DatabaseConfig
    {
        //should read form .env file for override; default in web.config
        private static string DbFile => ConfigurationManager.AppSettings["DatabasePath"];

        public static IDbConnection GetConnection()
        {
            var connectionString = 
                ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Data Source={DbFile};Version=3;";

            var cs = connectionString.Replace("{DbFile}", DbFile);
            
            return new SQLiteConnection(cs);
        }
    }
}
