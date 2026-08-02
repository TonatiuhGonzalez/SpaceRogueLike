using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private LevelManager _levelManager;
    [SerializeField] private RunData _runData;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private MinimapData _minimapData;

    [Header("Radar")]
    [SerializeField] private RectTransform _blipContainer;
    [SerializeField] private RectTransform _blipPrefab;
    [SerializeField] private RectTransform _playerMarker;
    [SerializeField] private float _radarInteriorRadius = 150f;
    [SerializeField] private int _maxVisibleBlips = 12;

    [Header("HUD Text")]
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _enemyCountText;

    private readonly List<RectTransform> _activeBlips = new();
    private readonly Queue<RectTransform> _blipPool = new();

    private void Awake()
    {
        for (int i = 0; i < _maxVisibleBlips; i++)
        {
            RectTransform blip = Instantiate(_blipPrefab, _blipContainer);
            blip.gameObject.SetActive(false);
            _blipPool.Enqueue(blip);
        }
    }

    private void OnEnable()
    {
        _levelManager.OnEnemyCountChanged += UpdateProgressText;
    }

    private void OnDisable()
    {
        _levelManager.OnEnemyCountChanged -= UpdateProgressText;
    }

    private void LateUpdate()
    {
        ReturnAllBlips();
        RefreshBlips();
        RotatePlayerMarker();
    }

    private void RefreshBlips()
    {
        IReadOnlyList<EnemyHealth> tracked = _levelManager.TrackedEnemies;
        float detectionRadius = _minimapData.DetectionRadius;
        float scale = _radarInteriorRadius / detectionRadius;

        for (int i = 0; i < tracked.Count && _activeBlips.Count < _maxVisibleBlips; i++)
        {
            EnemyHealth enemy = tracked[i];
            if (enemy == null || !enemy.gameObject.activeSelf) continue;

            Vector2 offset = (Vector2)enemy.transform.position - (Vector2)_playerTransform.position;
            if (offset.magnitude > detectionRadius) continue;

            RectTransform blip = GetBlipFromPool();
            if (blip == null) break;

            blip.anchoredPosition = offset * scale;
        }
    }

    private void RotatePlayerMarker()
    {
        _playerMarker.localRotation = Quaternion.Euler(0f, 0f, _playerTransform.eulerAngles.z);
    }

    private RectTransform GetBlipFromPool()
    {
        if (_blipPool.Count == 0) return null;
        RectTransform blip = _blipPool.Dequeue();
        blip.gameObject.SetActive(true);
        _activeBlips.Add(blip);
        return blip;
    }

    private void ReturnAllBlips()
    {
        for (int i = 0; i < _activeBlips.Count; i++)
        {
            _activeBlips[i].gameObject.SetActive(false);
            _blipPool.Enqueue(_activeBlips[i]);
        }
        _activeBlips.Clear();
    }

    private void UpdateProgressText(int enemiesRemaining)
    {
        _enemyCountText.text = enemiesRemaining.ToString();
        _levelText.text = _runData.CurrentLevel.ToString();
    }
}
