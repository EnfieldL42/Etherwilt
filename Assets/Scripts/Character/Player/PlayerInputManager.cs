using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;


    public PlayerManager player;
    PlayerControls playerControls;

    [Header("Camera Input")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Movement Input")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Player Action Input")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;
    [SerializeField] bool RBInput = false;
    [SerializeField] bool RightArrowInput = false;
    [SerializeField] bool LeftArrowInput = false;
    [SerializeField] bool reviveInput = false;

    [Header("Lock On Input")]
    [SerializeField] bool lockOnInput;
    [SerializeField] bool lockOnLeftInput;
    [SerializeField] bool lockOnRightInput;
    private Coroutine lockOnCoroutine;



    private void Awake()
    {


        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }



    }

    private void OnEnable()
    {

        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            //movement
            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>(); //stores vector 2 of input in i then reads it and adds it to the vector 2 movement
            playerControls.PlayerMovement.Movement.canceled += i => movementInput = Vector2.zero;
            //camera
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();
            //playerControls.PlayerCamera.Mouse.performed += i => cameraInput = i.ReadValue<Vector2>();

            //dodge
            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;

            //sprint
            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

            //Attacking
            playerControls.PlayerActions.RB.performed += i => RBInput = true;
            playerControls.PlayerActions.LockOn.performed += i => lockOnInput = true;
            playerControls.PlayerActions.LockOnSeekLeftTarget.performed += i => lockOnLeftInput = true;
            playerControls.PlayerActions.LockOnSeekRightTarget.performed += i => lockOnRightInput = true;


            //Dpad
            playerControls.PlayerActions.RightArrow.performed += i => RightArrowInput = true;
            playerControls.PlayerActions.LeftArrow.performed += i => LeftArrowInput = true;
            playerControls.PlayerActions.ReviveInput.performed += i => reviveInput = true;

        }

        playerControls.Enable();

    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange; //stop checking if scene is being changed
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += OnSceneChange; //checks if the scene has changed


        instance.enabled = false;

        if(playerControls != null)
        {
            playerControls.Disable();
        }

    }

    private void Update()
    {
        HandleAllInputs();

    }

    private void OnApplicationFocus(bool focus)//cant move player if tabbed out of the game
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();

            }
            else
            {
                playerControls.Disable();

            }
        }

    }
    private void OnSceneChange(Scene oldScene, Scene newScene)    //checks if the the scene is the main world scene, enables player input if it is/ disables if its not(main menu)

    {
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        else
        {
            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }
    }




    private void HandleAllInputs()
    {
        HandleCameraMovementInput();
        HandlePlayerMovementInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();

        HandleRBInput();
        HandleLockOnInput();
        HandleLockOnSwitchInput();

        HandleRightWeaponSwitch();
        HandleLeftWeaponSwitch();
        HandleRevive();

    }

    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        //returns the absolute number, just want to know if we are moving, not left or right
        //mathf.abs(no negatives) was used so that inputs will always add to one another instead of potentially minusing as input can be negative
        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        //moveAmount 0 = idle/ 0.5 = walking/ 1 = running (optional but souls game use it)
        if(moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 &&  moveAmount <= 1)
        {
            moveAmount = 1;
        }
        
        //0 on horizontal for non-strafing movement (no left or right as you are always going forwards when not locked on)
        //the horizontal will be used when we are strafing or locked on

        if (player == null)
        {
            return;
        }

        if (!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);

        }
        else
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.playerNetworkManager.isSprinting.Value);

        }

    }

    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;

    }

    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;

            player.playerLocomotionManager.AttemptToPerformDodge();

        }
    }

    private void HandleSprintInput()
    {
        if (sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
    }

    private void HandleJumpInput()
    {
        if(jumpInput)
        {
            jumpInput = false;

            //if ui window open, return without doing anything

            //attempt to perform jump
            player.playerLocomotionManager.AttemptToPerformJump();
        }

    }

    private void HandleRBInput()
    {
        if(RBInput)
        {
            RBInput = false;

            //TODO: if we have UI window, return and do nothing

            player.playerNetworkManager.SetCharacterActionHand(true);

            //TODO: if we are two handing the weapon, use the two handed action

            player.PlayerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oneHandedRBAction, player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleRightWeaponSwitch()
    {
        if(RightArrowInput)
        {
            RightArrowInput = false;

            player.switchRightWeapon = true;

        }
    }

    private void HandleLeftWeaponSwitch()
    {
        if(LeftArrowInput)
        {
            LeftArrowInput = false;
            player.switchLeftWeapon = true;
        }
    }

    private void HandleRevive()
    {
        if(reviveInput)
        {
            reviveInput = false;

            player.respawnCharacter = true;
        }
    }

    private void HandleLockOnInput()
    {
        if(player.playerNetworkManager.isLockedOn.Value)//checks if target lock on is dead
        {
            if(player.PlayerCombatManager.currentTarget == null)
            {
                return;
            }

            if (player.PlayerCombatManager.currentTarget.isDead.Value)
            {
                player.playerNetworkManager.isLockedOn.Value = false;
            }

            //attempt to find new target


            //this makes it so the couritine cannot be run at the same time
            if(lockOnCoroutine != null)
            {
                StopCoroutine(lockOnCoroutine);
            }

            lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
        }

        if(lockOnInput && player.playerNetworkManager.isLockedOn.Value)
        {
            lockOnInput = false;
            PlayerCamera.instance.ClearLockOnTargets();
            player.playerNetworkManager.isLockedOn.Value = false;

            //disable lock on
            return;
        }

        if (lockOnInput && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOnInput = false;
            //if we are aiming with ranged then return

            PlayerCamera.instance.HandleLocatingLockOnTarget();

            if(PlayerCamera.instance.nearestLockOnTarget != null)
            {
                //set the target as our current target
                player.PlayerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }
        }
    }

    private void HandleLockOnSwitchInput()
    {
        if(lockOnLeftInput)
        {
            lockOnLeftInput = false;

            if(player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLocatingLockOnTarget();

                if(PlayerCamera.instance.leftLockOnTarget != null)
                {
                    player.PlayerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }

            }
        }

        if (lockOnRightInput)
        {
            lockOnRightInput = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLocatingLockOnTarget();

                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    player.PlayerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }

            }
        }
    }


}

