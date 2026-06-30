# LessonPlans Feature Design Investigation & Analysis

This report provides a detailed analysis of the CSS styling conventions, design tokens, and the existing Headmaster review flow to inform the design and implementation of the **LessonPlans** feature for teachers in the HelloTwo Schools Management System.

---

## 1. CSS Styling Conventions & Design-System Guidelines

To ensure the new Teacher Lesson Plans page matches the premium theme of the application, we must adhere to the design system outlined in `design-system.md` and implemented in existing portal layouts (e.g., `TeacherLayout.razor`).

### 1.1 Layout & Shell Classes
The Teacher portal uses the same layout structure as the Headmaster and Admin portals:
*   **Root Container:** `<div class="admin-layout">`
*   **Sidebar Overlay (Mobile):** `<div class="sidebar-overlay @(isSidebarOpen ? "show" : "")">`
*   **Sidebar:** `<aside class="admin-sidebar @(isSidebarOpen ? "open" : "")">` with:
    *   `.sidebar-header` containing `<span class="brand-title">HelloTwo Teacher</span>`
    *   `.sidebar-nav` containing `.nav-item` links (with inline SVGs).
    *   `.sidebar-footer` containing the logout form and `.btn-premium.btn-secondary-custom`.
*   **Main Column:** `<main class="admin-main">`
*   **Header:** `<header class="admin-header">` (contains mobile menu toggle, `.header-school` showing "Teacher Portal", and `.header-profile` showing `.profile-name`).
*   **Content Padding:** `<div class="admin-content">` (paddings: `1rem` on mobile, `2rem` on desktop).

### 1.2 Design Tokens (CSS Custom Properties)
All premium styles rely on variables defined in `wwwroot/app.css` under `:root`:

| Token Type | Token Name | Value | Description / Usage |
|---|---|---|---|
| **Colors (BG)** | `--bg-base` | `#f4f6fb` | Page background |
| | `--bg-surface` / `--bg-panel` | `#ffffff` | Cards, panels, headers |
| **Colors (Borders)**| `--border-color` | `#e2e5f0` | Standard divider/border |
| | `--border-2` | `#d0d4e2` | Stronger border (e.g., inputs) |
| **Colors (Text)** | `--text-primary` | `#1a1d2e` | Headings, primary text |
| | `--text-secondary` | `#3b3f54` | Supporting text, body |
| | `--text-muted` | `#7c8298` | Captions, labels, placeholders |
| **Colors (Brand)** | `--primary` | `#6366f1` | Indigo (main action color) |
| | `--primary-hover` | `#4f46e5` | Darker indigo |
| | `--primary-glow` | `rgba(99,99,241,0.08)` | Active nav, focus rings |
| | `--accent-2` | `#8b5cf6` | Purple (for gradient headers/buttons) |
| **Semantic Colors** | `--success` / `-bg` / `-border` | `#10b981` / `10%` opacity | Green (On-time, Approved, Success) |
| | `--warning` / `-bg` / `-border` | `#f59e0b` / `10%` opacity | Amber (Late, Pending) |
| | `--danger` / `-bg` / `-border` | `#f43f5e` / `10%` opacity | Rose (Rejected, Late just.) |
| | `--info` / `-bg` / `-border` | `#06b6d4` / `10%` opacity | Cyan (General info) |
| **Border Radius** | `--radius-lg` | `14px` | Panels, cards, modals |
| | `--radius-md` | `10px` | Tabs |
| | `--radius-sm` | `8px` | Buttons, inputs |
| **Shadows** | `--shadow` | `0 1px 3px ...` | Default card shadow |
| | `--shadow-lg` | `0 4px 24px ...` | Hover states, modals |

### 1.3 Key UI Component Patterns
For the Teacher Lesson Plans page, we should use the following standard components:

1.  **Page Structure:**
    ```html
    <div class="d-flex flex-column" style="gap: 2rem;">
        <!-- Page Header -->
        <div style="display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 1rem;">
            <div>
                <h2 style="font-size: 1.85rem; font-weight: 900; color: var(--text-primary); margin-bottom: 0.4rem;">Lesson Plans</h2>
                <p style="color: var(--text-muted); font-size: 0.92rem;">Upload your daily lesson plans and view review feedback.</p>
            </div>
            <!-- Action Button -->
            <button class="btn-premium" @onclick="OpenSubmitModal">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" style="margin-right: 0.25rem;"><line x1="12" y1="5" x2="12" y2="19"></line><line x1="5" y1="12" x2="19" y2="12"></line></svg>
                Submit Lesson Plan
            </button>
        </div>
        
        <!-- Stats Dashboard Grid (Optional but recommended) -->
        <div class="dashboard-grid">
            <div class="stat-card">
                <span class="stat-value">@totalPlans</span>
                <span class="stat-label">Total Submitted</span>
            </div>
            <div class="stat-card">
                <span class="stat-value" style="color: var(--success);">@onTimePlans</span>
                <span class="stat-label">On Time</span>
            </div>
            <div class="stat-card">
                <span class="stat-value" style="color: var(--warning);">@latePlans</span>
                <span class="stat-label">Late Submissions</span>
            </div>
        </div>

        <!-- Main Data Table Container -->
        <div class="glass-panel" style="padding: 0; overflow: hidden;">
            <div class="table-container">
                <table class="premium-table">
                    ...
                </table>
            </div>
        </div>
    </div>
    ```
