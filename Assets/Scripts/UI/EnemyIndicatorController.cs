using System.Collections.Generic;
using UnityEngine;

public class EnemyIndicatorController : MonoBehaviour
{
    private const float EDGE_PADDING = 20f;

    [SerializeField] private RectTransform _indicatorPrefab;
    [SerializeField] private int _maxIndicators = 8;
    [SerializeField] private LevelManager _levelManager;

    private Camera _gameCamera;
    private RectTransform _canvasRect;
    private readonly List<RectTransform> _activeIndicators = new();
    private readonly Queue<RectTransform> _pool = new();

    private void Awake()
    {
        _gameCamera = Camera.main;
        _canvasRect = GetComponent<RectTransform>();

        for (int i = 0; i < _maxIndicators; i++)
        {
            RectTransform rt = Instantiate(_indicatorPrefab, transform);
            rt.gameObject.SetActive(false);
            _pool.Enqueue(rt);
        }
    }

    private void LateUpdate()
    {
        ReturnAll();

        if (_levelManager == null) return;

        IReadOnlyList<EnemyHealth> tracked = _levelManager.TrackedEnemies;
        int shown = 0;

        for (int i = 0; i < tracked.Count && shown < _maxIndicators; i++)
        {
            EnemyHealth enemy = tracked[i];
            if (enemy == null || !enemy.gameObject.activeSelf) continue;

            Vector3 viewportPos = _gameCamera.WorldToViewportPoint(enemy.transform.position);

            bool offScreen = viewportPos.z < 0f
                || viewportPos.x < 0f || viewportPos.x > 1f
                || viewportPos.y < 0f || viewportPos.y > 1f;

            if (!offScreen) continue;

            RectTransform indicator = GetFromPool();
            if (indicator == null) break;

            PositionIndicator(indicator, viewportPos);
            shown++;
        }
    }

    private void PositionIndicator(RectTransform indicator, Vector3 viewportPos)
    {
        Vector2 canvasSize = _canvasRect.sizeDelta;
        Vector2 halfSize = canvasSize * 0.5f;

        // Convert viewport direction from center (flip z-behind enemies)
        Vector2 dir = viewportPos.z < 0f
            ? -(new Vector2(viewportPos.x, viewportPos.y) - new Vector2(0.5f, 0.5f))
            : new Vector2(viewportPos.x, viewportPos.y) - new Vector2(0.5f, 0.5f);

        if (dir == Vector2.zero) dir = Vector2.up;

        // Find edge intersection
        float slope = dir.y / (dir.x != 0f ? dir.x : 0.0001f);
        float edgeX = halfSize.x - EDGE_PADDING;
        float edgeY = halfSize.y - EDGE_PADDING;

        Vector2 pos;
        if (Mathf.Abs(slope) * edgeX < edgeY)
        {
            float x = Mathf.Sign(dir.x) * edgeX;
            pos = new Vector2(x, x * slope);
        }
        else
        {
            float y = Mathf.Sign(dir.y) * edgeY;
            pos = new Vector2(y / slope, y);
        }

        indicator.anchoredPosition = pos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        indicator.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private RectTransform GetFromPool()
    {
        if (_pool.Count == 0) return null;
        RectTransform rt = _pool.Dequeue();
        rt.gameObject.SetActive(true);
        _activeIndicators.Add(rt);
        return rt;
    }

    private void ReturnAll()
    {
        for (int i = 0; i < _activeIndicators.Count; i++)
        {
            _activeIndicators[i].gameObject.SetActive(false);
            _pool.Enqueue(_activeIndicators[i]);
        }
        _activeIndicators.Clear();
    }
}
