using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBossData", menuName = "ScriptableObjects/BossData")]
public class BossData : ScriptableObject
{
    public string bossName;
    public GameObject bossPrefab;
    public List<BossPhaseData> phases;
}


