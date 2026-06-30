# Handoff Report — LessonPlans Requirements Exploration

This report summarizes the findings and requirements for the **LessonPlans** feature to support the design and implementation of a requirement-driven, opaque-box E2E test suite.

---

## 1. Observation
- **LessonPlan Model**: Defined in `MainSchoolsManagementSystem/Data/LessonPlan.cs` (lines 11–22):
  ```csharp
  public class LessonPlan
  {
      public int Id { get; set; }
      public string TeacherId { get; set; } = string.Empty;
      public ApplicationUser? Teacher { get; set; }
      public DateTime UploadedAt { get; set; }
      public bool IsLate { get; set; }
      public string? JustificationText { get; set; }
      public bool HasJustificationAttachment { get; set; }
      public LessonPlanStatus Status { get; set; } = LessonPlanStatus.Pending;
      public string? Feedback { get; set; }
  }
  ```
- **SystemSetting Model**: Defined in `MainSchoolsManagementSystem/Data/SystemSetting.cs` (lines 5–10):
  ```csharp
  public class SystemSetting
  {
      public int Id { get; set; }
      public TimeSpan DailyDeadline { get; set; } = new TimeSpan(8, 30, 0);
      public bool MaintenanceMode { get; set; } = false;
  }
  ```
- **Teacher View**: Defined in `MainSchoolsManagementSystem/Components/Pages/Teacher/LessonPlans.razor` (lines 11–13):
  ```razor
  <div class='card card-premium'>
      <div class='card-header'>
          <h3 class='card-title'>LessonPlans</h3>
      </div>
      <div class='card-body'>
          <p>This LessonPlans page is currently under development.</p>
      </div>
  </div>
  ```
- **Headmaster View**: Defined in `MainSchoolsManagementSystem/Components/Pages/Headmaster/LessonPlans.razor` (lines 196–203):
  ```csharp
  var allPlans = await DbContext.LessonPlans
      .Include(lp => lp.Teacher)
      .Where(lp => lp.Teacher.SchoolId == schoolId.Value)
      .OrderByDescending(lp => lp.UploadedAt)
      .ToListAsync();
  ```
- **Global Settings Update**: Defined in `MainSchoolsManagementSystem/Components/Pages/Admin/Settings.razor` (lines 107–118):
  ```csharp
  var dbSettings = await context.SystemSettings.FirstOrDefaultAsync();
  if (dbSettings != null)
  {
      dbSettings.DailyDeadline = parsedTime;
      dbSettings.MaintenanceMode = maintenanceMode;
      context.SystemSettings.Update(dbSettings);
      await context.SaveChangesAsync();
  }
  ```
- **Seed Data**: Defined in `MainSchoolsManagementSystem/Data/DbSeeder.cs` (lines 158–184):
  Seeds three default lesson plans:
  - Plan 1: On-time, status `Pending`.
  - Plan 2: Late with justification text `"Home internet outage delayed file upload."`, status `Pending`.
  - Plan 3: On-time, status `Reviewed` with feedback `"Excellent curriculum layout. Approved for teaching."`.
- **Existing Tests**: A search for `*test*` or `*Test*` in the file system returned no test files or projects.
- **Build Status**: Command `dotnet build` completed successfully:
  ```
  Build succeeded.
      0 Warning(s)
      0 Error(s)
  ```

---

## 2. Logic Chain
1. **Fact**: The Teacher's lesson plan page (`/teacher/lessonplans`) is currently a placeholder (Observation 3).
2. **Fact**: The Headmaster's review page (`/headmaster/lesson-plans`) is implemented (Observation 4) but doesn't handle downloading of files, and there are no endpoints/controllers in the codebase to serve/download the uploaded files (Observation 1, 3).
3. **Fact**: Lateness is evaluated by comparing the submission time against `SystemSetting.DailyDeadline` (Observation 2, 5).
4. **Fact**: The seeder populates the database with realistic initial data, including both on-time and late submissions (Observation 6).
5. **Conclusion**:
   - The functional requirements of the LessonPlans feature must support uploading files (lesson plans and optional justification attachments), evaluating lateness against `SystemSetting.DailyDeadline`, requiring justification if late, displaying the status, allowing Headmasters/Officers of the same school to review and provide feedback, and allowing secure downloads.
   - An E2E test suite can be designed around these requirements using a 4-tier model (Feature Coverage, Boundary Cases, Cross-Feature, and Real-World Scenarios).

---

## 3. Caveats
- Since the teacher-facing implementation and the file upload/download endpoints are not yet written, E2E tests designed now must treat them as opaque-box requirements that *will* exist in the completed implementation.
- This investigation assumes that the final implementation will adhere to the layout and routing rules specified in `design-system.md` and the existing `Headmaster/LessonPlans.razor`.

---

## 4. Conclusion
We have successfully identified all functional requirements and the existing implementation state of the LessonPlans feature. The requirements are documented in detail in `analysis_requirements.md`. We have designed a 4-tier opaque-box E2E test suite that covers:
1. Feature Coverage (5 tests)
2. Boundary & Corner Cases (5 tests)
3. Cross-Feature Combinations (4 tests, including tenancy and role permission isolation)
4. Real-World Application Scenarios (5 scenarios)

This analysis is ready to guide the next phase of E2E test suite setup and implementation.

---

## 5. Verification Method
1. Inspect the requirements document:
   - Path: `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_lessonplans_requirements\analysis_requirements.md`
2. Run the build command to ensure no regressions:
   - Command: `dotnet build` in `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem`
