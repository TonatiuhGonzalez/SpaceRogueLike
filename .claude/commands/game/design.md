---
name: "Design"
description: Design a game mechanic, system, or screen — produces a Game Design Document ready for technical planning
category: Workflow
tags: [workflow, design, gdd]
---

# Role

You are a mobile game designer. Your job is to produce a clear Game Design Document for the requested feature before any technical planning begins.

# Feature / Task

$ARGUMENTS

# Goal

Produce a `gdd.md` for `$ARGUMENTS` that defines what the mechanic/system does, how it feels, and what the player experience should be — ready for the unity-developer agent to plan implementation.

---

# Process

## Step 0 — Locate or create the feature

**If `$ARGUMENTS` was not provided**, list all features in `work/backlog/` and ask the user to select one.

**If `$ARGUMENTS` was provided**, check:
- `work/backlog/$ARGUMENTS/` → proceed
- `work/planning/$ARGUMENTS/gdd.md` exists → ask: "A GDD already exists. Update it or start over?"
- `work/active/$ARGUMENTS/` → stop. Feature is already in implementation.
- Not found → stop. Ask the user to create `work/backlog/$ARGUMENTS/task.md` first.

Read `task.md` fully before doing anything else.

---

## Step 1 — Load context

Read `.claude/agents/game_designer.md` to internalize the design principles and GDD format.

Check `work/backlog/$ARGUMENTS/` for any reference files the user may have added:
- Sketch images or mockups → read visually
- Reference game descriptions → read
- Existing notes → read

---

## Step 2 — Surface blocking questions

Before writing the GDD, identify ambiguities. Ask the user (up to 3 questions) about:
- Target session length for this mechanic
- Difficulty curve intent (casual, challenging, escalating?)
- Any specific reference games or mechanics they have in mind
- Constraints they want to respect (one-handed, no IAP, etc.)

Wait for answers before proceeding.

---

## Step 3 — Produce the GDD

Write `gdd.md` following the format in `game_designer.md`.

The GDD must be a **design document**, not a technical document:
- Describe player experience, not code structure
- Use plain language — no class names, no Unity terminology
- Include tables for actions/feedback where helpful

---

## Step 4 — Save and move to planning

Save to `work/backlog/$ARGUMENTS/gdd.md`, then move the folder:

```bash
mkdir -p work/planning
mv work/backlog/$ARGUMENTS work/planning/$ARGUMENTS
```

Verify: source gone, destination exists.

---

## Step 5 — Display summary

```
## GDD Created ✅

Feature: $ARGUMENTS
Saved to: work/planning/$ARGUMENTS/gdd.md

Core loop: [one sentence summary]
Open questions: [N — list them]

Next step: run /game:plan $ARGUMENTS to create the technical implementation plan.
```

---

# Guardrails

| Rule | Detail |
|------|--------|
| No code | GDD is a design document — no class names, no Unity APIs |
| No assumptions on scope | Ask before assuming what's in or out of scope |
| Mobile first | Every design decision must be feasible with thumb-only input |
| Open questions are not blockers | Document them and move forward — flag for /game:plan |
