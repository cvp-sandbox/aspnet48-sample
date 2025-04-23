# Migration Plan for ASP.NET MVC 4.8 to .NET 9.0

Based on the successful migration of the landing page, this document provides a comprehensive migration plan for the remaining MVC Controller Actions and their associated views, partials, CSS, images, and JavaScript. This plan adopts a granular action-by-action migration approach to allow for side-by-side comparison between the legacy and new implementations.

## 1. Pre-Migration Analysis

1. **Controller Inventory**
   - Identify all remaining controllers to migrate:
     - AccountController
     - EventController
     - AdminController (SKIP - not used)
     - ManageController (SKIP - not used)

2. **Dependency Analysis**
   - Map controller dependencies (repositories, services)
   - Identify shared components and utilities
   - Document authentication/authorization requirements for each controller

3. **Asset Inventory**
   - Create an inventory of all CSS, JavaScript, and image files
   - Identify which assets are controller-specific vs. shared

## 2. Infrastructure Setup

1. **Repository Migration**
   - Complete migration of remaining repository interfaces and implementations
   - Ensure all SQL queries are compatible with the SQLite version used in .NET 9.0

2. **Authentication Infrastructure**
   - Migrate custom ASP.NET Identity implementation to ASP.NET Core Identity
   - Implement LegacyPasswordHasher for backward compatibility
   - Set up role-based authorization equivalent to the custom AuthorizeRolesAttribute

3. **YARP Configuration Updates**
   - Configure YARP to support action-level routing
   - Prepare route configurations for each individual action being migrated
   - Set up priority ordering to ensure proper request routing
   - Example configuration structure:
   ```json
   {
     "ReverseProxy": {
       "Routes": {
         "homeIndexRoute": {
           "ClusterId": "localCluster",
           "Match": {
             "Path": "/Home/Index",
             "Methods": [ "GET" ]
           },
           "Order": 1
         },
         "eventDetailsRoute": {
           "ClusterId": "localCluster",
           "Match": {
             "Path": "/Event/Details/{id}",
             "Methods": [ "GET" ]
           },
           "Order": 2
         },
         // Other migrated action routes...
         "legacyRoute": {
           "ClusterId": "legacyCluster",
           "Match": {
             "Path": "/{**catch-all}",
             "Methods": [ "GET", "POST", "PUT", "DELETE" ]
           },
           "Order": 999
         }
       }
     }
   }
   ```

## 3. Action-by-Action Migration Process

Instead of migrating entire controllers at once, this plan adopts a granular approach of migrating one controller action at a time. This allows for:

1. Running both applications side-by-side with YARP
2. Comparing the newly migrated action and views with the legacy implementation
3. Catching and fixing issues early before moving to the next action
4. Maintaining a working system throughout the migration process

### 3.1. Action Migration Process

For each controller action, follow this process:

1. **Analysis**
   - Examine the legacy action implementation
   - Identify dependencies (repositories, services, models)
   - Document view requirements (main view, partial views, JavaScript, CSS)
   - Note authorization requirements

2. **Implementation**
   - Implement required models and repositories (if not already done)
   - Implement the controller action
   - Create/migrate the associated view(s)
   - Migrate required assets (CSS, JavaScript, images)

3. **YARP Configuration**
   - Add a specific route for the migrated action
   - Set appropriate priority to ensure it's used instead of the legacy route

4. **Testing**
   - Test the migrated action in isolation
   - Compare with the legacy implementation
   - Verify all functionality works as expected
   - Test navigation between migrated and non-migrated parts

5. **Documentation**
   - Document the migration of the action
   - Note any changes or improvements made
   - Update the migration checklist

### 3.2. Suggested Action Migration Order

Rather than migrating entire controllers at once, here's a suggested order for migrating individual actions:

1. **Read-Only Actions First**
   - EventController.Index (list events)
   - EventController.Details (view event details)

2. **Simple Form Actions**
   - EventController.Register (register for event)
   - EventController.CancelRegistration (cancel registration)

3. **Complex Form Actions**
   - EventController.Create (create event)
   - EventController.Edit (edit event)
   - AccountController.Register (user registration)

### 3.3. Example: Migrating EventController.Details Action

Let's walk through an example of migrating a single action:

1. **Analysis**
   - EventController.Details displays event details and registration status
   - Dependencies: IEventRepository, IRegistrationRepository
   - Views: Details.cshtml
   - Authorization: [Authorize] - requires authentication

2. **Implementation Steps**
   - Ensure Event and Registration models are migrated
   - Implement required repository methods
   - Create EventController with Details action
   - Migrate Details.cshtml view
   - Migrate any CSS/JS specific to the details page

3. **YARP Configuration**
   ```json
   "eventDetailsRoute": {
     "ClusterId": "localCluster",
     "Match": {
       "Path": "/Event/Details/{id}",
       "Methods": [ "GET" ]
     },
     "Order": 10
   }
   ```

