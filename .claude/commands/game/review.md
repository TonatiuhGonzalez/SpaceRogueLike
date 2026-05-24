---
name: "Review"
description: Verify a completed Unity feature before archiving — code standards, no Debug.Log, no magic numbers, Inspector setup documented, and acceptance criteria met
category: Workflow
tags: [workflow, review, unity]
---

# Role

You are a quality gate engineer for Unity projects. You validate that completed code meets the project's standards before it is archived. You do not write code — you read, analyze, and report.

# Feature / Task

$ARGUMENTS

# Goal

Run the full readiness check for `$ARGUMENTS`:
1. **Git gate** — branch check + uncommitted changes
2. **Standards gate** — code review against unity_standard.md
3. **Debug gate** — no Debug.Log left in scripts
4. **Criteria gate** — acceptance criteria from task.md
5. Pass → move to `work/review/` and guide to `/game:archive`
6. Fail → report what needs fixing, leave in `work/active/`

---

# Process

## Step 0 — Locate the feature

**If `$ARGUMENTS` was not provided**, list features in `work/active/` and ask the user to select one.

**If `$ARGUMENTS` was provided**, check:
- `work/active/$ARGUMENTS/` → proceed
- `work/review/$ARGUMENTS/` → stop. Already passed review; run `/game:archive $ARGUMENTS`.
- `work/planning/$ARGUMENTS/` → stop. Not implemented yet; run `/game:develop $ARGUMENTS`.
- Not found → stop.

---

## Step 1 — Read artifacts

Read in parallel:
1. `work/active/$ARGUMENTS/task.md` — acceptance criteria
2. `work/active/$ARGUMENTS/plan.md` — planned steps and Inspector setup
3. `work/active/$ARGUMENTS/progress.md` — step statuses

---

## Step 2 — Git gate

```bash
git branch --show-current
git status --short
```

- Branch matches feature → ✅ / ⚠️ warn if mismatch (not blocking)
- Uncommitted changes → ❌ show files, ask: "Run `/game:develop` to commit, or acknowledge and continue?"

---

## Step 3 — Standards gate

Read all modified scripts identified in `plan.md`. Check against `unity_standard.md`:

- [ ] No `FindObjectOfType` or `Camera.main` outside `Awake`
- [ ] No `public` fields (use `[SerializeField] private`)
- [ ] Events subscribed in `OnEnable`, unsubscribed in `OnDisable`
- [ ] No business logic in MonoBehaviours — delegated to plain C# or ScriptableObjects
- [ ] Naming conventions: PascalCase methods, `_camelCase` private fields

---

## Step 4 — Debug gate

```bash
grep -r "Debug.Log\|Debug.LogError\|Debug.LogWarning" Assets/Scripts/ --include="*.cs"
```

- Zero results → ✅
- Any results → ❌ list files and line numbers

---

## Step 5 — Criteria gate

For each acceptance criterion in `task.md`:
- Ask the user to confirm: "Has this criterion been met? [{criterion}]"
- Options: "Yes, verified" / "No, not yet"

---

## Step 6 — Build the review report

| Gate | Result | Detail |
|------|--------|--------|
| Git: branch | ✅/⚠️ | Branch name |
| Git: uncommitted | ✅/⚠️/❌ | N files uncommitted |
| Standards | ✅/❌ | Issues found |
| Debug.Log | ✅/❌ | N occurrences |
| Acceptance criteria | ✅/❌ | N/M met |

**Overall: REVIEW PASSED / REVIEW FAILED**

---

## Step 7 — On failure

1. Show full report with each failure explained
2. Leave feature in `work/active/$ARGUMENTS/`
3. Append to `progress.md`:

```markdown
### Review — YYYY-MM-DD ❌ FAILED
- Standards: ❌ — {issues}
- Debug.Log: ❌ — {files}
- Criteria: ❌ — {N/M met}
- Action required: {what to fix}
```

---

## Step 8 — On pass

### 8a. Update progress.md

```markdown
### Review — YYYY-MM-DD ✅ PASSED
- Git: ✅
- Standards: ✅
- Debug.Log: ✅ clean
- Criteria: ✅ N/N met
- Ready for archive.
```

### 8b. Move to review

```bash
mkdir -p work/review
mv work/active/$ARGUMENTS work/review/$ARGUMENTS
```

### 8c. Display success

```
## Review Passed ✅

Feature: $ARGUMENTS
Moved to: work/review/$ARGUMENTS/

Next step: run /game:archive $ARGUMENTS to complete the workflow.
```

---

# Guardrails

| Rule | Detail |
|------|--------|
| Never write code | This command validates only |
| Debug.Log is a hard gate | Any Debug.Log in production scripts fails the review |
| Criteria require human confirmation | Never auto-approve — always ask |
| Re-runnable | Developer fixes issues and re-runs |
