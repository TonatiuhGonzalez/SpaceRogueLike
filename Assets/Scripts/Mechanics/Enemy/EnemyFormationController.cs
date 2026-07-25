using System.Collections.Generic;
using UnityEngine;

public class EnemyFormationController : MonoBehaviour
{
    private readonly List<EnemyMovement> _members = new();

    public void Register(EnemyMovement member)
    {
        if (member == null || _members.Contains(member)) return;
        _members.Add(member);
        Redistribute();
    }

    public void Unregister(EnemyMovement member)
    {
        if (member == null) return;
        if (_members.Remove(member))
            Redistribute();
    }

    private void Redistribute()
    {
        int count = _members.Count;
        if (count == 0) return;

        float angleStep = 360f / count;
        for (int i = 0; i < count; i++)
            _members[i].FormationAngle = angleStep * i;
    }
}
