# Business Logic Migration Tasks

We have .NET Framework 4.8 ASP.NET MVC application `EventRegistrationSystem` and we are moving the business logic to a new .NET 9.0 Web Minimal API project, `EventManagement.Api`, following the REPR (request-endpoint-response) pattern with unit tests and integration tests for the Request handlers. Once a legacy endpoint is moved to the WebAPI  that legacy endpoint can call the WebAPI via `ApiClient.cs` passing the authenticated username and roles as  Header values. The authorization check / policies in the legacy app should be similarly applied to the Request Handlers.


## EventController
1. Identify all actions in `EventController` and their corresponding business logic.
2. Create new request handlers in `EventManagement.Api` for each action in `EventController`.
3. Implement unit tests for each new request handler using Xunit and NSubstitute.
4. Implement integration tests for each new request handler.
5. Update the legacy `EventController` to call the new WebAPI endpoints using `ApiClient`.

## AccountController
1. Identify all actions in `AccountController` (`Login`, `LogOff`, `Register`) and their corresponding business logic.
2. Create new request handlers in `EventManagement.Api` for each action in `AccountController`.
3. Implement unit tests for each new request handler using Xunit and NSubstitute.
4. Implement integration tests for each new request handler.
5. Update the legacy `AccountController` to call the new WebAPI endpoints using `ApiClient`.

## HomeController
1. Identify all actions in `HomeController` (`Index`, `_FeaturedEvents`, `_Stats`, `_UpcomingEvents`) and their corresponding business logic.
2. Create new request handlers in `EventManagement.Api` for each action in `HomeController`.
3. Implement unit tests for each new request handler using Xunit and NSubstitute.
4. Implement integration tests for each new request handler.
5. Update the legacy `HomeController` to call the new WebAPI endpoints using `ApiClient`.

## General Tasks
1. Ensure consistent authorization policies between the legacy application and the new WebAPI.
2. Verify that the shared Sqlite database (`_db\EventRegistration.db`) is correctly accessed by both the legacy application and the WebAPI.
3. Perform end-to-end testing to confirm the functionality of migrated business logic.
4. Update documentation to reflect the migration of business logic.