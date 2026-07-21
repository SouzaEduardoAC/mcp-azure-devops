# Product Requirements Document (PRD): Work Item Activity & State Transition History

**Feature Title:** Azure DevOps Work Item Activity & State Transition History  
**Target Repository:** `SouzaEduardoAC/mcp-azure-devops` (Forked from `viamus/mcp-azure-devops`)  
**Document Status:** Draft — Pending Phase 1 Gate Sign-off  

---

## 1. Executive Summary & Problem Statement

Currently, when LLM agents perform board analysis or query work items using tools like `query_work_items` or `get_work_item`, the returned data includes static summaries and basic dates (`activatedDate`, `closedDate`). However, LLMs lack visibility into the **chronological activity history** of the card.

Specifically, downstream LLMs cannot answer critical agile questions such as:
- *When was a card moved to a specific column/state, and by whom?*
- *How long did a card sit in "Code Review" or "Blocked" before moving forward?*
- *Who touched or transitioned the card across its lifecycle?*

This PRD defines the requirements to retrieve and return complete state transition history (column/state name, author, and timestamp) for work items in `mcp-azure-devops`.

---

## 2. Target Users & Use Cases

### Target Users
- **LLM Board Analysts & AI Agents:** Automated assistants analyzing agile board efficiency, bottlenecks, and cycle times.
- **Scrum Masters & Engineering Managers:** Users asking AI to summarize team throughput and column lead times.

### Key Use Cases
1. **Board Bottleneck Analysis:** An LLM queries board work items and calculates lead time per column based on state transition timestamps.
2. **Audit Trail:** An LLM audits card movements to identify frequent back-and-forth state changes (re-opens, failed QA passes).
3. **Card Summary Enhancement:** Detailed card inspection includes a chronological activity timeline of who moved the card and when.

---

## 3. Scope & Requirements (MoSCoW)

### 3.1 Must Have (V1 Scope)
- **State Transition History Data Model:**
  - `state` / `column`: Target state or board column name (e.g., `Active`, `In Review`, `Done`).
  - `movedBy`: Display name or unique identity of the user who performed the transition.
  - `timestamp`: UTC DateTime of the transition.
  - `previousState`: (Optional/Recommended) The state prior to transition.
- **Azure DevOps REST Integration:**
  - Query Azure DevOps Work Item Updates endpoint (`GET _apis/wit/workItems/{id}/updates`) to parse history revisions.
  - Filter revisions for changes to `System.State` or `Board.Column` / `System.BoardColumn`.
- **Dedicated MCP Tools:**
  - `get_work_item_history(workItemId, project, organization)`: Retrieves the complete activity history for a single work item.
  - `get_work_items_history(workItemIds, project, organization)`: Retrieves activity history for a batch of work items (comma-separated IDs) concurrently using `Task.WhenAll`.
- **Opt-in Extension on Work Item Queries:**
  - Add optional `includeHistory` (boolean, default: `false`) parameter to `get_work_item` and `get_work_items` to avoid performance degradation on simple queries.

### 3.2 Should Have
- **Time in State Calculation:** Include calculated `durationMinutes` / `durationHours` for completed state transitions.

### 3.3 Could Have
- **Board Column vs. State Mapping:** Disambiguate customized Kanban board column names from standard Azure DevOps WIT `System.State` if configured in the team board settings.

### 3.4 Won't Have (Out of V1 Scope)
- Full text diffs of field changes (e.g. description changes, comment edits) outside of state/column transitions.

---

## 4. Technical Constraints & Performance Considerations

1. **Payload & N+1 Performance Impact:**
   - WIQL queries (`query_work_items`) return up to 20 items per page. Fetching updates for every work item synchronously in a bulk list can cause `20 + 1` HTTP requests to Azure DevOps.
   - *Requirement:* `includeHistory` must be `false` by default. Batching or lazy fetching must be implemented when querying multiple items.
2. **API Endpoint Compatibility:**
   - Must support Azure DevOps Services (`dev.azure.com`) and Azure DevOps Server (on-premise / TFS REST APIs).
3. **Backward Compatibility:**
   - Existing JSON output schemas for existing tools must remain untouched when `includeHistory` is omitted.

---

## 5. RICE Prioritization

| Feature Component | Reach | Impact | Confidence | Effort | RICE Score |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **`get_work_item_history` tool** | High (100%) | High (2.0) | High (100%) | Low (2) | **100.0** |
| **`includeHistory` parameter on `get_work_item`** | Medium (50%) | High (2.0) | High (90%) | Low (1) | **90.0** |
| **Duration per state calculation** | Low (30%) | Medium (1.0) | Medium (80%) | Low (1) | **24.0** |

---

## 6. Definition of Done (Phase 1 Exit Criteria)

- [x] PRD drafted and published to `docs/workitem-history-prd.md`.
- [ ] PRD reviewed and approved by user (`prd` gate approved via `request_approval`).
- [ ] Architect phase initiated for technical design.
