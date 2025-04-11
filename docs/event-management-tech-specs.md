# Event Management System - Technical Specifications

## Overview
This document outlines the technical specifications for the Event Management System using ASP.NET MVC 5 with .NET Framework 4.8, Bootstrap, jQuery, and SQLite database.

## Technology Stack
- **Backend Framework**: ASP.NET MVC 5 (.NET Framework 4.8)
- **Database**: SQLite
- **Frontend**: Bootstrap 5.3.3, jQuery 3.7.1
- **Authentication**: ASP.NET Identity 2.2.4 (customized for SQLite)
- **Data Access**: Dapper 2.1.66 for direct SQL access
- **Dependency Injection**: Unity 5.11.1
- **Export Formats**: iCal/ICS for calendar events, CSV for data export

## System Architecture

### Authentication & Authorization

#### Identity Implementation
The system uses ASP.NET Identity 2.2.4 with a custom SQLite implementation:

1. **Custom ApplicationUser Class**:
   ```csharp
   public class ApplicationUser : IUser
   {
       public string Id { get; set; }
       public string UserName { get; set; }
       public string Email { get; set; }
       public bool EmailConfirmed { get; set; }
       public string PasswordHash { get; set; }
       public string SecurityStamp { get; set; }
       public string PhoneNumber { get; set; }
       public bool PhoneNumberConfirmed { get; set; }
       public bool TwoFactorEnabled { get; set; }
       public DateTime? LockoutEndDateUtc { get; set; }
       public bool LockoutEnabled { get; set; }
       public int AccessFailedCount { get; set; }
       
       // Constructor generates a new GUID for Id
       public ApplicationUser()
       {
           Id = Guid.NewGuid().ToString();
       }
       
       // Method to generate claims identity for authentication
       public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
       {
           var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
           return userIdentity;
       }
   }
   ```

2. **Custom SQLiteUserStore**:
   The application implements a custom `SqliteUserStore` that implements multiple ASP.NET Identity interfaces:
   - `IUserStore<ApplicationUser>`
   - `IUserPasswordStore<ApplicationUser>`
   - `IUserEmailStore<ApplicationUser>`
   - `IUserLockoutStore<ApplicationUser, string>`
   - `IUserTwoFactorStore<ApplicationUser, string>`

   This store uses Dapper to execute direct SQL queries against the SQLite database.

3. **User Manager Configuration**:
   ```csharp
   public static ApplicationUserManager Create(
       IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
   {
       var manager = new ApplicationUserManager(new SqliteUserStore());
       
       // Configure validation logic for usernames
       manager.UserValidator = new UserValidator<ApplicationUser>(manager)
       {
           AllowOnlyAlphanumericUserNames = false,
           RequireUniqueEmail = true
       };
       
       // Configure validation logic for passwords
       manager.PasswordValidator = new PasswordValidator
       {
           RequiredLength = 6,
           RequireNonLetterOrDigit = true,
           RequireDigit = true,
           RequireLowercase = true,
           RequireUppercase = true,
       };
       
       // Configure user lockout defaults
       manager.UserLockoutEnabledByDefault = true;
       manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
       manager.MaxFailedAccessAttemptsBeforeLockout = 5;
       
       // Configure token provider for password reset and two-factor auth
       var dataProtectionProvider = options.DataProtectionProvider;
       if (dataProtectionProvider != null)
       {
           manager.UserTokenProvider = 
               new DataProtectorTokenProvider<ApplicationUser>(
                   dataProtectionProvider.Create("ASP.NET Identity"));
       }
       
       return manager;
   }
   ```

#### Role-Based Authorization

1. **Role Constants**:
   ```csharp
   public static class Roles
   {
       public const string Admin = "Admin";
       public const string Organizer = "Organizer";
       public const string User = "User";
   }
   ```

2. **Custom AuthorizeRolesAttribute**:
   ```csharp
   public class AuthorizeRolesAttribute : AuthorizeAttribute
   {
       private readonly string[] _roles;

       public AuthorizeRolesAttribute(params string[] roles)
       {
           _roles = roles;
       }

       protected override bool AuthorizeCore(HttpContextBase httpContext)
       {
           bool isAuthorized = base.AuthorizeCore(httpContext);

           if (!isAuthorized)
               return false;

           // If the user is authenticated, check if they are in any of the required roles
           string userId = httpContext.User.Identity.GetUserId();
           if (string.IsNullOrEmpty(userId))
               return false;

           // Check if user is in any of the required roles
           var roleRepository = DependencyResolver.Current.GetService<IRoleRepository>();
           foreach (var role in _roles)
           {
               if (roleRepository.UserIsInRole(userId, role))
                   return true;
           }

           return false;
       }
   }
   ```

