# BRIEFING — 2026-06-30T06:11:45+06:30

## Mission
Implement Milestone 1 and Milestone 2 for the Teacher Attendance feature, including the Razor page and the check-in business logic with proper database persistence, lateness calculation, and design system styling.

## 🔒 My Identity
- Archetype: worker
- Roles: implementer, qa, specialist
- Working directory: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_m1_m2
- Original parent: 573287df-1a49-4333-b586-5eb1df205254
- Milestone: M1 & M2 Teacher Attendance

## 🔒 Key Constraints
- Save attendance records to the database.
- Calculate lateness against `SystemSetting.DailyDeadline` (fallback to 8:30 AM).
- Use the currently logged-in teacher's ID.
- Store `Date` as local date (midnight) and `CheckedInAt` in UTC. Convert to local time for display/comparison.
- Follow design system styling.
- Verify work by building the project and running existing unit tests.
- DO NOT CHEAT. All implementations must be genuine. No hardcoded outputs.

## Current Parent
- Conversation ID: 573287df-1a49-4333-b586-5eb1df205254
- Updated: not yet

## Task Summary
- **What to build**: Teacher Attendance page and check-in business logic.
- **Success criteria**: Functional check-in, database persistence, correct lateness calculation, correct timezone conversion, and beautiful design-compliant UI.
- **Interface contracts**: SystemSetting, Attendance models, DbContext.
- **Code layout**: `MainSchoolsManagementSystem/Components/Pages/Teacher/Attendance.razor`

## Key Decisions Made
- [TBD]

## Artifact Index
- `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\worker_m1_m2\handoff.md` — Final handoff report.