4. **Testing**
   - Access /Event/Details/1 directly
   - Navigate to it from other pages
   - Verify all details display correctly
   - Compare with legacy implementation

## 4. Shared Components Migration

1. **Layout and Navigation**
   - Migrate _Layout.cshtml if not already done
   - Migrate _LoginPartial.cshtml for authentication status display
   - Ensure consistent navigation across all pages

2. **Shared Partials**
   - Migrate _ValidationScriptsPartial.cshtml
   - Migrate any other shared partial views

3. **Tag Helpers**
   - Create tag helpers to replace HTML helpers where appropriate
   - Implement LegacyLink tag helper for backward compatibility

4. **View Components**
   - Create view components to replace Html.Action calls
   - Implement shared view components (e.g., navigation menus)

## 5. Asset Migration

1. **CSS**
   - Migrate all CSS files to wwwroot/css
   - Update bundling configuration to use ASP.NET Core bundling

2. **JavaScript**
   - Migrate all JavaScript files to wwwroot/js
   - Update bundling configuration to use ASP.NET Core bundling

3. **Images**
   - Migrate all images to wwwroot/images
   - Update image references in views

4. **Fonts**
   - Migrate any custom fonts to wwwroot/fonts

## 6. Testing Strategy

1. **Unit Testing**
   - Create unit tests for controllers and repositories
   - Test authorization logic

2. **Integration Testing**
   - Test end-to-end flows for each controller
   - Test form submissions and validation

3. **Authentication Testing**
   - Test login with existing credentials
   - Test registration of new users
   - Test role-based access control

4. **Cross-Browser Testing**
   - Verify UI consistency across browsers

## 7. Deployment Considerations

1. **Database Compatibility**
   - Ensure SQLite queries are compatible between versions
   - Test with production-like data volume

2. **Configuration Management**
   - Migrate Web.config settings to appsettings.json
   - Migrate .env file handling to .NET 9.0 approach

3. **Logging and Monitoring**
   - Set up logging equivalent to the legacy application
   - Implement health checks

## 8. Phased Rollout Strategy

1. **Action Prioritization**
   - Prioritize actions based on usage, complexity, and dependencies
   - Start with simpler, read-only actions to build experience
   - Group related actions when they share significant dependencies

2. **Testing Phases**
   - Internal testing after each action migration
   - User acceptance testing for groups of related functionality
   - Continuous comparison with legacy implementation

3. **Rollback Plan**
   - Maintain ability to route back to legacy actions if issues arise
   - Document rollback procedures for each action migration
   - YARP configuration allows for quick rollback by changing route priorities

## 9. Post-Migration Tasks

1. **Performance Optimization**
   - Review and optimize database queries
   - Implement caching where appropriate

2. **Code Cleanup**
   - Remove legacy compatibility code when no longer needed
   - Refactor to use more modern .NET 9.0 patterns

3. **Documentation**
   - Update developer documentation
   - Document any API changes

4. **Feature Enhancements**
   - Identify opportunities for improvements using new .NET 9.0 features
   - Plan for future enhancements

## 10. Migration Checklist Template

For each action, use this checklist to track migration progress:

```markdown
# [ControllerName].[ActionName] Migration Checklist

## Prerequisites
- [ ] Required models migrated
- [ ] Required repositories migrated
- [ ] Authentication/authorization infrastructure in place (if needed)

## Implementation
- [ ] Action method implemented
- [ ] Authorization attributes applied
- [ ] Main view migrated
- [ ] Partial views migrated
- [ ] HTML Helpers replaced with Tag Helpers
- [ ] Client-side validation implemented (if needed)

## Assets
- [ ] Required CSS migrated
- [ ] Required JavaScript migrated
- [ ] Image references updated

## YARP Configuration
- [ ] Route added for this specific action
- [ ] Route priority set appropriately
- [ ] Route tested with direct access

## Testing
- [ ] Action functions correctly
- [ ] Matches legacy behavior
- [ ] Authorization rules enforced correctly
- [ ] Form submission works (if applicable)
- [ ] Navigation to/from this action works
- [ ] Edge cases tested
```

## 11. Benefits of Action-by-Action Migration

1. **Incremental Progress**
   - Each action migration is a small, manageable task
   - You can see immediate results after each migration
   - Easier to track progress and estimate remaining work

2. **Risk Mitigation**
   - Issues are isolated to individual actions
   - Easier to rollback or fix problems
   - Rest of the application remains functional

3. **Parallel Development**
   - Multiple developers can work on different actions
   - Independent actions can be migrated simultaneously

4. **Learning Opportunity**
   - Start with simpler actions to build experience
   - Apply lessons learned to more complex actions

5. **Continuous Validation**
   - Side-by-side comparison ensures functional equivalence
   - Catch and fix issues early in the process
