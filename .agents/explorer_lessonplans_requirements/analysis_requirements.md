# LessonPlans Feature: Requirements & Analysis Report

This report provides a comprehensive analysis of the **LessonPlans** feature within the `SchoolsManagementSystem` codebase. The findings are structured to define the functional requirements from an end-user's perspective, facilitating the design of a requirement-driven, opaque-box End-to-End (E2E) test suite.

---

## 1. Executive Summary
The **LessonPlans** feature is designed to allow Teachers to submit weekly lesson plans and receive feedback from their respective Headmasters or Officers. Currently:
- The **Teacher-facing interface** (`/teacher/lessonplans`) is a placeholder under development.
- The **Headmaster-facing interface** (`/headmaster/lesson-plans`) is semi-implemented, allowing list viewing and submitting feedback but lacking file upload/download integration.
- There are **no existing unit, integration, or E2E tests** in the solution.
- The database schema for `LessonPlan` and global `SystemSetting` (containing the daily deadline) is fully defined and migrated.

---

## 2. End-User Functional Requirements

### 2.1 Teacher Persona
A Teacher interacts with the system to submit and track their lesson plans.
1. **Access Control**: Only users with the `Teacher` role can access `/teacher/lessonplans`.
2. **View Uploaded Lesson Plans**:
   - Displays a list of all lesson plans uploaded by the logged-in teacher, sorted by `UploadedAt` descending.
   - For each plan, displays:
     - Upload timestamp.
     - Compliance status: **On Time** or **Late Submission**.
     - Review status: **Pending Review** or **Reviewed**.
     - Feedback (if reviewed).
     - Download links for the lesson plan file and justification attachment (if applicable).
3. **Submit Lesson Plan**:
   - The teacher can upload a lesson plan file (e.g., PDF, Word, Image).
   - **Lateness Check**: The system automatically compares the submission time against the global daily deadline (configured in `SystemSettings.DailyDeadline`, which defaults to `08:30:00`).
     - **On-Time Submission**: If submitted before or at the deadline, `IsLate` is set to `false`.
     - **Late Submission**: If submitted after the deadline, `IsLate` is set to `true`.
       - The UI must require the teacher to provide a **Justification Text**.
       - The UI must optionally allow the teacher to upload a **Justification Attachment** (file).
   - Upon successful submission, the record is saved with status `Pending`.

### 2.2 Headmaster / Officer Persona
A Headmaster or Officer reviews the lesson plans submitted by teachers within their school.
1. **Access Control**: Only users with `Admin`, `Headmaster`, or `Officer` roles can access `/headmaster/lesson-plans`.
2. **School Tenancy Isolation**:
   - A Headmaster/Officer can only view or review lesson plans submitted by teachers belonging to the same school (`SchoolId` matches).
3. **Review Dashboard**:
   - Tabbed navigation separating **Pending Review** and **Reviewed** plans.
   - Lists: Teacher's name, Upload timestamp, Compliance status (On Time / Late), and Status.
   - Action:
     - Clicking **Review & Feedback** (for Pending) or **View Feedback** (for Reviewed) opens a modal.
     - Offers download links for the lesson plan file and justification attachment (if any).
4. **Review & Approval**:
   - In the modal, the reviewer can view the lateness justification if the plan was late.
   - For pending plans, the reviewer can enter feedback in a textarea and click **Submit Review & Approve**.
   - This transitions the plan's status to `Reviewed` and stores the feedback. For reviewed plans, the feedback is displayed as read-only.

### 2.3 Admin Persona
An Admin manages global policies that dictate lesson plan compliance.
1. **Access Control**: Only users with the `Admin` role can access `/admin/settings`.
2. **Global Policy Configuration**:
   - The Admin can update the **Daily Submission Deadline** (saved in the `SystemSettings` table).
   - Any lesson plans uploaded after the new deadline will be evaluated against it.

---

## 3. Technical & Database Architecture

