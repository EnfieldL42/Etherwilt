using Unity.Netcode;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int vertical;
    int horizontal;
    public string lastDamageAnimationPlayer;

    [Header("Damage Animations")]
    //ping hit
    public string forward_Ping_Damage = "forward_Ping_Damage";
    public string backward_Ping_Damage = "backward_Ping_Damage";
    public string left_Ping_Damage = "left_Ping_Damage";
    public string right_Ping_Damage = "right_Ping_Damage";

    //medium hit
    public string hit_Forward_Medium_01 = "hit_Forward_Medium_01";
    public string hit_Backward_Medium_01 = "hit_Backward_Medium_01";
    public string hit_Left_Medium_01 = "hit_Left_Medium_01";
    public string hit_Right_Medium_01 = "hit_Right_Medium_01";




    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    protected virtual void Update()
    {

    }

    public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappedHorizontalAmount;
        float snappedVerticalAmount;

        if(horizontalMovement > 0 && horizontalMovement <= 0.5f)
        {
            snappedHorizontalAmount = 0.5f;
        }
        else if(horizontalMovement > 0.5f && horizontalMovement <= 1)
        {
            snappedHorizontalAmount = 1;
        }
        else if(horizontalMovement < 0 && horizontalMovement >= -0.5f)
        {
            snappedHorizontalAmount = -0.5f;
        }
        else if(horizontalMovement < -0.5f && horizontalMovement >= -1)
        {
            snappedHorizontalAmount = -1;
        }
        else
        {
            snappedHorizontalAmount = 0;
        }

        if (verticalMovement > 0 && verticalMovement <= 0.5f)
        {
            snappedVerticalAmount = 0.5f;
        }
        else if (verticalMovement > 0.5f && verticalMovement <= 1)
        {
            snappedVerticalAmount = 1;
        }
        else if (verticalMovement < 0 && verticalMovement >= -0.5f)
        {
            snappedVerticalAmount = -0.5f;
        }
        else if (verticalMovement < -0.5f && verticalMovement >= -1)
        {
            snappedVerticalAmount = -1;
        }
        else
        {
            snappedVerticalAmount = 0;
        }

        if (isSprinting)
        {
            snappedVerticalAmount = 2;
        }


        character.animator.SetFloat(horizontal, snappedHorizontalAmount, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, snappedVerticalAmount, 0.1f, Time.deltaTime);
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false, bool canRun = true, bool canRoll = false)
    {
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;//can be used to stop characters from attempting a new action, flags will turn true if you are stunned
        character.canRotate = canRotate;
        character.canMove = canMove;
        character.canRun = canRun;
        character.canRoll = canRoll;

        character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }
    
    public virtual void PlayTargetActionAnimationInstantly(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false, bool canRun = true, bool canRoll = false)
    {
        character.applyRootMotion = applyRootMotion;
        character.animator.Play(targetAnimation);
        character.isPerformingAction = isPerformingAction;//can be used to stop characters from attempting a new action, flags will turn true if you are stunned
        character.canRotate = canRotate;
        character.canMove = canMove;
        character.canRun = canRun;
        character.canRoll= canRoll;

        character.characterNetworkManager.NotifyTheServerOfInstantActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

    public virtual void PlayTargetAttackActionAnimation(WeaponItem weapon, AttackType attackType, string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false, bool canRoll = false)
    {

        //keep track of last attack performed (for combos)
        //keep track of current attack type(light or heavy)
        //update animation set to current weapons animation
        //decide if our attack can be parried
        //tell the network we are in an "isattacking" flag (for counter damage)

        character.characterCombatManager.currentAttackType = attackType;
        character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
        UpdateAnimatorController(weapon.weaponAnimator);
        character.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;
        character.canRotate = canRotate;
        character.canMove = canMove;
        character.canRoll = canRoll;


        character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }
    
    public void UpdateAnimatorController(AnimatorOverrideController weaponController)
    {
        character.animator.runtimeAnimatorController = weaponController;
    }


}
