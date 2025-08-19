using UnityEngine;

[CreateAssetMenu(menuName = "A.I/States/Attack")]
public class AttackState : AIState
{
    [Header("Current Attack")]
    [HideInInspector] public AICharacterAttackAction currentAttack;
    [HideInInspector] public bool willPerformCombo = true;

    [Header("State Flags")]
    protected bool hasPerformedAttack = false;
    protected bool hasPerformedCombo = false;

    [Header("Pivot After Attack")]
    [SerializeField] protected bool pivotAfterAttack = false;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aICharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idle);
        }

        if(aiCharacter.aICharacterCombatManager.currentTarget.isDead.Value)
        {
            return SwitchState(aiCharacter, aiCharacter.idle);
        }


        aiCharacter.aICharacterCombatManager.RotateTowardsTargetWhilstAttacking(aiCharacter);

        aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);

        PerformComboAttack(aiCharacter);

        if (aiCharacter.isPerformingAction)
        {
            return this;
        }


        if (!hasPerformedAttack)
        {
            if(aiCharacter.aICharacterCombatManager.actionRecoveryTimer > 0)
            {
                return this;
            }

            PerformAttack(aiCharacter);
            
            return this;
        }

        if(aiCharacter.pursueState.canPivot)
        {
            if (pivotAfterAttack)
            {
                aiCharacter.aICharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }


        return SwitchState(aiCharacter, aiCharacter.combatState);
    }

    protected void PerformAttack(AICharacterManager aiCharacter)
    {
        hasPerformedAttack = true;
        currentAttack.AttemptToPerformAction(aiCharacter);
        aiCharacter.aICharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;

    }


    protected virtual void PerformComboAttack(AICharacterManager aiCharacter)
    {
        bool canPerformCombo = false;

        if (!willPerformCombo)
        {
            return;
        }

        if (hasPerformedCombo)
        {
            return;
        }

        if(currentAttack.comboAction == null)
        {
            return;
        }

        //if we dont need to hit enemy before, perform combo attack
        if (aiCharacter.aICharacterCombatManager.canPerformCombo && !aiCharacter.combatState.onlyPerformComboIfInitialAttackHits)
        {
            canPerformCombo = true;
        }

        //if we need to hit enemy before, check if we have hit the target during the initial attack
        if (aiCharacter.aICharacterCombatManager.canPerformCombo && aiCharacter.combatState.onlyPerformComboIfInitialAttackHits && aiCharacter.aICharacterCombatManager.hasHitTargetDuringCombo)
        {
            canPerformCombo = true;
        }

        if (canPerformCombo)
        {
            hasPerformedCombo = true;
            currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
        }

    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasPerformedAttack = false;
        hasPerformedCombo = false;
        willPerformCombo = false;
    }

}
