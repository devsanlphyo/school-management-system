# Context: LessonPlans Feature

## Overview
The goal is to implement a lesson plan submission and review workflow in the `SchoolsManagementSystem`.
Teachers submit lesson plans. If a submission is after the daily deadline, it is marked late and requires a justification text (and optionally a justification file).
Headmasters review the submissions, see the files/justifications, and provide feedback.

## Current Codebase State
- **LessonPlan Model**: `MainSchoolsManagementSystem/Data/LessonPlan.cs`
- **SystemSetting Model**: `MainSchoolsManagementSystem/Data/SystemSetting.cs`
- **Teacher Page**: `MainSchoolsManagementSystem/Components/Pages/Teacher/LessonPlans.razor` (placeholder)
- **Headmaster Page**: `MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor` (has list, modal, and review submission logic, but lacks file view/download links and justification display)

## Key Questions for Explorer
1. Where are files uploaded? Are there existing file upload helpers?
2. How do we determine the file extension of a saved lesson plan/justification when displaying the download link, given that the database does not store the file extension?
3. How is the `DailyDeadline` retrieved from the database? Is there always a single `SystemSetting` record (e.g., ID = 1)?
4. What is the current layout/theme/CSS styling we should use to match the rest of the application?
