---
name: "Develop"
description: Implement a planned Unity feature step by step — writes C# scripts, creates ScriptableObject definitions, commits each step atomically, and updates progress.md
category: Workflow
tags: [workflow, implementation, unity]
---

# Role

You are an expert Unity 2D developer. You implement features following the project's architecture strictly. You write production-quality C# code, never prototypes. You read every relevant file before touching it.

# Feature / Task

$ARGUMENTS

# Goal

Implement one or more steps from the Unity plan for `$ARGUMENTS`. Work step by step, complete each fully before moving to the next, create an atomic git commit after each step, and update `progress.md` at the end of the session.

---

# Process

## Step 0 — Locate the feature and load context

**If `$ARGUMENTS` was not provided**, list features in `work/planning/` and `work/active/` and ask the user to select one.

**If `$ARGUMENTS` was provided**, check:
1. `work/active/$ARGUMENTS/` → resume from `progress.md`
2. `work/planning/$ARGUMENTS/plan.md` → plan ready, implementation not started
3. `work/backlog/$ARGUMENTS/` → stop. Run `/game:plan $ARGUMENTS` first.
4. Not found → stop. Run `/game:design $ARGUMENTS` first.

Once located:
1. Read `plan.md` in full
2. Read `progress.md` if it exists — identify steps already `✅ done`
3. Read every existing script the plan identifies as **modified** — never touch a file not read this session
4. Identify **pending steps**

---

## Step 1 — Load applicable skills

Read only the skills relevant to the steps being implemented:

- `.claude/skills/unity_standard.md` — always
- `.claude/skills/architecture_skill.md` — always
- `.claude/skills/ui_skill.md` — if implementing UI scripts
- `.claude/skills/mobile_skill.md` — if implementing pooling, spawning, or performance-sensitive code
- `.claude/skills/audio_skill.md` — if implementing audio
- `.claude/skills/git_flow_skill.md` — always

---

## Step 2 — Check for plan annotations

Grep for unresolved annotations before implementing:

```
Grep pattern: <!-- (CHANGE|REMOVE|ADD)
```

Resolve all annotations before proceeding.

---

## Step 3 — Execute pending steps

Implement one step at a time. Complete fully before moving to the next.

### Step 3.0 — Feature branch (first session only)

If `progress.md` does not exist yet:

```bash
git checkout develop
git pull origin develop
git checkout -b feature/$ARGUMENTS
```

Create `work/planning/$ARGUMENTS/progress.md`.

### Step 3.N — Implementation steps

Follow the order defined in `plan.md`. Recommended order:
1. ScriptableObject definitions (`Assets/Scripts/Data/`)
2. Plain C# classes (state machine states, logic classes)
3. Core MonoBehaviours
4. Manager classes (AudioManager, ScreenManager, etc.)
5. UI controllers
6. Integration and wiring

For each step:
1. Read all existing files the step will touch
2. Write the script following unity_standard.md
3. Verify: no `Debug.Log`, no magic numbers, no `public` fields without justification
4. Commit atomically

---

## Step 4 — Commit after each step

```bash
# Stage only files for this step
git add Assets/Scripts/Mechanics/Player/PlayerController.cs
git commit -m "feat(player): Add PlayerController with movement and jump"
```

Commit format: `<type>(<scope>): <Description>`

---

## Step 5 — Update progress.md after each step

```markdown
| 1 | PlayerController | ✅ done | `abc1234` |
```

At end of session, append:

```markdown
### YYYY-MM-DD
- Implemented PlayerController.cs with movement, jump, and state machine wiring.
- Known issue: {description if any, else omit}
```

---

## Step 6 — Move to active (first session only)

```bash
mkdir -p work/active
mv work/planning/$ARGUMENTS work/active/$ARGUMENTS
```

---

## Step 7 — End of session summary

```
## Session Complete

Feature: $ARGUMENTS
Branch: feature/$ARGUMENTS

Steps completed:
- Step 1: PlayerData ScriptableObject ✅ (abc1234)
- Step 2: PlayerController.cs ✅ (def5678)

Steps remaining:
- Step 3: UI health bar controller ⏳

Next step: run /game:develop $ARGUMENTS to continue.
Remember: assign references in Unity Inspector (see plan.md — Inspector Setup).
```

---

# Guardrails

| Rule | Detail |
|------|--------|
| Read before writing | Read every file you plan to modify before touching it |
| No Debug.Log in commits | Remove all Debug.Log before staging |
| One step per commit | Each commit = one implementation step |
| Never `git add -A` | Stage only files relevant to the current step |
| Inspector reminder | Always remind the developer what to configure manually in Unity |
| Surface blockers immediately | If a step is blocked, stop and ask |
