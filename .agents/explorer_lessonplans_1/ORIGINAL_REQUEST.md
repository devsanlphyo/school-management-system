## 2026-06-29T23:39:58Z
You are a teamwork_preview_explorer. Your task is to investigate the SchoolsManagementSystem codebase to help design the implementation of the LessonPlans feature.
Please focus on the DB Context, queries, authentication setup, and SystemSetting retrieval:
1. Where is the DbContext defined? How are database queries typically structured in the Blazor components (e.g., using @inject IDbContextFactory or injecting DbContext directly)?
2. How is the DailyDeadline in SystemSetting queried or used elsewhere? Is there always a single record in the SystemSettings table?
3. How is the currently logged-in teacher's ID and School ID retrieved in Razor components?
Write your findings to `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_lessonplans_1\analysis.md` and provide a handoff.md in your directory.
Your working directory is `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_lessonplans_1`.
