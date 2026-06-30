# Original User Request

## Initial Request — 2026-06-30T06:11:27+06:30

You are the Implementation Orchestrator for the Profile Page feature in the HelloTwo Schools Management System.
Your working directory is: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\implementation_orch_profile\

Your mission:
Implement the Profile Page feature according to the requirements and design system, ensure it passes all E2E tests, and perform adversarial hardening.

Instructions:
1. Read the requirements in c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\ORIGINAL_REQUEST.md, the project plan in c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\PROJECT.md, and the design system in c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\design-system.md.
2. Decompose the implementation into milestones:
   - Milestone 1: DB Schema Extension (Add ProfilePicturePath to ApplicationUser, generate and apply EF migration)
   - Milestone 2: Profile Page UI, Editing, and Validation (Revamp Index.razor with form, validation, and database saving)
   - Milestone 3: Profile Picture Upload, Storage, and Avatar Display (Upload control, secure storage, and avatar with fallback)
   - Milestone 4: Design System Compliance & Refinement (Style according to design-system.md)
3. For each milestone, spawn subagents (Explorer, Worker, Reviewer, Challenger, Auditor) to implement, review, and verify the changes.
4. MANDATORY: On every iteration, run the Forensic Auditor to verify integrity (no cheating, no hardcoding).
5. Once the E2E Testing Orchestrator publishes `TEST_READY.md` at the project root, run the E2E test suite and fix any failures.
6. Perform Phase 2: Adversarial Coverage Hardening (Tier 5) using Challengers to find gaps and generate adversarial tests.
7. Maintain your own plan, progress, and context in your working directory.
8. When finished, write a handoff report and send a message to your parent (conv ID: 4e6b7263-9bf9-43cd-b66a-64eeed07c5ac) with your results. Do NOT write code directly yourself; spawn workers or explorers.
