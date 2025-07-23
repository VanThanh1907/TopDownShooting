using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;
using Unity.VisualScripting;

public enum BossState
{
    Idle,
    MoveToPlayer,
    AttackMelee,
    AttackRanged,
    SpecialSkill,
    Dead
}

public class BossController : MonoBehaviour
{
    [SerializeField] private BossData data;
    private Transform player;
    private Health health;
    private BossMovement movement;
    private BossAttack attack;
    private BossPhaseManager phaseManager;
    private BossAnimation bossAnimation;

    private BossState currentState = BossState.Idle;
    private float stateTimer;
    private float specialSkillTimer;
    private bool hasPerformedSpecialSkill = false;


    public void Setup(BossData bossData)
    {
        data = bossData;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        health = GetComponent<Health>();
        movement = GetComponent<BossMovement>();
        attack = GetComponent<BossAttack>();
        phaseManager = GetComponent<BossPhaseManager>();
        bossAnimation = GetComponent<BossAnimation>();

        phaseManager.Setup(data);
        specialSkillTimer = 0f;
        ChangeState(BossState.Idle, Random.Range(1f, 2f));
    }

    void Update()
    {
        if (health.IsDead() || player == null) return;

        stateTimer -= Time.deltaTime;
        specialSkillTimer -= Time.deltaTime;
        phaseManager.CheckPhaseChange(health.CurrentPercent);

        switch (currentState)
        {
            case BossState.Idle:
            case BossState.MoveToPlayer:
                bossAnimation.PlayAnimation("Walk", true);
                movement.MoveToPlayer(player);
                if (stateTimer <= 0)
                    ChooseAttackState();
                break;

            case BossState.AttackMelee:
                attack.PerformMeleeAttack(player);
                if (stateTimer <= 0)
                    ChangeState(BossState.MoveToPlayer, Random.Range(0.1f, 0.3f));
                break;

            case BossState.AttackRanged:
                bossAnimation.PlayAnimation("Attack", true);
                attack.PerformRangedAttack(player);
                if (stateTimer <= 0)
                    ChangeState(BossState.MoveToPlayer, Random.Range(1f, 3f));
                break;

            case BossState.SpecialSkill:
            
                
                if (!hasPerformedSpecialSkill)
                {
                    attack.PerformSpecialSkill(player);
                    hasPerformedSpecialSkill = true;
                    Debug.Log($"Performed Special Skill at {Time.time}");
                }
                if (stateTimer <= 0)
                {
                    ChangeState(BossState.MoveToPlayer, 0.1f); 
                }
                break;
        }
    }

    void ChangeState(BossState newState, float duration)
    {
        currentState = newState;
        stateTimer = duration;
        if (newState != BossState.SpecialSkill)
        {
            hasPerformedSpecialSkill = false; // Đặt lại cờ khi rời trạng thái SpecialSkill
        }
    }

    void ChooseAttackState()
    {
        // Danh sách các trạng thái khả dụng
        BossPhaseData currentPhase = phaseManager.GetCurrentPhase();
        List<BossState> availableStates = new List<BossState>();

        // Kiểm tra các trạng thái khả dụng
        if (currentPhase.meleeRange > 0 && attack != null && attack.CanPerformMeleeAttack(player))
        {
            availableStates.Add(BossState.AttackMelee);
            Debug.Log("MeleeAttack is available");
        }
        else
        {
            // Add ranged attack as a fallback
            availableStates.Add(BossState.AttackRanged);
            Debug.Log("RangedAttack is available");
        }

        // Check for special skill availability
        if (attack != null && attack.HasSpecialSkill() && specialSkillTimer <= 0)
        {
            availableStates.Add(BossState.SpecialSkill);
            Debug.Log("SpecialSkill is available");
        }

        // Nếu không có trạng thái nào khả dụng, chuyển về Idle
        if (availableStates.Count == 0)
        {
            Debug.LogWarning("No states available, switching to Idle");
            ChangeState(BossState.Idle, Random.Range(1f, 2f));
            return;
        }

        // Chọn ngẫu nhiên một trạng thái từ danh sách khả dụng
        BossState selectedState = availableStates[Random.Range(0, availableStates.Count)];
        float duration = selectedState == BossState.SpecialSkill ? Random.Range(2f, 4f) : Random.Range(1.5f, 3f);
        Debug.Log($"Selected state: {selectedState}, duration: {duration}");

        // Chuyển sang trạng thái được chọn
        ChangeState(selectedState, duration);

        // Reset timer kỹ năng đặc biệt nếu được chọn
        if (selectedState == BossState.SpecialSkill)
        {
            specialSkillTimer = attack.specialSkillCooldown;
        }
    }
   
}