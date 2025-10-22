using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;
    [HideInInspector] public PlayerManager localPlayer;

    [Header("NETWORK JOIN")]
    [SerializeField] bool startGameAsClient;


    [HideInInspector] public PlayerUIHudManager playerUIHudManager;
    [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
    [HideInInspector] public PlayerUICharacterMenuManager playerUICharacterMenuManager;
    [HideInInspector] public PlayerUIEquipmentManager playerUIEquipmentManager;
    [HideInInspector] public PlayerUIBonfireManager playerUIBonfireManager;
    [HideInInspector] public PlayerUITeleportLocationManager playerUITeleportLocationManager;
    [HideInInspector] public PlayerUILoadingScreenManager playerUILoadingScreenManager;
    [HideInInspector] public PlayerUILevelUpManager playerUILevelUpManager;
    [HideInInspector] public PlayerUITutorialManager playerUITutorialManager;

    [Header("UI Flags")]
    public bool menuWindowIsOpen = false;
    public bool popUpWindowIsOpen = false;
    public bool bonfireWindowIsOpen = false;


    [Header("Device Inputs")]
    public static ControlScheme CurrentControlScheme { get; private set; }
    public InputActionAsset inputActions;
    public static event Action<ControlScheme> OnInputSchemeChanged;
    public bool isUsingGamepad;

    [Header("UI keybinds")]
    [SerializeField] Image[] playerUIScreenSubmitImage;
    [SerializeField] Image[] playerUIEscapeImage;
    [SerializeField] Image[] playerUIUnequipImage;

    [SerializeField] Sprite enterKeyboardSprite;
    [SerializeField] Sprite escapeKeyboardSprite;
    [SerializeField] Sprite unequipKeyboardSprite;
    [SerializeField] Sprite enterXboxSprite;
    [SerializeField] Sprite escapeXboxSprite;
    [SerializeField] Sprite unequipXboxSprite;
    //[SerializeField] Sprite enterPSSprite;
    //[SerializeField] Sprite escapePSSprite;
    //[SerializeField] Sprite unequipPSSprite;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
        playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
        playerUICharacterMenuManager = GetComponentInChildren<PlayerUICharacterMenuManager>();
        playerUIEquipmentManager = GetComponentInChildren<PlayerUIEquipmentManager>();
        playerUIBonfireManager = GetComponentInChildren<PlayerUIBonfireManager>();
        playerUITeleportLocationManager = GetComponentInChildren<PlayerUITeleportLocationManager>();
        playerUILoadingScreenManager = GetComponentInChildren<PlayerUILoadingScreenManager>();
        playerUILevelUpManager = GetComponentInChildren<PlayerUILevelUpManager>();
        playerUITutorialManager = GetComponentInChildren<PlayerUITutorialManager>();
    }

    private void Update()
    {
        if(startGameAsClient)
        {
            startGameAsClient = false;
            NetworkManager.Singleton.Shutdown();//must shut down the network as a host to start as a client?
            //then we restart as client
            NetworkManager.Singleton.StartClient();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-enable listening in case Unity resets it between scenes
        if (InputUser.listenForUnpairedDeviceActivity <= 0)
            InputUser.listenForUnpairedDeviceActivity++;

        // Reattach the callback if it was lost
        InputUser.onUnpairedDeviceUsed -= OnDeviceChanged;
        InputUser.onUnpairedDeviceUsed += OnDeviceChanged;

        StartCoroutine(DelayedDeviceDetection());
    }


    public void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseAllMenuWindows()
    {
        playerUICharacterMenuManager.CloseMenuAfterFixedUpdate();
        playerUIEquipmentManager.CloseMenuAfterFixedUpdate();
        CloseBonfireWindows();
    }

    public void CloseBonfireWindows()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        player.canMove = true;
        player.canRotate = true;

        bonfireWindowIsOpen = false;
        playerUIBonfireManager.CloseMenuAfterFixedUpdate();
        playerUITeleportLocationManager.CloseMenuAfterFixedUpdate();
        playerUILevelUpManager.CloseMenuAfterFixedUpdate();
    }



    private void OnDeviceChanged(InputControl control, InputEventPtr eventPtr)
    {
        var device = control.device;

        if (device is Gamepad && CurrentControlScheme != ControlScheme.Gamepad)
        {
            CurrentControlScheme = ControlScheme.Gamepad;
            OnInputSchemeChanged?.Invoke(ControlScheme.Gamepad);
            PlayerCamera.instance?.SwitchToGamePadSensitivity();
            LockMouse();
            isUsingGamepad = true;
            ChangeUIToXbox();

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                TitleScreenManager.instance.ChangeMainMenuUIToXbox();
            }
        }
        else if ((device is Pointer || device is Keyboard) && CurrentControlScheme != ControlScheme.KeyboardMouse)
        {
            CurrentControlScheme = ControlScheme.KeyboardMouse;
            OnInputSchemeChanged?.Invoke(ControlScheme.KeyboardMouse);
            PlayerCamera.instance?.SwitchToMouseSensitivity();
            isUsingGamepad = false;
            ChangeUIToKeyboard();

            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                UnlockMouse();
                TitleScreenManager.instance.ChangeMainMenuUIToKeyboard();
            }
            else
                LockMouse();

        }

    }

    private IEnumerator DelayedDeviceDetection()
    {
        yield return null;
        SimulateInitialDeviceDetection();
    }

    private void SimulateInitialDeviceDetection()
    {
        // Prioritize gamepad if present
        var gamepad = Gamepad.current;
        var keyboard = Keyboard.current;
        var mouse = Mouse.current;

        if (gamepad == null)
        {
            PlayerCamera.instance.SwitchToMouseSensitivity();

        }

        if (gamepad != null)
        {
            OnDeviceChanged(gamepad, new InputEventPtr());
        }

        // Else use keyboard or mouse
        if (keyboard != null)
        {
            OnDeviceChanged(Keyboard.current, new InputEventPtr());
        }
        else if (mouse != null)
        {
            OnDeviceChanged(Mouse.current, new InputEventPtr());
        }
    }

    public enum ControlScheme
    {
        KeyboardMouse = 0, Gamepad = 1 // just need to be same indexes as defined in inputActionAsset
    }


    public void ChangeUIToKeyboard()
    {
        //enterKeyboardImage;
        //deleteKeyboardImage;

        foreach (Image img in playerUIScreenSubmitImage)
        {
            img.sprite = enterKeyboardSprite;
        }

        foreach (Image img in playerUIEscapeImage)
        {
            img.sprite = escapeKeyboardSprite;
        }

        foreach (Image img in playerUIUnequipImage)
        {
            img.sprite = unequipKeyboardSprite;
        }
    }

    public void ChangeUIToXbox()
    {
        foreach (Image img in playerUIScreenSubmitImage)
        {
            img.sprite = enterXboxSprite;
        }

        foreach (Image img in playerUIEscapeImage)
        {
            img.sprite = escapeXboxSprite;
        }

        foreach (Image img in playerUIUnequipImage)
        {
            img.sprite = unequipXboxSprite;
        }
    }

}
