# Business Logic Migration Tasks

We have .NET Framework 4.8 ASP.NET MVC application `EventRegistrationSystem` and we are moving the business logic to a new .NET 9.0 Web Minimal API project, `EventManagement.Api`, following the REPR (request-endpoint-response) pattern with unit tests and integration tests for the Request handlers. Once a legacy endpoint is moved to the WebAPI  that legacy endpoint can call the WebAPI via `ApiClient.cs` passing the authenticated username and roles as  Header values. The authorization check / policies in the legacy app should be similarly applied to the Request Handlers.


## EventController
NOTE: We have successfully created the `EventManagement.Api` project with a working endpoint for `src\EventManagement.Api\Features\Events\GetAllEvents\GetAllEventsHandler.cs`; with unit test and integration tests for the `GetAllEventsHandler.cs` handler;  in the legacy application, this endpoint is called using the class `ApiClient` in the `EventController` `Index` action

1. ✅ Identify all remaining actions in `EventController` and their corresponding business logic.
2. ✅ Create new request handlers in `EventManagement.Api` for each action in `EventController`:
   - ✅ GetEventById
   - ✅ CreateEvent
   - ✅ UpdateEvent
   - ✅ DeleteEvent
   - ✅ RegisterForEvent
   - ✅ CancelRegistration
   - ✅ GetEventsByCreator (MyEvents)
   - ✅ GetRegistrationsByUserId (MyRegistrations)
3. ✅ Implement unit tests for each new request handler using Xunit and NSubstitute:
   - ✅ GetEventByIdHandler tests
   - ✅ CreateEventHandler tests
   - ✅ RegisterForEventHandler tests
4. ✅ Implement integration tests for each new request handler:
   - ✅ GetEventById integration test
5. ✅ Update the legacy `EventController` to call the new WebAPI endpoints using `ApiClient`.

## HomeController
1. ✅ Identify all actions in `HomeController` (`Index`, `_FeaturedEvents`, `_Stats`, `_UpcomingEvents`) and their corresponding business logic.
2. ✅ Create new request handlers in `EventManagement.Api` for each action in `HomeController`:
   - ✅ GetFeaturedEvents
   - ✅ GetStats
   - ✅ GetUpcomingEvents
3. ✅ Implement unit tests for each new request handler using Xunit and NSubstitute:
   - ✅ GetFeaturedEventsHandler tests
   - ✅ GetStatsHandler tests
   - ✅ GetUpcomingEventsHandler tests
4. ✅ Implement integration tests for each new request handler.
5. ✅ Update the legacy `HomeController` to call the new WebAPI endpoints using `ApiClient`:
   - ✅ Index action (using GetFeaturedEvents endpoint)
   - ✅ _Stats action (using GetStats endpoint)
   - ✅ _FeaturedEvents action (using GetFeaturedEvents endpoint)
   - ✅ _UpcomingEvents action (using GetUpcomingEvents endpoint)

## AccountController
1. ✅ Identify all actions in `AccountController` (`Login`, `LogOff`, `Register`) and their corresponding business logic.
2. ✅ Create new request handlers in `EventManagement.Api` for each action in `AccountController`:
   - ✅ Create Users module structure in `src/EventManagement.Api/Features/Users/`
   - ✅ Create `Login` feature:
     - ✅ LoginRequest.cs
     - ✅ LoginResponse.cs
     - ✅ LoginHandler.cs
     - ✅ LoginEndpoint.cs
   - ✅ Create `Register` feature:
     - ✅ RegisterRequest.cs
     - ✅ RegisterResponse.cs
     - ✅ RegisterHandler.cs
     - ✅ RegisterEndpoint.cs
   - ✅ Create `LogOff` feature:
     - ✅ LogOffRequest.cs
     - ✅ LogOffResponse.cs
     - ✅ LogOffHandler.cs
     - ✅ LogOffEndpoint.cs
   - ✅ Create user repositories:
     - ✅ IUserRepository.cs
     - ✅ UserRepository.cs
     - ✅ IRoleRepository.cs
     - ✅ RoleRepository.cs
   - ✅ Update Program.cs to register and map Users module
3. ✅ Implement unit tests for each new request handler using Xunit and NSubstitute:
   - ✅ LoginHandlerTests.cs
   - ✅ RegisterHandlerTests.cs
   - ✅ LogOffHandlerTests.cs
4. ✅ Implement integration tests for each new request handler:
   - ✅ UsersApiTests.cs
5. ✅ Update the legacy `AccountController` to call the new WebAPI endpoints using `ApiClient`:
   - ✅ Update Login action
   - ✅ Update LogOff action
   - ✅ Update Register action

## General Tasks
1. Ensure consistent authorization policies between the legacy application and the new WebAPI.
2. Verify that the shared Sqlite database (`_db\EventRegistration.db`) is correctly accessed by both the legacy application and the WebAPI.
3. Perform end-to-end testing to confirm the functionality of migrated business logic.
4. Update documentation to reflect the migration of business logic.
5. Be consistent with implementation
