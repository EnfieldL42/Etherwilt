using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;

    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;

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

}
