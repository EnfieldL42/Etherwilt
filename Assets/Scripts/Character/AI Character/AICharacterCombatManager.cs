using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{

    [Header("Target Information")]
    public float distanceFromTarget;
    public float viewableAngle;
    public Vector3 targetsDirection;

    [Header("Detection")]
    [SerializeField] float detectionRadius = 15;
    public float minimumFOV = -35;
    public float maximumFOV = 35;
    public void FindTargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if (currentTarget != null)
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
                        PivotTowardsTarget(aiCharacter);
                    } 
                     
                }
            }

        }

    }

    public void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        //play a pivot animation depending on viwable angle of target
        if(aiCharacter == null)
        {
            return;
        }

        //if(viewableAngle >= 20 && viewableAngle <= 60)
        //{
        //    aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_45", true);
        //}
        //else if(viewableAngle <= -20 && viewableAngle >= -60)
        //{
        //    aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_45", true);
        //}
        //else if (viewableAngle >= 61 && viewableAngle <= 110)
        //{
        //    aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_90", true);
        //}
        //else if (viewableAngle <= -61 && viewableAngle >= -110)
        //{
        //    aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_90", true);
        //}
        //else if (viewableAngle >= 111 && viewableAngle <= 145)
        //{
        //    aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_135", true);
        //}
        //else if (viewableAngle <= -111 && viewableAngle >= -145)
        //{
        //    aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_135", true);
        //}
        //else if (viewableAngle >= 146 && viewableAngle <= 180)
        //{
        //    aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Right_180", true);
        //}
        //else if (viewableAngle <= -146 && viewableAngle >= -180)
        //{
        //    aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Turn_Left_180", true);
        //}

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

}
