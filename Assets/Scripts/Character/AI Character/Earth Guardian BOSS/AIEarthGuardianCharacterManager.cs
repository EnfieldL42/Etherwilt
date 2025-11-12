using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;
using UnityEngine.TextCore.Text;

public class AIEarthGuardianCharacterManager : AIBossCharacterManager
{
    [HideInInspector] public AIEarthGuardianSoundFXManager earthGuardianSoundFXManager;
    [HideInInspector] public AIEarthGuardianTailCombatManager tailCombatManager;
    [HideInInspector] public AIEarthGuardianBodyCombatManager bodyCombatManager;

    [Header("Burrowing Attack")]
    [SerializeField] DoNothingState doNothingState;
    [SerializeField] CombatStanceState burrowedCombatStanceState;
    [SerializeField] AICharacterAttackAction burrowAttack;
    [HideInInspector] public bool canUnburrowAttack = false;
    private bool isBurrowed = false;
    public bool forceBurrowAttack = false;

    [SerializeField] float burrowMaxTime = 5f;

    [Header("Navmesh Agent")]
    public float navmeshAgentStoppingDistance = 0;

    protected override void Awake()
    {
        base.Awake();
        earthGuardianSoundFXManager = GetComponent<AIEarthGuardianSoundFXManager>();
        tailCombatManager = GetComponent<AIEarthGuardianTailCombatManager>();
        bodyCombatManager = GetComponent<AIEarthGuardianBodyCombatManager>();
    }


    override protected void Update()
    {
        base.Update();

        if (forceBurrowAttack)
        {
            forceBurrowAttack = false;
            ForceBurrowAttack();
        }

        if(!isBurrowed)
        {
            navmeshAgent.stoppingDistance = navmeshAgentStoppingDistance;
        }
        else
        {
            navmeshAgent.stoppingDistance = combatState.maximumEngagementDistance;
        }

        if (canUnburrowAttack)
        {
            BurrowTimer();
        }

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            burrowedCombatStanceState = Instantiate(burrowedCombatStanceState);
            doNothingState = Instantiate(doNothingState);
        }
    }

    public override void PhaseShift()
    {
        if (canPhaseShift == false)
        {
            return;
        }
        canPhaseShift = false;

        currentState = doNothingState;
        isBurrowed = true;
        animator.SetBool("isBurrowed", isBurrowed);
        characterAnimatorManager.PlayTargetActionAnimationInstantly(phaseShiftAnimation, true);
        StartCoroutine(WaitThenChangeState(6f));
    }

    private IEnumerator WaitThenChangeState(float time)
    {
        yield return new WaitForSeconds(time);
        canUnburrowAttack = true;
        combatState = burrowedCombatStanceState;
        currentState = combatState;
    }

    public void TurnOffIsBurrowed()
    {
        isBurrowed = false;
        animator.SetBool("isBurrowed", isBurrowed);
        canUnburrowAttack = false;
    }

    public void ShiftPhaseAfterBurrowAttack()
    {
        combatState = Instantiate(phase02CombatStanceState);
        //currentState = combatState;
    }

    public void ForceBurrowAttack()
    {
        if (canUnburrowAttack)
        {
            canUnburrowAttack = false;
            attack.hasPerformedAttack = false;
            attack.currentAttack = burrowAttack;
            currentState = attack;
        }

    }

    private void BurrowTimer()
    {
        float timer = 0f;

        if (timer < burrowMaxTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            timer = 0f;
            ForceBurrowAttack();
        }
    }

}
