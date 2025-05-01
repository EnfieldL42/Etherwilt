using UnityEngine;
using Unity.Netcode;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int vertical;
    int horizontal;

    [Header("Damage Animations")]
    [HideInInspector] public string hit_Forward_Medium_01 = "hit_Forward_Medium_01";
    [HideInInspector] public string hit_Backward_Medium_01 = "hit_Backward_Medium_01";
    [HideInInspector] public string hit_Left_Medium_01 = "hit_Left_Medium_01";
    [HideInInspector] public string hit_Right_Medium_01 = "hit_Right_Medium_01";


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }


    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalValue, bool isSprinting)
    {
        float horizontalAmount = horizontalMovement;
        float verticalAmount = verticalValue;

        if (isSprinting)
        {
            verticalAmount = 2;
        }

        character.animator.SetFloat(horizontal, horizontalAmount, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, verticalAmount, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;//can be used to stop characters from attempting a new action, flags will turn true if you are stunned
        character.canRotate = canRotate;
        character.canMove = canMove;


        character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

    public virtual void PlayTargetAttackActionAnimation(AttackType attackType, string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {

        //keep track of last attack performed (for combos)
        //keep track of current attack type(light or heavy)
        //update animation set to current weapons animation
        //decide if our attack can be parried
        //tell the network we are in an "isattacking" flag (for counter damage)

        Debug.Log("PLAYING ANIMATION: " + targetAnimation);
        character.characterCombatManager.currentAttackType = attackType;
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;


        character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

}