### 3.1 Database Schema
The database models are defined in `MainSchoolsManagementSystem/Data/`:

#### `LessonPlan` (in `LessonPlan.cs`)
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
    public LessonPlanStatus Status { get; set; } = LessonPlanStatus.Pending; // Pending = 0, Reviewed = 1
    public string? Feedback { get; set; }
}
```

#### `SystemSetting` (in `SystemSetting.cs`)
There is a single global record representing the system settings.
```csharp
public class SystemSetting
{
    public int Id { get; set; }
    public TimeSpan DailyDeadline { get; set; } = new TimeSpan(8, 30, 0);
    public bool MaintenanceMode { get; set; } = false;
}
```

### 3.2 File Storage & Serving Strategy
Because the `LessonPlan` model does not store file paths or extensions:
1. **Storage Location**: Files are saved in a local directory (recommended: a root-level `uploads/` folder for security, avoiding public access via `wwwroot`).
2. **Naming Convention**:
   - Lesson Plan file: `lessonplan_{id}.{extension}`
   - Justification file: `justification_{id}.{extension}`
3. **Resolution**: The system scans the directory using `Directory.EnumerateFiles(uploadsFolder, "lessonplan_{id}.*")` to dynamically resolve the file extension at runtime.
4. **Authorized Endpoints**: Custom endpoints (e.g., `/api/lessonplans/{id}/download`) must be created to verify permissions and serve files with correct MIME types.

---

## 4. Proposed 4-Tier E2E Test Suite Design (Opaque-Box)

To verify the LessonPlans feature end-to-end without relying on implementation details, we propose a 4-tier E2E test suite.

### Tier 1: Feature Coverage
1. **Test 1.1: On-Time Submission Flow**
   - **Action**: Log in as a Teacher, navigate to `/teacher/lessonplans`, and upload a lesson plan file before the daily deadline (e.g., at 08:00 AM).
   - **Expected**: Submission succeeds. The plan appears in the teacher's list as "On Time" and "Pending Review". Justification fields are not displayed.
2. **Test 1.2: Late Submission Flow**
   - **Action**: Log in as a Teacher, navigate to `/teacher/lessonplans`, and upload a lesson plan file after the daily deadline (e.g., at 09:00 AM).
   - **Expected**: The UI prompts for justification. The teacher enters justification text, uploads a justification file, and submits. The plan appears in the list as "Late Submission" and "Pending Review".
3. **Test 1.3: Headmaster Review Flow**
   - **Action**: Log in as a Headmaster, navigate to `/headmaster/lesson-plans`, view the pending plan, open the review modal, enter feedback, and submit.
   - **Expected**: The plan moves from "Pending Review" to the "Reviewed" tab.
4. **Test 1.4: Teacher Feedback Visibility**
   - **Action**: Log in as the Teacher, navigate to `/teacher/lessonplans`, and inspect the reviewed plan.
   - **Expected**: Status is updated to "Reviewed", and the headmaster's feedback text is visible.
5. **Test 1.5: File Download Capability**
   - **Action**: Log in as the Teacher (or Headmaster) and click the download links for the lesson plan and justification files.
   - **Expected**: The files download successfully, and their contents match the uploaded files.

### Tier 2: Boundary & Corner Cases
1. **Test 2.1: Deadline Boundary - Exactly On-Time**
   - **Action**: Submit a lesson plan at exactly the deadline time (e.g., `08:30:00`).
   - **Expected**: Marked as "On Time".
2. **Test 2.2: Deadline Boundary - Marginally Late**
   - **Action**: Submit a lesson plan exactly 1 second after the deadline (e.g., `08:30:01`).
   - **Expected**: Marked as "Late Submission", requiring justification.
3. **Test 2.3: Validation - Missing Late Justification**
   - **Action**: Attempt to submit a late lesson plan without entering justification text.
   - **Expected**: Submission is blocked; validation error is displayed.
4. **Test 2.4: Optional Justification Attachment**
   - **Action**: Submit a late lesson plan with justification text but no justification file.
   - **Expected**: Submission succeeds (justification file is optional).
5. **Test 2.5: Concurrent Submissions**
   - **Action**: Submit multiple lesson plans in rapid succession from the same teacher.
   - **Expected**: All records and files are saved correctly without concurrency conflicts.

### Tier 3: Cross-Feature Combinations
1. **Test 3.1: Multi-Tenant Isolation**
   - **Action**: Log in as Headmaster of School A. Try to view or download lesson plans from School B.
   - **Expected**: School B plans are not visible in the dashboard. Directly hitting download endpoints for School B files returns `403 Forbidden` or `404 Not Found`.
2. **Test 3.2: Role Permission Enforcement**
   - **Action**: Attempt to access `/headmaster/lesson-plans` as a Teacher, or `/teacher/lessonplans` as a Headmaster.
   - **Expected**: Access denied or redirected to login/unauthorized page.
3. **Test 3.3: Global Deadline Change Interaction**
   - **Action**: Log in as Admin, change the deadline from `08:30` to `09:00` in `/admin/settings`. Log in as a Teacher, upload at `08:45`.
   - **Expected**: The upload is marked as "On Time".
4. **Test 3.4: Maintenance Mode Interaction**
   - **Action**: Log in as Admin, enable Maintenance Mode. Attempt to access `/teacher/lessonplans` or `/headmaster/lesson-plans`.
   - **Expected**: Non-admin users are blocked from accessing the feature.

### Tier 4: Real-World Application Scenarios
1. **Scenario 4.1: Late Upload Recovery Workflow**
   - A teacher uploads a plan late, forgets justification (validation fails), adds justification, completes upload, and verifies the submission appears in their history.
2. **Scenario 4.2: Full School Review Cycle**
   - Three teachers submit plans (2 on-time, 1 late). The Headmaster logs in, downloads the late justification file, reviews and approves all three, providing unique feedback for each. Both teachers verify their updated status and feedback.
3. **Scenario 4.3: Multi-Tenant Simultaneous Work**
   - Headmasters from St. Jude Academy and Oakridge High review their respective dashboards simultaneously. They verify that only their own teachers' plans are visible and that no data is cross-contaminated.
4. **Scenario 4.4: Admin Policy Shift**
   - An Admin changes the daily deadline mid-week. Teachers submitting after the change are evaluated under the new deadline, while previously submitted plans remain unchanged.
5. **Scenario 4.5: File Integrity Verification**
   - A teacher uploads a PDF lesson plan and a PNG justification. The Headmaster downloads both and verifies that the file sizes, hashes, and extensions match the original files perfectly.

---

## 5. Opaque-Box E2E Test Strategy & Setup Recommendations

To implement this suite successfully:
1. **Test Tooling**: Playwright for .NET is recommended. It integrates seamlessly with ASP.NET Core, supports authentication state sharing, and runs headlessly in CI/CD.
2. **Test Database**: Use a separate test database instance (or SQLite in-memory if supported, though SQL Server is preferred since the main app uses it). Run migrations and seed default users/settings before running tests.
3. **Authentication Mocking**: Utilize Playwright's storage state to log in once per role (Admin, Headmaster, Teacher) and reuse the session cookies across tests to speed up execution.
4. **Time Manipulation**: For boundary tests (Tiers 1 & 2), either:
   - Programmatically adjust the `SystemSettings.DailyDeadline` in the database before the test to make the current time "before" or "after" the deadline.
   - Use a library to mock `DateTime.Now` in the application if running in-process, or adjust the system clock. Programmatic deadline adjustment is the most robust and opaque-box method.
5. **File Verification**: Store test files in a `TestData` directory. Upload them via Playwright's `SetInputFilesAsync` method. Verify downloads by capturing the download event in Playwright and asserting file content/hash matches.
