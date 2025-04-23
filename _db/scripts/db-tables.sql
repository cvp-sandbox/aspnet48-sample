CREATE TABLE Events (
                EventId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                EventDate TEXT NOT NULL,
                Location TEXT,
                MaxAttendees INTEGER,
                CreatedBy TEXT NOT NULL,
                CreatedDate TEXT NOT NULL
            )

CREATE TABLE Registrations (
                RegistrationId INTEGER PRIMARY KEY AUTOINCREMENT,
                EventId INTEGER NOT NULL,
                UserId TEXT NOT NULL,
                RegistrationDate TEXT NOT NULL,
                FOREIGN KEY (EventId) REFERENCES Events (EventId)
            )
CREATE TABLE Roles (
                RoleId INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL UNIQUE
            )
CREATE TABLE UserRoles (
                UserRoleId INTEGER PRIMARY KEY AUTOINCREMENT,
                UserId TEXT NOT NULL,
                RoleId INTEGER NOT NULL,
                FOREIGN KEY (RoleId) REFERENCES Roles (RoleId)
            )
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
)
CREATE TABLE AspNetUserLogins (
    LoginProvider TEXT,
    ProviderKey TEXT,
    UserId TEXT,
    PRIMARY KEY (LoginProvider, ProviderKey, UserId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id)
)
CREATE TABLE AspNetUserClaims (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers (Id)
)