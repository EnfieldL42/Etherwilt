using UnityEngine;
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

        player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
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
}
