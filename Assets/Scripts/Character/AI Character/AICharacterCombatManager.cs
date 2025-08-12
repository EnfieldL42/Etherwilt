using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AICharacterCombatManager : CharacterCombatManager
{
    protected AICharacterManager aiCharacter;

    [Header("Action Recovery")]
    public float actionRecoveryTimer = 0;

    [Header("Target Information")]
    public float distanceFromTarget;
    public float viewableAngle;
    public Vector3 targetsDirection;

    [Header("Detection")]
    [SerializeField] float detectionRadius = 15;
    public float minimumFOV = -35;
    public float maximumFOV = 35;

    [Header("Attack Rotation Speed")]
    public float attackRotationSpeed = 25;


    [Header("Stance")]
    public float maxStance = 150;
    public float currentStance;
    [SerializeField] float stanceRegeneratedPerSecond = 15;
    [SerializeField] bool ignoreStanceBreak = false;

    [Header("Stance Timer")]
    [SerializeField] float stanceRegenerationTimer = 0;
    private float stanceTickTimer = 0;
    [SerializeField] float defaultTimeUntilStanceRegenerationBegins = 15;


    public HashSet<CharacterManager> damagedCharactersThisAttack = new HashSet<CharacterManager>();
     
    protected override void Awake()
    {  
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();      
        lockOnTransform = GetComponentInChildren<LockOnTrasform>().transform;
    }

    private void FixedUpdate()
    {
        HandleStanceBreak();
    }

    public void AwardEtherOnDeath(PlayerManager player)
    {
        if (player.characterGroup == CharacterGroup.Team02)
        {
            return;
        }

        player.playerStatsManager.AddEther(aiCharacter.characterStatsManager.etherDroppedOnDeath);
    }

    private void HandleStanceBreak()
    {
        if(!aiCharacter.IsOwner)
        {
            return;
        }
        if(aiCharacter.isDead.Value)
        {
            return;
        }

        if(stanceRegenerationTimer > 0)
        {
            stanceRegenerationTimer -= Time.deltaTime;
        }
        else
        {
            stanceRegenerationTimer = 0;

            if(currentStance < maxStance)
            {
                //begin adding stance each tick
                stanceTickTimer += Time.deltaTime;

                if(stanceTickTimer >= 1)
                {
                    stanceTickTimer = 0;
                    currentStance += stanceRegeneratedPerSecond;
                }
            }
            else
            {
                currentStance = maxStance;
            }
        }

        if(currentStance <= 0)
        {
            DamageIntensity previousDamageIntensity = WorldUtilityManager.instance.GetDamageIntensityBasedOnPoiseDamage(previousPoiseDamageTaken);

            if(previousDamageIntensity == DamageIntensity.Colossal)
            {
                currentStance = 1;
                return;
            }

            //check if being backstabbed or ripposted-do not play the stance break animation

            currentStance = maxStance;

            if(ignoreStanceBreak)
            {
                return;
            }

            aiCharacter.characterAnimatorManager.PlayTargetActionAnimationInstantly("Stance_Break_01", true);
        }
    }

    public void DamageStance(int stanceDamage)
    {
        stanceRegenerationTimer = defaultTimeUntilStanceRegenerationBegins;

        currentStance -= stanceDamage;
    }

    public void FindTargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if (aiCharacter.aICharacterCombatManager.currentTarget != null)
        {
            return;
        }

        if(aiCharacter.isPerformingAction)
        {
            return;
        }

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());

        for (int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

            if (targetCharacter == null) 
            {
                continue;
            }

            if(targetCharacter == aiCharacter)
            {
                continue;
            }

            if (targetCharacter.isDead.Value)
            {
                continue;
            }

            //can I attack this character? is it friendly?
            if(WorldUtilityManager.instance.CanIDamageThisTarget(aiCharacter.characterGroup, targetCharacter.characterGroup))
            {
                //if potential target is found, it has to be infront of it
                Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float angleOfPotentialTarget = Vector3.Angle(targetDirection, aiCharacter.transform.forward);



                if (angleOfPotentialTarget > minimumFOV && angleOfPotentialTarget < maximumFOV)
                {

                    if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position, WorldUtilityManager.instance.GetEnviroLayers()))
                    {
                        Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position, Color.red);
                    }
                    else
                    {
                        targetsDirection = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtilityManager.instance.GetAngleOfTarget(transform, targetsDirection);
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                        if(aiCharacter.pursueState.canPivot)
                        {
                            PivotTowardsTarget(aiCharacter);

                        }
                    } 
                     
                }
            }

        }

    }

    public virtual void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        //play a pivot animation depending on viwable angle of target
        if (aiCharacter.isPerformingAction)
        {
            return;
        }

        if (viewableAngle >= 20 && viewableAngle <= 60)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_45", true);
        }
        else if (viewableAngle <= -20 && viewableAngle >= -60)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_45", true);
        }
        else if (viewableAngle >= 61 && viewableAngle <= 110)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
        }
        else if (viewableAngle <= -61 && viewableAngle >= -110)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
        }
        else if (viewableAngle >= 111 && viewableAngle <= 145)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_135", true);
        }
        else if (viewableAngle <= -111 && viewableAngle >= -145)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_135", true);
        }
        else if (viewableAngle >= 146 && viewableAngle <= 180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
        }
        else if (viewableAngle <= -146 && viewableAngle >= -180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
        }

        if (viewableAngle >= 20 && viewableAngle <= 110)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
        }
        else if (viewableAngle <= -20 && viewableAngle >= -110)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
        }
        else if (viewableAngle >= 111 && viewableAngle <= 180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
        }
        else if (viewableAngle <= -111 && viewableAngle >= -180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
        }

    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if(aiCharacter.aICharacterNetworkManager.isMoving.Value)
        {
            aiCharacter.transform.rotation = aiCharacter.navmeshAgent.transform.rotation;
        }
    }

    public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
    {
        if (currentTarget == null)
        {
            return;
        }

        if(!aiCharacter.canRotate)
        {
            return;
        }

        if(!aiCharacter.isPerformingAction)
        {
            return;
        }

        Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
        targetDirection.y = 0;
        targetDirection.Normalize();

        if(targetDirection == Vector3.zero)
        {
            targetDirection = aiCharacter.transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, attackRotationSpeed * Time.deltaTime);
    }

    public void HandleActionRecovery(AICharacterManager aiCharacter)
    {
        if (actionRecoveryTimer > 0)
        {
            if (!aiCharacter.isPerformingAction)
            {
                actionRecoveryTimer -= Time.deltaTime;
            }
        }
    }

    public void ResetAttackHashSet()
    {
        damagedCharactersThisAttack.Clear();
        // Enable all relevant colliders here
    }
}
