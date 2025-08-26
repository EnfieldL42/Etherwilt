using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.TextCore.Text;

public class AIEarthGuardianCharacterManager : AIBossCharacterManager
{
    [HideInInspector] public AIEarthGuardianSoundFXManager earthGuardianSoundFXManager;
    [HideInInspector] public AIEarthGuardianTailCombatManager tailCombatManager;
    [HideInInspector] public AIEarthGuardianBodyCombatManager bodyCombatManager;

    [Header("Burrowing Attack")]
    [SerializeField] CombatStanceState burrowedCombatStanceState;
    public AICharacterAttackAction burrowAttack;
    public bool forceBurrowAttack = false;

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

    }

    public override void PhaseShift()
    {
        if (canPhaseShift == false)
        {
            return;
        }

        canPhaseShift = false;
        animator.SetBool("isBurrowed", true);
        characterAnimatorManager.PlayTargetActionAnimation(phaseShiftAnimation, true);
        combatState = Instantiate(burrowedCombatStanceState);
        currentState = pursueState;

        PhaseShift();
    }

    public void ShiftPhaseAfterBurrowAttack()
    {
        animator.SetBool("isBurrowed", false);
        combatState = Instantiate(phase02CombatStanceState);
        currentState = combatState;
    }

    public void ForceBurrowAttack()
    {
        attack.currentAttack = burrowAttack;
        currentState = attack;
    }

}
