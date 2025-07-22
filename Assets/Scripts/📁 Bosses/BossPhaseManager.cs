using UnityEngine;

public class BossPhaseManager : MonoBehaviour
{
    private BossData data;
    private BossAttack attack;
    private BossMovement movement;
    private int currentPhaseIndex;
    private BossPhaseData currentPhase;

    public void Setup(BossData bossData)
    {
        data = bossData;
        attack = GetComponent<BossAttack>();
        movement = GetComponent<BossMovement>();
        SwitchToPhase(0);
    }

    public void CheckPhaseChange(float healthPercent)
    {
        for (int i = data.phases.Count - 1; i >= 0; i--)
        {
            if (healthPercent <= data.phases[i].triggerAtPercent && i > currentPhaseIndex)
            {
                SwitchToPhase(i);
                break;
            }
        }
    }

    private void SwitchToPhase(int index)
    {
        currentPhaseIndex = index;
        currentPhase = data.phases[index];
        
        // Cập nhật thông số cho các thành phần
        movement.Setup(currentPhase.moveSpeed);
        attack.Setup(currentPhase, GetComponent<BossAttack>().firePoint);
    }

    public BossPhaseData GetCurrentPhase()
    {
        return currentPhase;
    }
}
