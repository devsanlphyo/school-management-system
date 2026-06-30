# Synthesis of Explorer Reports: Milestone 1

## Consensus
All three explorers agree on the following implementation details:
1. **Route & Layout**: The page must be located at `MainSchoolsManagementSystem/Components/Pages/Teacher/Attendance.razor`, route to `@page "/teacher/attendance"`, and use `@layout MainSchoolsManagementSystem.Components.Layout.TeacherLayout`.
2. **Authorization**: Access must be restricted to users with the `Teacher` role using `@attribute [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Teacher")]`.
3. **Database Access**: `IDbContextFactory<ApplicationDbContext>` must be injected and used to create short-lived contexts, as per Section 10.1 of `design-system.md`.
4. **Current User**: Retreived via `AuthenticationStateProvider` and extracting the `NameIdentifier` claim.
5. **UI Styling**: The page should use `glass-panel` for cards/panels, `premium-table` for the history table, and `btn-premium` for the check-in button, following the design system.

## Resolved Conflicts / Key Decisions
- **Timezone and Lateness logic**: 
  - `Attendance.Date` should be stored as the local date (`DateTime.Today`).
  - `Attendance.CheckedInAt` should be stored in UTC (`DateTime.UtcNow`).
  - To calculate lateness, `CheckedInAt` must be converted to local time (`.ToLocalTime()`) and its `TimeOfDay` compared against the `DailyDeadline` from `SystemSettings` (fallback to `08:30:00`).
  - This ensures timezone consistency on the server.

## Next Steps
- Spawn a `teamwork_preview_worker` to implement the `/teacher/attendance` page and the check-in logic.
- The worker will write the implementation to `MainSchoolsManagementSystem/Components/Pages/Teacher/Attendance.razor`.
