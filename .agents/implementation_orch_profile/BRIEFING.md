# BRIEFING — 2026-06-30T06:11:27+06:30

## Mission
Implement the Profile Page feature according to the requirements and design system, ensure it passes all E2E tests, and perform adversarial hardening.

## 🔒 My Identity
- Archetype: orchestrator
- Roles: orchestrator, user_liaison, human_reporter, successor
- Working directory: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\implementation_orch_profile\
- Original parent: main agent
- Original parent conversation ID: 4e6b7263-9bf9-43cd-b66a-64eeed07c5ac

## 🔒 My Workflow
- **Pattern**: Project
- **Scope document**: c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\PROJECT.md
1. **Decompose**: Decomposed the implementation into 4 milestones matching the user request.
2. **Dispatch & Execute**: Iterate through each milestone using the Explorer -> Worker -> Reviewer -> Challenger -> Auditor cycle.
3. **On failure** (in this order):
   - Retry: nudge stuck agent or re-send task
   - Replace: spawn fresh agent with partial progress
   - Skip: proceed without (only if non-critical)
   - Redistribute: split stuck agent's remaining work
   - Redesign: re-partition decomposition
   - Escalate: report to parent (sub-orchestrators only, last resort)
4. **Succession**: Self-succeed at 16 spawns. Write handoff.md, spawn successor.
- **Work items**:
  1. DB Schema Extension [in-progress]
  2. Profile Page UI, Editing, and Validation [pending]
  3. Profile Picture Upload, Storage, and Avatar Display [pending]
  4. Design System Compliance & Refinement [pending]
- **Current phase**: 1 (Implementation of Milestones)
- **Current focus**: Milestone 1: DB Schema Extension

## 🔒 Key Constraints
- NEVER write, modify, or create source code files directly.
- NEVER run build/test commands yourself — require workers to do so.
- Run the Forensic Auditor on every iteration to verify integrity (no cheating, no hardcoding).
- Never reuse a subagent after it has delivered its handoff — always spawn fresh.

## Current Parent
- Conversation ID: 4e6b7263-9bf9-43cd-b66a-64eeed07c5ac
- Updated: not yet

## Key Decisions Made
- Decomposed the implementation into 4 milestones as requested.
- Decided to un-ignore `PhoneNumber` in `ApplicationDbContext.cs` to satisfy the requirement for an editable phone number.

## Team Roster
| Agent | Type | Work Item | Status | Conv ID |
|-------|------|-----------|--------|---------|
| Explorer 1 | teamwork_preview_explorer | Milestone 1 Analysis | completed | 33220ba2-79e9-4b09-9ee5-38a26241ae02 |
| Explorer 2 | teamwork_preview_explorer | Milestone 1 Analysis | completed | ad7f65ca-bdd6-4eec-a494-db2cab1561b2 |
| Explorer 3 | teamwork_preview_explorer | Milestone 1 Analysis | completed | 1e74eb6d-e298-4259-a466-89ce31b278a4 |
| Worker 1 | teamwork_preview_worker | Milestone 1 Implementation | pending | 0d9ec127-b62f-4aef-9611-ba65e6f8bc5c |

## Succession Status
- Succession required: no
- Spawn count: 4 / 16
- Pending subagents: 0d9ec127-b62f-4aef-9611-ba65e6f8bc5c
- Predecessor: none
- Successor: not yet spawned

## Active Timers
- Heartbeat cron: 3e6f64c1-4c0a-4057-a762-bcf3869ac3e4/task-14
- Safety timer: none

## Artifact Index
- c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\implementation_orch_profile\ORIGINAL_REQUEST.md — Verbatim user request
- c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\implementation_orch_profile\BRIEFING.md — This briefing document
- c:\Users\sanlphyo\workspace\csharpworkspace\SchoolsManagementSystem\.agents\implementation_orch_profile\progress.md — Liveness and task progress tracker
