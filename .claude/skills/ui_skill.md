---
description: UI standard for this Unity 2D mobile project — uGUI setup, Canvas configuration, TextMeshPro usage, safe areas, multiple resolutions, and screen management patterns.
globs: ["Assets/Scripts/UI/**/*.cs"]
alwaysApply: false
---

# UI Skill — uGUI + TextMeshPro

## Canvas Setup (mandatory)

Every Canvas must be configured as follows:

| Property | Value | Reason |
|----------|-------|--------|
| Render Mode | Screen Space - Overlay | Standard for 2D mobile UI |
| Canvas Scaler → UI Scale Mode | Scale With Screen Size | Adapts to all resolutions |
| Reference Resolution | 1080 × 1920 | Portrait mobile baseline |
| Screen Match Mode | Match Width Or Height |  |
| Match | 0.5 | Balances width and height scaling |

---

## TextMeshPro Rules

- **Never** use the legacy `Text` component — always `TextMeshProUGUI`
- Always assign a font asset from `Assets/Fonts/`
- Never hardcode font size for dynamic text — use Auto Size with min/max bounds

```csharp
// ✅ Reference via SerializeField
[SerializeField] private TextMeshProUGUI _scoreText;
[SerializeField] private TextMeshProUGUI _timerText;

// ✅ Update text
_scoreText.text = score.ToString("N0");
_timerText.text = $"{minutes:00}:{seconds:00}";

// ❌ Never legacy Text
[SerializeField] private Text _scoreText; // Wrong
```

---

## Safe Area Handling

All full-screen panels must respect device safe areas (notches, home indicators).

```csharp
public class SafeAreaPanel : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        Vector2 anchorMin = safeArea.position / screenSize;
        Vector2 anchorMax = (safeArea.position + safeArea.size) / screenSize;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}
```

Add `SafeAreaPanel` to the root RectTransform of every full-screen panel.

---

## Screen Manager Pattern

Centralized screen navigation — no screen references each other directly.

```csharp
public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    [SerializeField] private GameObject _mainMenuScreen;
    [SerializeField] private GameObject _gameplayScreen;
    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private GameObject _gameOverScreen;

    private GameObject _currentScreen;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowScreen(GameScreen screen)
    {
        _currentScreen?.SetActive(false);

        _currentScreen = screen switch
        {
            GameScreen.MainMenu  => _mainMenuScreen,
            GameScreen.Gameplay  => _gameplayScreen,
            GameScreen.Pause     => _pauseScreen,
            GameScreen.GameOver  => _gameOverScreen,
            _ => null
        };

        _currentScreen?.SetActive(true);
    }
}

public enum GameScreen { MainMenu, Gameplay, Pause, GameOver }
```

---

## UI Controller Pattern

One controller script per screen. Thin — only handles UI events and updates display.

```csharp
public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _settingsButton;

    [Header("Display")]
    [SerializeField] private TextMeshProUGUI _highScoreText;

    private void OnEnable()
    {
        _playButton.onClick.AddListener(OnPlayPressed);
        _settingsButton.onClick.AddListener(OnSettingsPressed);
        UpdateHighScore();
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(OnPlayPressed);
        _settingsButton.onClick.RemoveListener(OnSettingsPressed);
    }

    private void OnPlayPressed()
    {
        ScreenManager.Instance.ShowScreen(GameScreen.Gameplay);
        GameManager.Instance.SetState(GameState.Playing);
    }

    private void OnSettingsPressed()
    {
        ScreenManager.Instance.ShowScreen(GameScreen.Settings);
    }

    private void UpdateHighScore()
    {
        _highScoreText.text = $"Best: {PlayerPrefs.GetInt("HighScore", 0):N0}";
    }
}
```

---

## Anchor Rules

- Full screen panels: anchor to all four corners (stretch)
- Top elements (score, lives): anchor to top-center or top-left/right
- Bottom elements (buttons, joystick): anchor to bottom-center or corners
- Never use absolute pixel positions — always anchors relative to parent

---

## Button Feedback

All buttons must have visual feedback on press:

```csharp
// Add to any button for scale punch feedback
public class ButtonFeedback : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float _punchScale = 0.9f;
    [SerializeField] private float _duration = 0.1f;

    private Vector3 _originalScale;

    private void Awake() => _originalScale = transform.localScale;

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(_originalScale * _punchScale));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleTo(_originalScale));
    }

    private IEnumerator ScaleTo(Vector3 target)
    {
        float elapsed = 0f;
        Vector3 start = transform.localScale;
        while (elapsed < _duration)
        {
            transform.localScale = Vector3.Lerp(start, target, elapsed / _duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = target;
    }
}
```

---

## Rules

- TextMeshPro only — never legacy Text
- SafeAreaPanel on every full-screen panel root
- Add/remove button listeners in OnEnable/OnDisable — never in Start
- UI controllers do not contain game logic — they call GameManager or service classes
- Canvas Scaler always configured (Scale With Screen Size, 1080×1920)
- No hardcoded positions or pixel sizes
