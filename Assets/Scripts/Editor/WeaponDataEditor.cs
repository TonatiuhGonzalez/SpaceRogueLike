using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    private SerializedProperty _weaponName;
    private SerializedProperty _icon;
    private SerializedProperty _weaponType;

    private SerializedProperty _damage;
    private SerializedProperty _fireRate;
    private SerializedProperty _range;
    private SerializedProperty _projectileSpeed;

    private SerializedProperty _coneAngleDegrees;

    private SerializedProperty _bulletCount;
    private SerializedProperty _fanSpreadAngle;

    private SerializedProperty _explosionRadius;
    private SerializedProperty _explosionDamagePercent;

    private SerializedProperty _chainCount;
    private SerializedProperty _chainDamagePercents;
    private SerializedProperty _chainDelay;

    private SerializedProperty _isVampiric;
    private SerializedProperty _vampiricHealPercent;

    private SerializedProperty _poisonTickPercent;

    private SerializedProperty _projectilePrefab;
    private SerializedProperty _fireSFX;

    private SerializedProperty _projectileSprite;
    private SerializedProperty _projectileScale;

    private void OnEnable()
    {
        _weaponName = serializedObject.FindProperty(nameof(WeaponData.WeaponName));
        _icon = serializedObject.FindProperty(nameof(WeaponData.Icon));
        _weaponType = serializedObject.FindProperty(nameof(WeaponData.WeaponType));

        _damage = serializedObject.FindProperty(nameof(WeaponData.Damage));
        _fireRate = serializedObject.FindProperty(nameof(WeaponData.FireRate));
        _range = serializedObject.FindProperty(nameof(WeaponData.Range));
        _projectileSpeed = serializedObject.FindProperty(nameof(WeaponData.ProjectileSpeed));

        _coneAngleDegrees = serializedObject.FindProperty(nameof(WeaponData.ConeAngleDegrees));

        _bulletCount = serializedObject.FindProperty(nameof(WeaponData.BulletCount));
        _fanSpreadAngle = serializedObject.FindProperty(nameof(WeaponData.FanSpreadAngle));

        _explosionRadius = serializedObject.FindProperty(nameof(WeaponData.ExplosionRadius));
        _explosionDamagePercent = serializedObject.FindProperty(nameof(WeaponData.ExplosionDamagePercent));

        _chainCount = serializedObject.FindProperty(nameof(WeaponData.ChainCount));
        _chainDamagePercents = serializedObject.FindProperty(nameof(WeaponData.ChainDamagePercents));
        _chainDelay = serializedObject.FindProperty(nameof(WeaponData.ChainDelay));

        _isVampiric = serializedObject.FindProperty(nameof(WeaponData.IsVampiric));
        _vampiricHealPercent = serializedObject.FindProperty(nameof(WeaponData.VampiricHealPercent));

        _poisonTickPercent = serializedObject.FindProperty(nameof(WeaponData.PoisonTickPercent));

        _projectilePrefab = serializedObject.FindProperty(nameof(WeaponData.ProjectilePrefab));
        _fireSFX = serializedObject.FindProperty(nameof(WeaponData.FireSFX));

        _projectileSprite = serializedObject.FindProperty(nameof(WeaponData.ProjectileSprite));
        // Private field — FindProperty needs the literal name, nameof() can't reach a private member.
        _projectileScale = serializedObject.FindProperty("_projectileScale");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_weaponName);
        EditorGUILayout.PropertyField(_icon);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Type", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_weaponType);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_damage);
        EditorGUILayout.PropertyField(_fireRate);
        EditorGUILayout.PropertyField(_range);
        EditorGUILayout.PropertyField(_projectileSpeed);

        DrawTypeSpecificFields((WeaponType)_weaponType.enumValueIndex);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("References", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_projectilePrefab);
        EditorGUILayout.PropertyField(_fireSFX);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Visual", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(_projectileSprite);
        EditorGUILayout.PropertyField(_projectileScale);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTypeSpecificFields(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Dispersion:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Dispersion", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_coneAngleDegrees);
                break;

            case WeaponType.Fan:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Fan", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_bulletCount);
                EditorGUILayout.PropertyField(_fanSpreadAngle);
                break;

            case WeaponType.Area:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Area", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_explosionRadius);
                EditorGUILayout.PropertyField(_explosionDamagePercent);
                break;

            case WeaponType.Zapper:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Zapper", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_chainCount);
                EditorGUILayout.PropertyField(_chainDamagePercents, true);
                EditorGUILayout.PropertyField(_chainDelay);
                break;

            case WeaponType.Poison:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Poison", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_poisonTickPercent);
                break;

            case WeaponType.Vampiric:
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Vampiric", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(_isVampiric);
                if (_isVampiric.boolValue)
                    EditorGUILayout.PropertyField(_vampiricHealPercent);
                break;
        }
    }
}
