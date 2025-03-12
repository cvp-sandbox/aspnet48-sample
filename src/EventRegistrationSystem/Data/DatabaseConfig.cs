using System;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace EventRegistrationSystem.Data
{
    public static class DatabaseConfig
    {
        private static string DbFile => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EventRegistration.db");

        public static IDbConnection GetConnection()
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabase();
            }

            return new SQLiteConnection($"Data Source={DbFile};Version=3;");
        }

        private static void CreateDatabase()
        {
            SQLiteConnection.CreateFile(DbFile);

            using (var connection = new SQLiteConnection($"Data Source={DbFile};Version=3;"))
            {
                connection.Open();

                // Create tables
                ExecuteNonQuery(connection, CreateEventsTableSql);
                ExecuteNonQuery(connection, CreateRegistrationsTableSql);
                ExecuteNonQuery(connection, CreateRolesTableSql);
                ExecuteNonQuery(connection, CreateUserRolesTableSql);
                ExecuteNonQuery(connection, CreateIdentityTablesSQL);
            }
        }

        private static void ExecuteNonQuery(SQLiteConnection connection, string sql)
        {
            using (var command = new SQLiteCommand(sql, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private static string CreateIdentityTablesSQL => @"
CREATE TABLE AspNetUsers (
    Id TEXT PRIMARY KEY,
    Email TEXT,
    EmailConfirmed INTEGER,
    PasswordHash TEXT,
    SecurityStamp TEXT,
    PhoneNumber TEXT,
    PhoneNumberConfirmed INTEGER,
    TwoFactorEnabled INTEGER,
    LockoutEndDateUtc TEXT,
    LockoutEnabled INTEGER,
    AccessFailedCount INTEGER,
    UserName TEXT NOT NULL UNIQUE
);

CREATE TABLE AspNetUserLogins (
    LoginProvider TEXT,
    ProviderKey TEXT,
    UserId TEXT,
    PRIMARY KEY (LoginProvider, ProviderKey, UserId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id)
);

CREATE TABLE AspNetUserClaims (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id)
);";

        private static string CreateEventsTableSql => @"
            CREATE TABLE Events (
                EventId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                EventDate TEXT NOT NULL,
                Location TEXT,
                MaxAttendees INTEGER,
                CreatedBy TEXT NOT NULL,
                CreatedDate TEXT NOT NULL
            );";

        private static string CreateRegistrationsTableSql => @"
            CREATE TABLE Registrations (
                RegistrationId INTEGER PRIMARY KEY AUTOINCREMENT,
                EventId INTEGER NOT NULL,
                UserId TEXT NOT NULL,
                RegistrationDate TEXT NOT NULL,
                FOREIGN KEY (EventId) REFERENCES Events (EventId)
            );";

        private static string CreateRolesTableSql => @"
            CREATE TABLE Roles (
                RoleId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE
            );
            
            INSERT INTO Roles (Name) VALUES ('Admin');
            INSERT INTO Roles (Name) VALUES ('Organizer');
            INSERT INTO Roles (Name) VALUES ('User');";

        private static string CreateUserRolesTableSql => @"
            CREATE TABLE UserRoles (
                UserRoleId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                RoleId INTEGER NOT NULL,
                FOREIGN KEY (RoleId) REFERENCES Roles (RoleId)
            );";
    }
}