3. **Role Repository**:
   ```csharp
   public class RoleRepository : IRoleRepository
   {
       public bool UserIsInRole(string userId, string roleName)
       {
           using (var connection = DatabaseConfig.GetConnection())
           {
               connection.Open();
               int count = connection.ExecuteScalar<int>(@"
                   SELECT COUNT(*) 
                   FROM UserRoles ur
                   JOIN Roles r ON ur.RoleId = r.RoleId
                   WHERE ur.UserId = @UserId AND r.Name = @RoleName",
                   new { UserId = userId, RoleName = roleName });

               return count > 0;
           }
       }
       
       // Other role management methods...
   }
   ```

4. **Authorization Usage**:
   ```csharp
   // Controller-level authorization
   [Authorize]
   [AuthorizeRoles(Roles.Admin)]
   public class AdminController : Controller
   {
       // Only accessible to users with Admin role
   }
   
   // Action-level authorization
   [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
   public ActionResult Create()
   {
       // Only accessible to users with Admin or Organizer roles
   }
   
   // Resource-level authorization in code
   string userId = User.Identity.GetUserId();
   if (eventEntity.CreatedBy != userId && !User.IsInRole(Roles.Admin))
   {
       return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
   }
   ```

### Database Architecture

#### Database Schema
The application uses the following database schema:

```sql
-- ASP.NET Identity Tables
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
);

-- Custom Role Management Tables
CREATE TABLE Roles (
    RoleId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE
);

CREATE TABLE UserRoles (
    UserRoleId INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    RoleId INTEGER NOT NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles (RoleId)
);

-- Core Event Management Tables
CREATE TABLE Events (
    EventId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,
    Description TEXT,
    EventDate TEXT NOT NULL,
    Location TEXT,
    MaxAttendees INTEGER,
    CreatedBy TEXT NOT NULL,
    CreatedDate TEXT NOT NULL
);

CREATE TABLE Registrations (
    RegistrationId INTEGER PRIMARY KEY AUTOINCREMENT,
    EventId INTEGER NOT NULL,
    UserId TEXT NOT NULL,
    RegistrationDate TEXT NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Events (EventId)
);
```

### Data Access Layer

The application uses Dapper for data access instead of Entity Framework:

```csharp
public static class DatabaseConfig
{
    private static string DbFile => ConfigurationManager.AppSettings["DatabasePath"];

    public static IDbConnection GetConnection()
    {
        var connectionString = 
            ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString ?? "Data Source={DbFile};Version=3;";

        var cs = connectionString.Replace("{DbFile}", DbFile);
        
        return new SQLiteConnection(cs);
    }
}
```

#### Repository Pattern Implementation

```csharp
// Repository interface
public interface IEventRepository
{
    IEnumerable<Event> GetAllEvents();
    Event GetEventById(int eventId);
    IEnumerable<Event> GetEventsByCreator(string userId);
    int CreateEvent(Event eventEntity);
    bool UpdateEvent(Event eventEntity);
    bool DeleteEvent(int eventId);
    int GetRegistrationCount(int eventId);
}

// Repository implementation
public class EventRepository : IEventRepository
{
    public IEnumerable<Event> GetAllEvents()
    {
        using (var connection = DatabaseConfig.GetConnection())
        {
            connection.Open();
            return connection.Query<Event>("SELECT * FROM Events ORDER BY EventDate");
        }
    }
    
    public Event GetEventById(int eventId)
    {
        using (var connection = DatabaseConfig.GetConnection())
        {
            connection.Open();
            return connection.QueryFirstOrDefault<Event>(
                "SELECT * FROM Events WHERE EventId = @EventId", 
                new { EventId = eventId });
        }
    }
    
    // Other repository methods...
}
```

### Dependency Injection

The application uses Unity for dependency injection:

