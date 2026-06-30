# Handoff Report — LessonPlans Feature Investigation

This report summarizes the findings of the read-only investigation of the SchoolsManagementSystem codebase regarding the **LessonPlans** feature, specifically focusing on the Blazor pages UI layout, styling, and design-system guidelines.

---

## 1. Observation

We directly observed and examined the following files:

1.  `design-system.md` (root directory):
    *   Defines brand, typography (Outfit for headings, Plus Jakarta Sans for body/inputs), layout classes (`.admin-layout`, `.admin-sidebar`, `.admin-main`, `.admin-header`, `.admin-content`), and styling tokens.
    *   Defines premium components: `.glass-panel` (lines 180-198), `.premium-table` (lines 201-224), `.badge-present`/`.badge-late`/`.badge-pending` (lines 257-280), `.btn-premium`/`.btn-secondary-custom` (lines 293-318), and `.modal-backdrop-custom`/`.modal-content-custom` (lines 352-385).
2.  `MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor`:
    *   Implements the Headmaster-facing review and feedback flow.
    *   Uses a tabbed navigation structure (lines 22-29):
        ```html
        <div class="tab-navigation">
            <button class="tab-link @(activeTab == "pending" ? "active" : "")" @onclick='() => SetActiveTab("pending")'>Pending Review</button>
            <button class="tab-link @(activeTab == "reviewed" ? "active" : "")" @onclick='() => SetActiveTab("reviewed")'>Reviewed</button>
        </div>
        ```
    *   Uses a custom modal backdrop and content wrapper to show details and submit feedback (lines 102-109):
        ```html
        <div class="modal-backdrop-custom">
            <div class="modal-content-custom" style="max-width: 550px;">
                ...
        ```
    *   Submits feedback via `SubmitReview()` (lines 235-248), updating the status to `LessonPlanStatus.Reviewed`.
3.  `MainSchoolsManagementSystem/Data/LessonPlan.cs`:
    *   Defines the `LessonPlan` entity with properties: `Id`, `TeacherId`, `Teacher`, `UploadedAt`, `IsLate`, `JustificationText`, `HasJustificationAttachment`, `Status` (Pending = 0, Reviewed = 1), and `Feedback`.
4.  `MainSchoolsManagementSystem/Data/SystemSetting.cs`:
    *   Defines `SystemSetting` with `DailyDeadline` (TimeSpan, defaults to 8:30 AM) and `MaintenanceMode` (bool).
5.  `MainSchoolsManagementSystem/Components/Pages/Teacher/LessonPlans.razor`:
    *   A simple placeholder page stating "This LessonPlans page is currently under development."

---

## 2. Logic Chain

1.  **UI Layout & Styling consistency:** The design system (`design-system.md`) dictates that all portals (Admin, Headmaster, and Teacher) must share the same visual language, using specific CSS classes for layouts, panels, tables, badges, and modals. Therefore, the new teacher-facing Lesson Plans page should utilize these exact classes (e.g., `.glass-panel`, `.premium-table`, `.btn-premium`, and `.badge-*`).
2.  **Review and Feedback Integration:** The Headmaster's portal (`Headmaster/LessonPlans.razor`) reads from `DbContext.LessonPlans` and updates the `Status` to `LessonPlanStatus.Reviewed` and the `Feedback` text. This implies that the Teacher's portal must display this status and feedback to the teacher, requiring a read-only feedback modal matching the Headmaster's feedback display layout.
3.  **Lateness Detection:** The `LessonPlan` model has an `IsLate` flag and a `JustificationText` field, and the `SystemSetting` model has a `DailyDeadline`. When a teacher submits a lesson plan, the application must compare the submission time with the daily deadline. If it is past the deadline, the submission modal must dynamically display a lateness warning and require the teacher to enter `JustificationText`.

---

## 3. Caveats

*   **Attachment Handling:** The `LessonPlan` entity has a `HasJustificationAttachment` boolean field, but there is no file upload/download implementation currently present in the codebase. The implementation details of uploading/downloading files (e.g., PDF uploads for late justifications) must be designed during the implementation phase.
*   **Time Zones:** The daily deadline is stored as a `TimeSpan` (e.g. 08:30:00). Time comparison should be performed local to the school's timezone.

---

## 4. Conclusion

The Teacher Lesson Plans page should be designed with:
1.  A main dashboard showing submission statistics (Total, On Time, Late).
2.  A premium table showing past submissions, their status (Pending / Reviewed), and compliance (On Time / Late).
3.  A "Submit Lesson Plan" modal that checks `DateTime.Now.TimeOfDay` against `SystemSetting.DailyDeadline` to conditionally display and require a `JustificationText` input field.
4.  A read-only "View Feedback" modal for reviewed plans.

All elements must use the classes and tokens specified in `design-system.md`.

---

## 5. Verification Method

To verify the findings and design details:
1.  Inspect the detailed analysis report at `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_lessonplans_3\analysis.md`.
2.  Run the application build command to verify project compilation:
    ```powershell
    dotnet build MainSchoolsManagementSystem.sln
    ```
3.  Inspect `MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor` to verify the modal structure and review flow.