2.  **Form Groups (for Modals / Submission Forms):**
    ```html
    <div class="form-group">
        <label class="form-label">Justification for Lateness</label>
        <textarea class="form-control-custom" @bind="justificationText" placeholder="Provide a reason for the late submission..."></textarea>
    </div>
    ```
3.  **Status Badges:**
    *   *Pending Review:* `<span class="badge badge-pending">Pending Review</span>`
    *   *Reviewed:* `<span class="badge badge-present">Reviewed</span>`
    *   *On Time:* `<span class="badge badge-present">On Time</span>`
    *   *Late:* `<span class="badge badge-late">Late Submission</span>`
4.  **Inline Feedback Banners:**
    *   *Success:* `<div style="background: var(--success-bg); border: 1px solid var(--success-border); color: var(--success); ...">`
    *   *Error/Warning:* `<div style="background: var(--warning-bg); border: 1px solid var(--warning-border); color: var(--warning); ...">`

---

## 2. Headmaster Lesson Plans Review & Feedback Flow

The headmaster's side of the Lesson Plans feature is implemented in `Components/Pages/Headmaster/LessonPlans.razor`.

### 2.1 Underlyng Data Structure
The review flow interacts with the `LessonPlan` entity (defined in `Data/LessonPlan.cs`):
```csharp
public enum LessonPlanStatus
{
    Pending = 0,
    Reviewed = 1
}

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

### 2.2 UI Layout and Flow
The Headmaster Lesson Plans page consists of:
1.  **Tabbed Navigation:**
    *   Separates plans into two tabs: **Pending Review** (count of pending plans) and **Reviewed** (count of reviewed plans).
    *   Controlled by the `activeTab` string variable (`"pending"` or `"reviewed"`).
2.  **Roster Table:**
    *   Lists the submissions matching the active tab.
    *   Columns: **Teacher** (Teacher's full name), **Uploaded At** (formatted local time), **Compliance** (badge indicating "Late Submission" or "On Time"), **Status** ("Pending Review" or "Reviewed"), and **Actions** (a button to open the review modal).
3.  **Review & Feedback Modal:**
    *   Triggered via `@onclick="() => OpenReviewModal(plan)"`.
    *   Displays:
        *   **Teacher details** and **submission timestamp**.
        *   **Lateness Justification Banner:** Only shown if `selectedPlan.IsLate` is `true`. Displays the `JustificationText` provided by the teacher.
        *   **Feedback input:** If the plan is `Pending`, a `<textarea class="form-control-custom">` is bound to `feedbackText` for entering feedback. If the plan is `Reviewed`, it displays the existing feedback in a read-only `div`.
        *   **Footer Actions:** A "Close" button. If `Pending`, a "Submit Review & Approve" button is displayed.
4.  **Backend Save & State Update:**
    *   When the Headmaster clicks **Submit Review & Approve**, the page runs `SubmitReview()`:
        *   Sets `selectedPlan.Feedback = feedbackText`.
        *   Sets `selectedPlan.Status = LessonPlanStatus.Reviewed`.
        *   Updates the plan in the database: `DbContext.LessonPlans.Update(selectedPlan)`.
        *   Saves changes: `await DbContext.SaveChangesAsync()`.
        *   Reloads the list: `await LoadLessonPlans()`.
        *   Closes the modal and triggers a UI re-render: `CloseReviewModal()`, `StateHasChanged()`.

---

## 3. Recommended Design for Teacher LessonPlans Page

Based on the Headmaster implementation and the design system, the Teacher-facing `Components/Pages/Teacher/LessonPlans.razor` page should be designed with the following flow:

### 3.1 Submission Flow & Lateness Detection
1.  **Submission Trigger:** The teacher clicks "Submit Lesson Plan" in the page header. This opens a **Submit Lesson Plan Modal** (`showSubmitModal = true`).
2.  **Lateness Check:**
    *   The system fetches the `SystemSetting` from the database.
    *   It checks the current time (`DateTime.Now.TimeOfDay`) against `SystemSetting.DailyDeadline` (configured by the headmaster, defaults to 8:30 AM).
    *   *Dynamic UI behavior:* If the current time is past the deadline, the modal dynamically displays a warning banner:
        > ⚠️ **Late Submission Warning:** The daily deadline of `@settings.DailyDeadline` has passed. You must provide a justification for this late submission.
        *   A `JustificationText` `<textarea>` field becomes visible and **required** in the form.
3.  **Attachment Upload:**
    *   If the submission is late and requires supporting documentation (indicated by `HasJustificationAttachment`), we should provide a file input (e.g., `<InputFile>`) to upload a document, setting `HasJustificationAttachment = true`.
4.  **Save Action:**
    *   Creates a new `LessonPlan` entity.
    *   Sets `TeacherId = currentUserId`, `UploadedAt = DateTime.UtcNow`, `Status = LessonPlanStatus.Pending`.
    *   Determines `IsLate` based on the deadline comparison.
    *   Saves the record to `DbContext.LessonPlans` and redirects/updates the dashboard list.

### 3.2 View Submissions & Feedback
1.  **Dashboard Table:**
    *   Lists all lesson plans submitted by the logged-in teacher (`TeacherId == currentUserId`), ordered by `UploadedAt` descending.
    *   Columns: **Uploaded At**, **Deadline Status** ("On Time" / "Late Submission"), **Review Status** ("Pending Review" / "Reviewed"), and **Actions**.
2.  **Feedback Viewer Modal:**
    *   For plans with the status `Reviewed`, the Action column should have a "View Feedback" button.
    *   Clicking it opens a modal displaying the Headmaster's feedback (`Feedback` field) in a clean, read-only premium layout.
