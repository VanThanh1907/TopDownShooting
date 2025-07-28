using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BossData))]
public class BossDataEditor : Editor
{
    // Mảng để theo dõi trạng thái foldout của mỗi phase
    private bool[] phaseFoldouts;

    public override void OnInspectorGUI()
    {
        // Lấy đối tượng BossData
        BossData data = (BossData)target;

        // Hiển thị trường bossPrefab
        EditorGUILayout.PropertyField(serializedObject.FindProperty("bossPrefab"), new GUIContent("Boss Prefab"));

        // Lấy danh sách phases
        SerializedProperty phasesProp = serializedObject.FindProperty("phases");

        // Khởi tạo mảng foldouts nếu cần
        if (phaseFoldouts == null || phaseFoldouts.Length != phasesProp.arraySize)
        {
            phaseFoldouts = new bool[phasesProp.arraySize];
            for (int i = 0; i < phaseFoldouts.Length; i++)
            {
                phaseFoldouts[i] = true; // Mở tất cả foldout mặc định
            }
        }

        // Hiển thị tiêu đề và nút thêm/xóa phase
        EditorGUILayout.LabelField("Phases", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Phase"))
        {
            phasesProp.arraySize++;
            // Cập nhật mảng foldouts
            System.Array.Resize(ref phaseFoldouts, phasesProp.arraySize);
            phaseFoldouts[phasesProp.arraySize - 1] = true;
        }
        if (GUILayout.Button("Remove Last Phase") && phasesProp.arraySize > 0)
        {
            phasesProp.arraySize--;
            System.Array.Resize(ref phaseFoldouts, phasesProp.arraySize);
        }
        EditorGUILayout.EndHorizontal();

        // Tùy chỉnh hiển thị cho mỗi phase
        for (int i = 0; i < phasesProp.arraySize; i++)
        {
            SerializedProperty phase = phasesProp.GetArrayElementAtIndex(i);

            // Hiển thị foldout cho phase
            phaseFoldouts[i] = EditorGUILayout.Foldout(phaseFoldouts[i], $"Phase {i + 1}", true, EditorStyles.foldoutHeader);
            if (phaseFoldouts[i])
            {
                // Thêm khoảng cách và đường kẻ ngang
                EditorGUILayout.Space();
                EditorGUILayout.Separator();

                // Bắt đầu khu vực dọc với padding
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.Space(5f);

                // Hiển thị các trường không phụ thuộc vào specialSkills
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("triggerAtPercent"), new GUIContent("Trigger At Percent"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("moveSpeed"), new GUIContent("Move Speed"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("fireRate"), new GUIContent("Fire Rate"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("bulletPrefab"), new GUIContent("Bullet Prefab"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("meleeRange"), new GUIContent("Melee Range"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("meleeDamage"), new GUIContent("Melee Damage"));
                EditorGUILayout.PropertyField(phase.FindPropertyRelative("patterns"), new GUIContent("Fire Patterns"));

                // Hiển thị specialSkills
                SerializedProperty specialSkillsProp = phase.FindPropertyRelative("specialSkills");
                EditorGUILayout.PropertyField(specialSkillsProp, new GUIContent("Special Skills"));

                // Kiểm tra các kỹ năng được chọn
                bool hasFireZone = false;
                bool hasIceZone = false;
                bool hasPoisonZone = false;
                bool hasSummon = false;
                bool hasTeleport = false;

                for (int j = 0; j < specialSkillsProp.arraySize; j++)
                {
                    int skillValue = specialSkillsProp.GetArrayElementAtIndex(j).enumValueIndex;
                    switch ((BossPhaseData.SpecialSkill)skillValue)
                    {
                        case BossPhaseData.SpecialSkill.FireZone:
                            hasFireZone = true;
                            break;
                        case BossPhaseData.SpecialSkill.IceZone:
                            hasIceZone = true;
                            break;
                        case BossPhaseData.SpecialSkill.PoisonZone:
                            hasPoisonZone = true;
                            break;
                        case BossPhaseData.SpecialSkill.Summon:
                            hasSummon = true;
                            break;
                        case BossPhaseData.SpecialSkill.Teleport:
                            hasTeleport = true;
                            break;
                    }
                }

                // Hiển thị prefab nếu kỹ năng được chọn
                if (hasTeleport)
                {
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("teleportEffectPrefab"), new GUIContent("Teleport Effect Prefab", "Prefab for teleport effect"));
                }
                if (hasFireZone)
                {
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("fireZonePrefab"), new GUIContent("Fire Zone Prefab", "Prefab for fire zone skill"));
                }
                if (hasIceZone)
                {
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("iceZonePrefab"), new GUIContent("Ice Zone Prefab", "Prefab for ice zone skill"));
                }
                if (hasPoisonZone)
                {
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("poisonZonePrefab"), new GUIContent("Poison Zone Prefab", "Prefab for poison zone skill"));
                }
                if (hasSummon)
                {
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("summonZonePrefab"), new GUIContent("Summon Zone Prefab", "Prefab for summon zone skill"));
                }

                // Hiển thị các trường cấu hình bổ sung nếu prefab được gán
                if (hasFireZone && phase.FindPropertyRelative("fireZonePrefab").objectReferenceValue != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Fire Zone Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("fireZoneDamage"), new GUIContent("Fire Zone Damage"));
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("fireZoneDuration"), new GUIContent("Fire Zone Duration"));
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("fireZoneRadius"), new GUIContent("Fire Zone Radius"));
                }
                if (hasIceZone && phase.FindPropertyRelative("iceZonePrefab").objectReferenceValue != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Ice Zone Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("iceZoneDuration"), new GUIContent("Ice Zone Duration"));
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("iceZoneRadius"), new GUIContent("Ice Zone Radius"));
                }
                if (hasPoisonZone && phase.FindPropertyRelative("poisonZonePrefab").objectReferenceValue != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Poison Zone Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("poisonZoneDamage"), new GUIContent("Poison Zone Damage"));
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("poisonZoneDuration"), new GUIContent("Poison Zone Duration"));
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("poisonZoneRadius"), new GUIContent("Poison Zone Radius"));
                }
                if (hasSummon && phase.FindPropertyRelative("summonZonePrefab").objectReferenceValue != null)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Summon Zone Settings", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("minionPrefab"), new GUIContent("Minion Monster Prefab"));
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("minionCount"), new GUIContent("Minion Count"));
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("summonZoneDuration"), new GUIContent("Summon Zone Duration"));
                    EditorGUILayout.PropertyField(phase.FindPropertyRelative("summonZoneRadius"), new GUIContent("Summon Zone Radius"));
                }
                
                EditorGUILayout.Separator();
                EditorGUILayout.Space(10f);
                EditorGUILayout.EndVertical();
            }
                
        }

        // Áp dụng các thay đổi vào Inspector
        serializedObject.ApplyModifiedProperties();
    }
}