```csharp
public static class DependencyConfig
{
    public static void RegisterDependencies()
    {
        var container = new UnityContainer();

        // Register the SqliteUserStore with its dependencies
        container.RegisterType<SqliteUserStore>(new ContainerControlledLifetimeManager());

        // Register repositories
        container.RegisterType<IEventRepository, EventRepository>();
        container.RegisterType<IRegistrationRepository, RegistrationRepository>();
        container.RegisterType<IRoleRepository, RoleRepository>();

        // Register identity stores if using custom SQLite implementation
        container.RegisterType<IUserStore<ApplicationUser>, SqliteUserStore>();
        container.RegisterType<IUserPasswordStore<ApplicationUser>, SqliteUserStore>();
        container.RegisterType<IUserEmailStore<ApplicationUser>, SqliteUserStore>();
        container.RegisterType<SqliteUserStore>();

        // Register your UserManager and SignInManager
        container.RegisterType<ApplicationUserManager>();
        container.RegisterType<ApplicationSignInManager>(new InjectionConstructor(
            new ResolvedParameter<ApplicationUserManager>(),
            new ResolvedParameter<IAuthenticationManager>()
        ));

        // Register IAuthenticationManager with a factory method to get it from OWIN context
        container.RegisterFactory<IAuthenticationManager>(c => HttpContext.Current.GetOwinContext().Authentication, 
            new HierarchicalLifetimeManager());
        
        // Register MVC controllers
        container.RegisterType<AccountController>();
        container.RegisterType<ManageController>();
        container.RegisterType<AdminController>();
        container.RegisterType<EventController>();
        container.RegisterType<HomeController>();

        DependencyResolver.SetResolver(new UnityDependencyResolver(container));
    }
}
```

### Application Startup

The application uses OWIN for startup configuration:

```csharp
// Startup.cs
[assembly: OwinStartupAttribute(typeof(EventRegistrationSystem.Startup))]
namespace EventRegistrationSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

// Startup.Auth.cs
public void ConfigureAuth(IAppBuilder app)
{
    app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
    app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

    app.UseCookieAuthentication(new CookieAuthenticationOptions
    {
        AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
        LoginPath = new PathString("/Account/Login"),
        Provider = new CookieAuthenticationProvider
        {
            OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                validateInterval: TimeSpan.FromMinutes(30),
                regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
        }
    });

    app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
    app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
    app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
}
```

### Environment Configuration

The application uses a combination of Web.config and .env files for configuration:

```csharp
// DotEnvReader.cs
public static class DotEnvReader
{
    public static Dictionary<string, string> Load(string filePath)
    {
        var envVars = new Dictionary<string, string>();
        
        if (!File.Exists(filePath))
            return envVars;
            
        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split('=', 2);
            if (parts.Length != 2)
                continue;
                
            var key = parts[0].Trim();
            var value = parts[1].Trim();
            
            if (string.IsNullOrEmpty(key))
                continue;
                
            envVars[key] = value;
        }
        
        return envVars;
    }
}

// Global.asax.cs
private void LoadEnvSettings()
{
    var envFilePath = Server.MapPath("~/.env");
    var envVariables = DotEnvReader.Load(envFilePath);

    foreach (var envVariable in envVariables)
    {
        var key = envVariable.Key;
        var value = envVariable.Value;

        if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
        {
            ConfigurationManager.AppSettings[key] = value;
        }
        else
        {
            ConfigurationManager.AppSettings.Add(key, value);
        }
    }
}
```

## Controller Structure

The application follows the standard MVC controller pattern:

### Main Controllers

1. **HomeController**: Handles the home page and general navigation
2. **AccountController**: Handles user registration, login, and account management
3. **EventController**: Handles event creation, editing, and registration
4. **AdminController**: Handles administrative functions like user and role management

### Controller Authorization

Controllers use a combination of the `[Authorize]` attribute and the custom `[AuthorizeRoles]` attribute to control access:

```csharp
[Authorize]  // Requires authentication
public class EventController : Controller
{
    [HttpGet]
    public ActionResult Index()
    {
        // All authenticated users can view events
    }
    
    [HttpGet]
    [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
    public ActionResult Create()
    {
        // Only Admin and Organizer roles can create events
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AuthorizeRoles(Roles.Admin, Roles.Organizer)]
    public ActionResult Create(Event eventEntity)
    {
        // Process event creation
    }
}
```

## View Structure

The application uses Razor views with the standard MVC view structure:

```
Views/
├── Account/           # User account views
├── Admin/             # Admin panel views
├── Event/             # Event management views
├── Home/              # Home page and general views
├── Manage/            # User profile management views
├── Shared/            # Shared layouts and partials
│   ├── _Layout.cshtml # Main layout template
│   └── _LoginPartial.cshtml # Login/logout partial
└── Web.config         # View-specific configuration
```

## Security Considerations

1. **CSRF Protection**: All forms use `@Html.AntiForgeryToken()` and `[ValidateAntiForgeryToken]` attributes
2. **Password Security**: Passwords are hashed and stored securely using ASP.NET Identity
3. **Authorization**: Multi-level authorization ensures users can only access appropriate resources
4. **Input Validation**: Model validation is used to validate user input
