using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossData))]
public class BossDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Lấy đối tượng BossData
        BossData data = (BossData)target;

        // Hiển thị trường bossPrefab
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bossPrefab"), new GUIContent("Boss Prefab"));

        // Lấy danh sách phases
        SerializedProperty phasesProp = serializedObject.FindProperty("phases");

        // Hiển thị tiêu đề cho danh sách phases
        EditorGUILayout.LabelField("Phases", EditorStyles.boldLabel);

        // Hiển thị nút thêm/xóa phần tử trong danh sách
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Phase"))
        {
            phasesProp.arraySize++;
        }
        if (GUILayout.Button("Remove Last Phase") && phasesProp.arraySize > 0)
        {
            phasesProp.arraySize--;
        }
        EditorGUILayout.EndHorizontal();

        // Tùy chỉnh hiển thị cho mỗi phase
        for (int i = 0; i < phasesProp.arraySize; i++)
        {
            SerializedProperty phase = phasesProp.GetArrayElementAtIndex(i);
            EditorGUILayout.LabelField($"Phase {i + 1}", EditorStyles.boldLabel);

            // Hiển thị các trường không liên quan đến FireZone
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("triggerAtPercent"), new GUIContent("Trigger At Percent"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("moveSpeed"), new GUIContent("Move Speed"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("fireRate"), new GUIContent("Fire Rate"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("bulletPrefab"), new GUIContent("Bullet Prefab"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("meleeRange"), new GUIContent("Melee Range"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("meleeDamage"), new GUIContent("Melee Damage"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("fireZonePrefab"), new GUIContent("Fire Zone Prefab"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("iceZonePrefab"), new GUIContent("Ice Zone Prefab"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("poisonZonePrefab"), new GUIContent("Poison Zone Prefab"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("summonZonePrefab"), new GUIContent("Summon Zone Prefab"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("patterns"), new GUIContent("Fire Patterns"));
            EditorGUILayout.PropertyField(phase.FindPropertyRelative("specialSkills"), new GUIContent("Special Skills"));

            // Chỉ hiển thị các trường FireZone nếu fireZonePrefab được gán
            if (phase.FindPropertyRelative("fireZonePrefab").objectReferenceValue != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Fire Zone Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("fireZoneDamage"), new GUIContent("Fire Zone Damage"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("fireZoneDuration"), new GUIContent("Fire Zone Duration"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("fireZoneRadius"), new GUIContent("Fire Zone Radius"));
            }
            if (phase.FindPropertyRelative("iceZonePrefab").objectReferenceValue != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Ice Zone Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("iceZoneDuration"), new GUIContent("Ice Zone Duration"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("iceZoneRadius"), new GUIContent("Ice Zone Radius"));
            }
            if (phase.FindPropertyRelative("poisonZonePrefab").objectReferenceValue != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Poison Zone Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("poisonZoneDamage"), new GUIContent("Poison Zone Dame"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("poisonZoneDuration"), new GUIContent("Poison Zone Duration"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("poisonZoneRadius"), new GUIContent("Poison Zone Radius"));
            }
            if (phase.FindPropertyRelative("summonZonePrefab").objectReferenceValue != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Summon Zone Settings", EditorStyles.boldLabel);
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("minionPrefab"), new GUIContent("Minion Monster Prefab"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("minionCount"), new GUIContent("Minion Count"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("summonZoneDuration"), new GUIContent("Summon Zone Duration"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("summonZoneRadius"), new GUIContent("Summon Zone Darius"));

            }

            EditorGUILayout.Space();
        }

        // Áp dụng các thay đổi vào Inspector
        serializedObject.ApplyModifiedProperties();
    }
}