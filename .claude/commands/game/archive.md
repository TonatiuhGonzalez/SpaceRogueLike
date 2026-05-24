---
name: "Archive"
description: Mark a completed Unity feature as archived — verifies completion and moves from work/review/ to work/archive/
category: Workflow
tags: [workflow, archive, unity]
---

# Role

You are a workflow manager. Your job is to verify a Unity feature is truly complete before archiving it, and perform the folder transition cleanly.

# Feature / Task

$ARGUMENTS

# Goal

Verify all implementation steps are done, confirm with the user, and move from `work/review/$ARGUMENTS/` to `work/archive/YYYY-MM-DD-$ARGUMENTS/`.

---

# Process

## Step 1 — Locate the feature

**If `$ARGUMENTS` was not provided**, list features in `work/review/` and ask the user to select one.

**If `$ARGUMENTS` was provided**, check in order:

1. `work/review/$ARGUMENTS/` → proceed (normal path)
2. `work/active/$ARGUMENTS/` → ⚠️ warn: "Has not passed `/game:review` yet." Ask: "Archive anyway?" Options: "Archive with warning" / "Cancel — run review first"
3. `work/planning/$ARGUMENTS/` → stop. Not implemented yet.
4. `work/archive/` for `*-$ARGUMENTS` → stop. Already archived — show path.
5. Not found → stop.

---

## Step 2 — Read artifacts

1. `{source}/$ARGUMENTS/task.md` — acceptance criteria
2. `{source}/$ARGUMENTS/plan.md` — planned steps
3. `{source}/$ARGUMENTS/progress.md` — step statuses

---

## Step 3 — Evaluate completion

### Steps check
Parse progress.md steps table:
- `✅ done` → complete
- Anything else → incomplete

### Criteria check
From task.md — any `- [ ]` item → unsatisfied

### Annotations check
```
Grep pattern: <!-- (CHANGE|REMOVE|ADD)
```

### Completion report

| Check | Result |
|-------|--------|
| Implementation steps | X / Y complete |
| Acceptance criteria | X / Y satisfied |
| Plan annotations | None / N unresolved |

---

## Step 4 — Confirm with user

**All complete:**
> "All checks passed. Ready to archive `$ARGUMENTS`?"
> Options: "Archive now" / "Cancel"

**Incomplete items:**
> "The following items are not complete: [list]. Archive anyway?"
> Options: "Archive with warnings" / "Cancel — I'll finish first"

Never auto-archive. Always require explicit confirmation.

---

## Step 5 — Perform the archive

```bash
mv {source}/$ARGUMENTS work/archive/YYYY-MM-DD-$ARGUMENTS
```

Verify: source gone, destination exists.

---

## Step 6 — Update progress.md

```markdown
## Archive

- **Archived on:** YYYY-MM-DD
- **Archive path:** work/archive/YYYY-MM-DD-{feature}/
- **Reviewed:** ✅ via /game:review / ⚠️ skipped by user
- **Status:** All complete / Archived with warnings: [list]
```

---

## Step 7 — Display summary

```
## Archive Complete ✅

Feature: $ARGUMENTS
Archived to: work/archive/YYYY-MM-DD-$ARGUMENTS/

Completion:
- Steps: X / Y ✅
- Criteria: X / Y ✅
- Annotations: None
```

---

# Guardrails

| Rule | Detail |
|------|--------|
| Never auto-archive | Always require explicit user confirmation |
| Never delete | Use `mv` only — never `rm` |
| Verify the move | Confirm source gone, destination exists |
| Date prefix mandatory | Archive folder must include YYYY-MM-DD- prefix |
