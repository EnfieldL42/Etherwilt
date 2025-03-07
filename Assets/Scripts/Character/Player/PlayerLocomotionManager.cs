using Unity.Mathematics;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    CharacterStatsManager characterStatsManager;

    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    [Header("Movement Settings")]
    [SerializeField] private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float walkingSpeed = 2f;
    [SerializeField] float runningSpeed = 7f;
    [SerializeField] float rotationSpeed = 15f;

    [Header("Dodge")]
    private Vector3 rollDirection;
    [SerializeField] float dodgeStaminaCost = 25;

    [Header("Sprint")]
    public bool isSprinting = false;
    [SerializeField] float sprintSpeed = 9f;
    [SerializeField] float sprintingStaminaCost = 2;


    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();//
        characterStatsManager = GetComponent<CharacterStatsManager>();
    }

    protected override void Update()
    {
        
    }

    public void HandleAllMovement()
    {


        HandleGroundedMovement();
        HandleRotation();
    }

    private void GetMovementValues()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
    }



    private void HandleGroundedMovement()
    {

        GetMovementValues();

        if (!player.canMove)
        {
            return;
        }

        //move direction is based on cameras facing perspective & the movement input
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if(isSprinting)
        {
            player.characterController.Move(moveDirection * sprintSpeed * Time.deltaTime);

        }
        else
        {
            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                //move at running speed
                player.characterController.Move(moveDirection * runningSpeed * Time.deltaTime);

            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f)
            {
                //move at walking speed
                player.characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);

            }
        }




    }

    private void HandleRotation()
    {
        if(!player.canRotate)
        {
            return;
        }

        targetRotationDirection = Vector3.zero;
        targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
        targetRotationDirection = targetRotationDirection + PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
        targetRotationDirection.Normalize();
        targetRotationDirection.y = 0;

        if (targetRotationDirection == Vector3.zero)
        {
            targetRotationDirection = transform.forward;
        }

        Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = targetRotation;
    }

    public void AttemptToPerformDodge()
    {
        if (player.isPerformingAction)
        {
            return;
        }

        if(characterStatsManager.currentStamina <= 0)
        {
            return;
        }

        if (moveAmount > 0)//if we are moving it performs a roll
        {
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
            rollDirection.y = 0;
            rollDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            player.playerAnimatorManager.PlayTargetActionAnimation("Roll_Forward_01", true, true);

        }
        else//if we are stationary it performs a backstep
        {
            player.playerAnimatorManager.PlayTargetActionAnimation("Back_Step_01", true, true);

        }

        characterStatsManager.currentStamina -= dodgeStaminaCost;


    }

    public void HandleSprinting()
    {
        if(player.isPerformingAction)
        {
            isSprinting = false;
        }

        if(player.playerStatsManager.currentStamina <= 0)

        {
            isSprinting = false;
            return;
        }

        if (moveAmount >= 0.5)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        if(isSprinting)
        {
            float newStamina = player.playerStatsManager.currentStamina - (sprintingStaminaCost * Time.deltaTime);
            player.playerStatsManager.currentStamina = newStamina;
        }

    }

    public void SetisSprintingToFalse()
    {
        isSprinting = false;
    }

}
