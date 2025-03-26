using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class CharacterManager : NetworkBehaviour
{
    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;

    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool isGrounded = true;
    public bool isJumping = false;
    public bool applyRootMotion = false;
    public bool canRotate = true;
    public bool canMove = true;




    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();//
        characterNetworkManager = GetComponent<CharacterNetworkManager>();//
        animator = GetComponent<Animator>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
    }

    protected virtual void Update()
    {
        animator.SetBool("isGrounded", isGrounded);

        if(IsOwner) //if character is being controlled from host side(player), then assign its network position to the position of our transform

        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        else //if the character is being controlled elsewhere (another player or ai), assign their position here locally(the host) by the position of its network trasnsform
        {
            transform.position = Vector3.SmoothDamp(transform.position, characterNetworkManager.networkPosition.Value, ref characterNetworkManager.networkPositionVelocity, characterNetworkManager.networkPositionSmoothTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, characterNetworkManager.networkRotation.Value, characterNetworkManager.networkRotationSmoothTime);
        }
    }

    protected virtual void LateUpdate()
    {

    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if(IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            //reset flags that need to be reset

            //if not grounded play arial death anim

            if(!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true); 
            }
        
        }

        //play some death sfx

        yield return new WaitForSeconds(5);

        //award players with runes
        //disable character model
    }


    public virtual void ReviveCharacter()
    {

    }

}
