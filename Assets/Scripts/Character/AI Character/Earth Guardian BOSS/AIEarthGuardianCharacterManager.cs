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
    public CombatStanceState burrowedCombatStanceState;
    public AICharacterAttackAction burrowAttack;
    public bool isBurrowed = false;
    public bool forceBurrowAttack = false;

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
        StartCoroutine(WaitThenChangeState(10f));
    }

    private IEnumerator WaitThenChangeState(float time)
    {
        yield return new WaitForSeconds(time);
        combatState = burrowedCombatStanceState;
        currentState = combatState;
    }

    public void TurnOffIsBurrowed()
    {
        isBurrowed = false;
        animator.SetBool("isBurrowed", isBurrowed);
    }

    public void ShiftPhaseAfterBurrowAttack()
    {
        combatState = Instantiate(phase02CombatStanceState);
        //currentState = combatState;
    }

    public void ForceBurrowAttack()
    {
        attack.hasPerformedAttack = false;
        attack.currentAttack = burrowAttack;
        currentState = attack;
    }



}
