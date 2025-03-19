using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;
    CharacterStatsManager characterStatsManager;

    public float verticalMovement;
    public float horizontalMovement;
    public float moveAmount;

    [Header("Movement Settings")]
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;
    [SerializeField] float walkingSpeed = 2f;
    [SerializeField] float runningSpeed = 7f;
    [SerializeField] float sprintSpeed = 9f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float sprintingStaminaCost = 2;

    [Header("Jump")]
    [SerializeField] float jumpStaminaCost = 25;
    [SerializeField] float jumpHeight = 4;
    [SerializeField] float jumpForwardSpeed = 5;
    [SerializeField] float freeFallSpeed = 2;
    private Vector3 jumpDirection;


    [Header("Dodge")]
    private Vector3 rollDirection;
    [SerializeField] float dodgeStaminaCost = 25;


    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();//
        characterStatsManager = GetComponent<CharacterStatsManager>();
    }

    protected override void Update()
    {
        base.Update();

        if(player.IsOwner)
        {
            player.characterNetworkManager.verticalMovement.Value = verticalMovement;
            player.characterNetworkManager.horizontalMovement.Value = horizontalMovement;
            player.characterNetworkManager.moveAmount.Value = moveAmount;
        }
        else
        {
            verticalMovement = player.characterNetworkManager.verticalMovement.Value;
            horizontalMovement = player.characterNetworkManager.horizontalMovement.Value;
            moveAmount = player.characterNetworkManager.moveAmount.Value;

            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
        }
    }

    public void HandleAllMovement()
    {


        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandeFreeFallMovement();
    }

    private void GetMovementValues()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
    }



    private void HandleGroundedMovement()
    {



        if (!player.canMove)
        {
            return;
        }

        GetMovementValues();

        //move direction is based on cameras facing perspective & the movement input
        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if(player.playerNetworkManager.isSprinting.Value)
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

    private void HandleJumpingMovement()
    {
        if(player.isJumping)
        {
            player.characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    private void HandeFreeFallMovement()
    {
        if (!player.isGrounded)
        {
            Vector3 freeFallDirection;

            freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
            freeFallDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
            freeFallDirection.y = 0;

            player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);

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

        if (player.characterNetworkManager.currentStamina.Value <= 0)
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

        player.characterNetworkManager.currentStamina.Value -= dodgeStaminaCost;



    }

    public void AttemptToPerformJump()
    {

        //prevents double jumping
        if (player.isPerformingAction)
        {
            return;
        }

        if (player.characterNetworkManager.currentStamina.Value <= 0)
        {
            return;
        }

        if(player.isJumping)
        {
            return;
        }

        if (!player.isGrounded)
        {
            return;
        }
        //

        //check if we play one handed animation or two handed animation
        player.playerAnimatorManager.PlayTargetActionAnimation("main_jump_start_01", false);

        player.isJumping = true;


        player.characterNetworkManager.currentStamina.Value -= jumpStaminaCost;


        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
        jumpDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero)
        {

            if (player.playerNetworkManager.isSprinting.Value)
            {
                jumpDirection *= 1;
            }
            else if (PlayerInputManager.instance.moveAmount > 0.5)
            {
                jumpDirection *= 0.5f;
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5)
            {
                jumpDirection *= 0.25f;

            }
        }


    }


    public void HandleSprinting()
    {
        if(player.isPerformingAction)
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if (player.playerNetworkManager.currentStamina.Value <= 0)
        {
            player.playerNetworkManager.isSprinting.Value = false;
            return;
        }

        if (moveAmount >= 0.5)
        {
            player.playerNetworkManager.isSprinting.Value = true;
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }

        if(player.playerNetworkManager.isSprinting.Value)
        {
            player.playerNetworkManager.currentStamina.Value -= sprintingStaminaCost * Time.deltaTime;

        }

    }

    public void ApplyJumpingVelocity()
    {
        //apply upwards velocity
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);

    }

}
