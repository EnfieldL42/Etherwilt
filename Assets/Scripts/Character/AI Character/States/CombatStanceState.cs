using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I/States/Combat State")]
public class CombatStanceState : AIState
{
    [Header("Attacks")]
    public List<AICharacterAttackAction> aiCharacterAttacks; //a list of all possible attacks
    public List<AICharacterAttackAction> potentialAttacks; //all attacks possible in this situation (based on angle, distance, etc)
    private AICharacterAttackAction choosenAttack;
    private AICharacterAttackAction previousAttack;
    protected bool hasAttack = false;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false; //if can perform combo attack, after the initial attack
    [SerializeField] protected int percentageToPerformCombo = 25; //chance in percentage to perform a combo on next attack
    public bool onlyPerformComboIfInitialAttackHits = true;
    protected bool hasRolledForComboChance = false; //if we have already rolled for the chance during this state

    [Header("Engagement Distance")]
    [SerializeField] public float maximumEngagementDistance = 10; //distance we have to be away from the target before we enter the pursue state

    [Header("Strafing")]
    [SerializeField] bool canStrafe = false; //if the character can strafe around the target
    private bool hasChoosenStrafePath = false; //if we have already chosen a strafe path during this state
    [SerializeField] float strafeMoveAmount = 0;
    //private float maxStrafeTimer = 5f; //maximum time to strafe before we stop strafing
    //private float strafeTimer = 0f; //timer for strafing

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if(aiCharacter.isPerformingAction)
        {
            return this;
        }

        if(!aiCharacter.navmeshAgent.enabled)
        {
            aiCharacter.navmeshAgent.enabled = true;
        }

        if (aiCharacter.pursueState.canPivot)
        {
            if (!aiCharacter.aICharacterNetworkManager.isMoving.Value)
            {
                if (aiCharacter.aICharacterCombatManager.viewableAngle < -30 || aiCharacter.aICharacterCombatManager.viewableAngle > 30)
                {
                    aiCharacter.aICharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }
        }

        //USE THIS IF YOU WANT THE CHARACTER TO TURN WHEN OUTSIDE FOV INSTEAD OF ROTATE TOWARDS YOU

        aiCharacter.aICharacterCombatManager.RotateTowardsAgent(aiCharacter);

        //if out target is no longer present, switch back to idle state
        if (aiCharacter.aICharacterCombatManager.currentTarget == null)
        {
            return SwitchState(aiCharacter, aiCharacter.idle);
        }

        if (canStrafe)
        {
            SetStrafePath(aiCharacter);
        }

        if(canPerformCombo && !hasRolledForComboChance)
        {
            hasRolledForComboChance = true;
            aiCharacter.attack.willPerformCombo = RollForOutcomeChance(percentageToPerformCombo);
        }

        if (!hasAttack)
        {
            GetNewAttack(aiCharacter);
        }
        else
        {
            aiCharacter.attack.currentAttack = choosenAttack;
            //roll for combo chance
            return SwitchState(aiCharacter, aiCharacter.attack);
        }

        //uf we are outside of the combat engagement distance, swutch to pursue state
        if(aiCharacter.aICharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
        {
            return SwitchState(aiCharacter, aiCharacter.pursueState);
        }


        NavMeshPath path = new();
        aiCharacter.navmeshAgent.CalculatePath(aiCharacter.aICharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navmeshAgent.SetPath(path);

        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter)
    {
        potentialAttacks = new List<AICharacterAttackAction>();

        foreach (var potentialAttack in aiCharacterAttacks)
        {
            //if we are too close, continue
            if(potentialAttack.minimumAttackDistance > aiCharacter.aICharacterCombatManager.distanceFromTarget)
            {
                continue;
            }

            //if we are too far, continue
            if (potentialAttack.maximumAttackDistance < aiCharacter.aICharacterCombatManager.distanceFromTarget)
            {
                continue;
            }

            //if target is outside min fov, continue
            if (potentialAttack.minimumAttackAngle > aiCharacter.aICharacterCombatManager.viewableAngle)
            {
                continue;
            }

            //if target is outside max fov, continue
            if (potentialAttack.maximumAttackAngle < aiCharacter.aICharacterCombatManager.viewableAngle)
            {
                continue;
            }

            potentialAttacks.Add(potentialAttack);
        }

        if(potentialAttacks.Count <= 0)
        {
            Debug.Log("no attakcs");
            return;
        }

        var totalWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            totalWeight += attack.attackWeight;
        }

        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processedWeight = 0;

        foreach(var attack in potentialAttacks)
        {
            processedWeight += attack.attackWeight;

            if(randomWeightValue <= processedWeight)
            {
                choosenAttack = attack;
                previousAttack = choosenAttack;
                hasAttack = true;
                return;
            }
        }

        //4. pick on of the remaining attacks randomly, based on weight
        //5. select this attack and pass it to the attack state
    }

    protected virtual bool RollForOutcomeChance(int outcomeChance)
    {
        bool outcomeWillBePerformed = false;

        int randomPercentage = Random.Range(0, 100);

        if(randomPercentage > outcomeChance)
        {
            outcomeWillBePerformed = true;
        }

        return outcomeWillBePerformed;
    }

    protected virtual void SetStrafePath(AICharacterManager aICharacter)
    {
        int layersToCheck = WorldUtilityManager.instance.GetEnviroLayers() | WorldUtilityManager.instance.GetCharacterLayers();

        Collider[] colliders = Physics.OverlapSphere(aICharacter.aICharacterCombatManager.lockOnTransform.position, aICharacter.characterController.radius + 0.25f, layersToCheck);

        foreach (var collider in colliders)
        {
            if (collider.transform.root == aICharacter.transform.root)
            {
                continue;
            }

            aICharacter.characterAnimatorManager.SetAnimatorMovementParameters(0, Mathf.Abs(strafeMoveAmount));
            return;
        }

        ////checks if there is a wall close to the character
        //if (Physics.CheckSphere(aICharacter.aICharacterCombatManager.lockOnTransform.position, aICharacter.characterController.radius + 0.25f, WorldUtilityManager.instance.GetEnviroLayers()))
        //{
        //    //stop strafing, instead path towards the enemy
        //    aICharacter.characterAnimatorManager.SetAnimatorMovementParameters(0, Mathf.Abs(strafeMoveAmount));
        //    return;
        //}

        ////strafe
        aICharacter.characterAnimatorManager.SetAnimatorMovementParameters(strafeMoveAmount, 0);

        if (hasChoosenStrafePath)
        {
            return;
        }

        //strafe left or right?
        hasChoosenStrafePath = true;

        int leftOrRightIndex = Random.Range(0, 100);

        if (leftOrRightIndex >= 50)
        {
            //strafe left
            strafeMoveAmount = -0.5f;
        }
        else
        {
            //strafe right
            strafeMoveAmount = 0.5f;
        }
    }

    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasRolledForComboChance = false;
        hasAttack = false;
        //hasChoosenStrafePath = false;
        //strafeTimer = 0f;
        //strafeMoveAmount = 0;
    }

}
