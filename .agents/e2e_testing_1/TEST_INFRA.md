# E2E Test Infra: LessonPlans Feature

## Test Philosophy
- **Opaque-Box**: The E2E tests interact with the system solely through its public HTTP endpoints and user interfaces (simulated via Playwright). They do not rely on internal implementation details, C# class structures, or private methods.
- **Requirement-Driven**: All test cases are directly derived from the user requirements for the Teachers, Headmasters, and Admins.
- **Data Integrity and Security**: Tenancy isolation and role-based access control are verified at both the UI level and the API/endpoint level.

---

## Feature Inventory

### Feature A: Teacher Submission & History
1. **On-Time Submission**: Uploading a lesson plan file before the daily deadline (`SystemSettings.DailyDeadline`).
2. **Late Submission**: Uploading a lesson plan file after the daily deadline, which requires entering a justification text and optionally uploading a justification file.
3. **Submission History**: Viewing the list of all uploaded lesson plans, sorted by `UploadedAt` descending, showing compliance status, review status, and feedback.
4. **File Downloads**: Downloading the uploaded lesson plan file and justification attachment.

### Feature B: Headmaster Dashboard & Review
1. **Review Dashboard**: Viewing pending and reviewed lesson plans for teachers belonging to the same school.
2. **Review Action**: Submitting feedback and approving a pending lesson plan.
3. **Tenancy Isolation**: Restricting access so Headmasters can only see and review lesson plans from teachers in their own school.
4. **Feedback View**: Displaying the feedback as read-only for already reviewed lesson plans.

### Feature C: Admin Policy & Access Control
1. **Deadline Configuration**: Updating the global daily submission deadline.
2. **Maintenance Mode**: Enabling/disabling maintenance mode, which blocks non-admin access.
3. **Role-Based Access Control (RBAC)**: Ensuring unauthorized roles cannot access teacher, headmaster, or admin pages/endpoints.

---

## 4-Tier Test Case Design

### Tier 1: Feature Coverage
- **Test 1.1: Teacher On-Time Submission Flow**
  - *Action*: Log in as a Teacher, upload a lesson plan before the daily deadline.
  - *Expected*: Submission succeeds, marked "On Time" and "Pending Review". No justification requested.
- **Test 1.2: Teacher Late Submission Flow (With Attachment)**
  - *Action*: Log in as a Teacher, upload a lesson plan after the daily deadline, provide justification text and an attachment.
  - *Expected*: Submission succeeds, marked "Late Submission" and "Pending Review".
- **Test 1.3: Teacher Late Submission Flow (Without Attachment)**
  - *Action*: Log in as a Teacher, upload a lesson plan after the daily deadline, provide justification text only.
  - *Expected*: Submission succeeds, marked "Late Submission" and "Pending Review".
- **Test 1.4: Headmaster Review and Approval**
  - *Action*: Log in as a Headmaster, view the pending plan, enter feedback, and click "Submit Review & Approve".
  - *Expected*: Plan moves to "Reviewed" status with the feedback saved.
- **Test 1.5: Teacher Feedback Visibility & Downloads**
  - *Action*: Log in as the Teacher, view the reviewed plan, verify the feedback text, and download the uploaded files.
  - *Expected*: Feedback is visible. Downloaded files are identical to the uploaded files.

### Tier 2: Boundary & Corner Cases
- **Test 2.1: Deadline Boundary - Exactly On-Time**
  - *Action*: Submit a lesson plan at exactly the deadline time (e.g., `08:30:00`).
  - *Expected*: Marked as "On Time".
- **Test 2.2: Deadline Boundary - Marginally Late**
  - *Action*: Submit a lesson plan exactly 1 second after the deadline (e.g., `08:30:01`).
  - *Expected*: Marked as "Late Submission", requiring justification.
- **Test 2.3: Validation - Missing Late Justification**
  - *Action*: Attempt to submit a late lesson plan without entering justification text.
  - *Expected*: Submission is blocked; validation error is displayed.
- **Test 2.4: Optional Justification Attachment**
  - *Action*: Submit a late lesson plan with justification text but no justification file.
  - *Expected*: Submission succeeds (justification file is optional).
- **Test 2.5: Concurrent Submissions**
  - *Action*: Submit multiple lesson plans in rapid succession from the same teacher.
  - *Expected*: All records and files are saved correctly without concurrency conflicts.

### Tier 3: Cross-Feature Combinations
- **Test 3.1: Multi-Tenant Isolation**
  - *Action*: Log in as Headmaster of School A. Try to view or download lesson plans from School B.
  - *Expected*: School B plans are not visible in the dashboard. Directly hitting download endpoints for School B files returns `403 Forbidden` or `404 Not Found`.
- **Test 3.2: Role Permission Enforcement**
  - *Action*: Attempt to access `/headmaster/lesson-plans` as a Teacher, or `/teacher/lessonplans` as a Headmaster.
  - *Expected*: Access denied or redirected to login/unauthorized page.
- **Test 3.3: Global Deadline Change Interaction**
  - *Action*: Log in as Admin, change the deadline from `08:30` to `09:00` in `/admin/settings`. Log in as a Teacher, upload at `08:45`.
  - *Expected*: The upload is marked as "On Time".
- **Test 3.4: Maintenance Mode Interaction**
  - *Action*: Log in as Admin, enable Maintenance Mode. Attempt to access `/teacher/lessonplans` or `/headmaster/lesson-plans` as Teacher/Headmaster.
  - *Expected*: Non-admin users are blocked from accessing the feature.

### Tier 4: Real-World Application Scenarios
- **Scenario 4.1: Late Upload Recovery Workflow**
  - *Steps*: Teacher uploads a plan late, forgets justification (validation fails), adds justification, completes upload, and verifies it appears in their history.
- **Scenario 4.2: Full School Review Cycle**
  - *Steps*: Three teachers submit plans (2 on-time, 1 late). The Headmaster logs in, downloads the late justification file, reviews and approves all three, providing unique feedback. Both teachers verify their updated status and feedback.
- **Scenario 4.3: Multi-Tenant Simultaneous Work**
  - *Steps*: Headmasters from two different schools review their respective dashboards simultaneously. They verify that only their own teachers' plans are visible and no data is cross-contaminated.
- **Scenario 4.4: Admin Policy Shift**
  - *Steps*: Admin changes the daily deadline mid-week. Teachers submitting after the change are evaluated under the new deadline, while previously submitted plans remain unchanged.
- **Scenario 4.5: File Integrity & Security Verification**
  - *Steps*: Teacher uploads a PDF lesson plan and a PNG justification. Headmaster downloads both and verifies that the file sizes, hashes, and extensions match the original files. Unauthenticated users attempting to download these files are redirected to login.

---

## Test Architecture
- **Test Runner**: xUnit with Playwright for .NET.
- **Target Project**: `MainSchoolsManagementSystem.Tests`
- **Database Setup**: The tests will run against the application's database, utilizing a transaction or seeding/cleanup script to ensure a clean state before each test run.
- **Authentication**: Playwright's `StorageState` will be used to save authentication states for `Teacher`, `Headmaster`, and `Admin` roles, avoiding redundant logins.

## Coverage Thresholds
- **Tier 1**: 100% of defined feature coverage tests must pass.
- **Tier 2**: 100% of boundary and corner cases must pass.
- **Tier 3**: 100% of cross-feature combination tests must pass.
- **Tier 4**: 100% of real-world application scenarios must pass.
