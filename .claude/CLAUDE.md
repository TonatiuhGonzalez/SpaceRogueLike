# CLAUDE.md

## Project

Unity 2D mobile game — single developer, targeting Android and iOS.

```
Assets/
├── Scripts/
│   ├── Core/          ← GameManager, SceneLoader, ServiceLocator
│   ├── Mechanics/     ← game-specific systems and mechanics
│   ├── UI/            ← UI controllers and screen managers
│   ├── Data/          ← ScriptableObject class definitions
│   └── Utils/         ← extensions, helpers, constants
├── Prefabs/
│   ├── UI/
│   ├── Gameplay/
│   └── Effects/
├── Scenes/
├── ScriptableObjects/ ← actual SO instances (not class definitions)
├── Sprites/
├── Audio/
└── Fonts/
```

## Stack

| Tech | Role |
|------|------|
| Unity (LTS) | Engine |
| C# | Language |
| uGUI | UI layout and interaction |
| TextMeshPro | All text rendering |
| ScriptableObjects | Game data and configuration |
| Unity Audio | Sound system |
| Git + LFS | Version control |

## Agents

| Agent | When to invoke |
|-------|---------------|
| `game-designer` | Design a mechanic, system, or game flow — produces GDD |
| `unity-developer` | Plan technical implementation of a designed feature |

- Invoke the correct agent before planning or reviewing.
- Agents **NEVER implement** — only plan.
- Plans saved in `work/planning/{feature}/plan.md`.

## Commands

```
/game:design    → design a mechanic or system (GDD)
/game:plan      → technical plan from a GDD
/game:develop   → implement step by step from plan
/game:review    → code quality and standards review
/game:archive   → archive completed feature
```

## Non-negotiable rules

**Architecture:**
- MonoBehaviours are thin — Unity lifecycle only, no business logic inline
- No magic numbers — use ScriptableObjects or constants
- No `FindObjectOfType` or `GameObject.Find` in Update loops
- Cache `Camera.main` — never call it in Update
- Subscribe to events in OnEnable, unsubscribe in OnDisable
- Use `[SerializeField]` instead of `public` for Inspector fields

**UI:**
- TextMeshPro for all text — never legacy Text component
- Canvas Scaler: Scale With Screen Size, reference 1080×1920
- Handle safe areas (notches) on all full-screen panels
- No hardcoded positions — use anchors and layout groups

**Mobile:**
- Object pooling for anything spawned frequently (bullets, enemies, particles)
- Sprite atlases for UI and gameplay sprites
- Compress audio: AAC for music, PCM for short SFX
- No heavy allocations in Update — use coroutines or events

**C# conventions:**
- PascalCase: classes, methods, properties, public fields
- `_camelCase`: private fields
- `UPPER_SNAKE_CASE`: constants
- No `public` fields except when required by Unity serialization

**Git:**
- Git LFS for: `*.png *.jpg *.wav *.mp3 *.unity *.prefab *.asset *.fbx`
- Never commit `Library/`, `Temp/`, `Logs/`, `obj/`

## Skills index

| Skill | When to load |
|-------|-------------|
| `unity_standard` | Always |
| `architecture_skill` | Any new script or system |
| `ui_skill` | Any UI work (uGUI, TextMeshPro) |
| `mobile_skill` | Performance, optimization, mobile build |
| `audio_skill` | Any audio system or sound work |
| `git_flow_skill` | Git operations |
