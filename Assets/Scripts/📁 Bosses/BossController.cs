using UnityEngine;
using System.Collections.Generic;
using Spine.Unity;

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
        if (health.IsDead() || player == null || currentState == BossState.Dead) return;

        stateTimer -= Time.deltaTime;
        specialSkillTimer -= Time.deltaTime;
        phaseManager.CheckPhaseChange(health.CurrentPercent);

        switch (currentState)
        {
            case BossState.Idle:
                bossAnimation.PlayAnimation("Idle", true);
                if (stateTimer <= 0)
                    ChangeState(BossState.MoveToPlayer, Random.Range(2f, 3f));
                break;

            case BossState.MoveToPlayer:
                movement.MoveToPlayer(player);
                bossAnimation.PlayAnimation("Walk", true);
                if (stateTimer <= 0)
                    ChooseAttackState();
                break;

            case BossState.AttackMelee:
                attack.PerformMeleeAttack(player);
                bossAnimation.PlayAnimation("Attack", true);
                if (stateTimer <= 0)
                    ChangeState(BossState.MoveToPlayer, Random.Range(2f, 3f));
                break;

            case BossState.AttackRanged:
                attack.PerformRangedAttack(player);
                bossAnimation.PlayAnimation("Attack", true);
                if (stateTimer <= 0)
                    ChangeState(BossState.MoveToPlayer, Random.Range(2f, 3f));
                break;

            case BossState.SpecialSkill:
                attack.PerformSpecialSkill(player);
                bossAnimation.PlayAnimation("Dead", false);
                if (stateTimer <= 0)
                    ChangeState(BossState.MoveToPlayer, Random.Range(2f, 3f));
                break;

            case BossState.Dead:
                bossAnimation.PlayAnimation("Dead", false);
                break;
        }
    }

    void ChangeState(BossState newState, float duration)
    {
        currentState = newState;
        stateTimer = duration;
    }

   void ChooseAttackState()
    {
        // Danh sách các trạng thái khả dụng
        List<BossState> availableStates = new List<BossState>();

        // Kiểm tra các trạng thái khả dụng
        if (attack != null && attack.CanPerformMeleeAttack(player))
        {
            availableStates.Add(BossState.AttackMelee);
            Debug.Log("MeleeAttack is available");
        }
        availableStates.Add(BossState.AttackRanged); // Tấn công tầm xa luôn khả dụng
        if (attack != null && attack.HasSpecialSkill() && specialSkillTimer <= 0)
        {
            availableStates.Add(BossState.SpecialSkill);
            Debug.Log("SpecialSkill is available");
        }

        // Nếu không có trạng thái nào khả dụng, chuyển về Idle
        if (availableStates.Count == 0)
        {
            Debug.Log("No states available, switching to Idle");
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