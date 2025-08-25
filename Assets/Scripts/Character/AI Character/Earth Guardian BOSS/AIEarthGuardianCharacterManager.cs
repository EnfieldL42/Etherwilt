using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AIEarthGuardianCharacterManager : AIBossCharacterManager
{
    [HideInInspector] public AIEarthGuardianSoundFXManager earthGuardianSoundFXManager;
    [HideInInspector] public AIEarthGuardianTailCombatManager tailCombatManager;
    [HideInInspector] public AIEarthGuardianBodyCombatManager bodyCombatManager;

    [Header("Burrowing State")]
    [SerializeField] CombatStanceState burrowedCombatStanceState;

    protected override void Awake()
    {
        base.Awake();
        earthGuardianSoundFXManager = GetComponent<AIEarthGuardianSoundFXManager>();
        tailCombatManager = GetComponent<AIEarthGuardianTailCombatManager>();
        bodyCombatManager = GetComponent<AIEarthGuardianBodyCombatManager>();
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

    public void ForceSecondBodyAttack()
    {
        //currentState = attackState;
    }

}
