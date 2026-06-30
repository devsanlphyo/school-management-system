## 2026-06-29T23:39:16Z

You are a teamwork_preview_explorer. Your task is to investigate the SchoolsManagementSystem codebase to help design the implementation of the LessonPlans feature.
Specifically, please answer the following questions and provide code snippets/references where appropriate:
1. Are there any existing file upload implementations or helpers in the project? If so, how are they implemented?
2. How is the DailyDeadline in SystemSetting queried or used elsewhere? Is there always a single record in the SystemSettings table?
3. The database model for LessonPlan does not store the file extension (it only has HasJustificationAttachment, IsLate, etc.). How should we handle finding and serving the uploaded lesson plan and justification files with their correct extensions when displaying/downloading them? (e.g., should we scan the wwwroot/uploads directory for files matching lessonplan_{id}.*?).
4. Where is the DbContext defined and how are database queries typically structured in the Blazor components (e.g., using @inject IDbContextFactory or injecting DbContext directly)?
5. What are the CSS styling conventions, classes, or design tokens we should use for the Teacher portal page to match the premium theme of the application? (Check design-system.md or other teacher pages).
6. Does the `wwwroot/uploads` directory exist? If not, should the application create it dynamically?

Please write your findings to `c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\explorer_1\analysis.md` and provide a handoff.md in your directory.
