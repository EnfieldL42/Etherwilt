using Unity.Netcode;
using UnityEngine;

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

    [Header("UI Flags")]
    public bool menuWindowIsOpen = false;
    public bool popUpWindowIsOpen = false;
    public bool bonfireWindowIsOpen = false;

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

        playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
        playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
        playerUICharacterMenuManager = GetComponentInChildren<PlayerUICharacterMenuManager>();
        playerUIEquipmentManager = GetComponentInChildren<PlayerUIEquipmentManager>();
        playerUIBonfireManager = GetComponentInChildren<PlayerUIBonfireManager>();
        playerUITeleportLocationManager = GetComponentInChildren<PlayerUITeleportLocationManager>();
        playerUILoadingScreenManager = GetComponentInChildren<PlayerUILoadingScreenManager>();
        playerUILevelUpManager = GetComponentInChildren<PlayerUILevelUpManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
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
        playerUIBonfireManager.CloseMenuAfterFixedUpdate();
        playerUITeleportLocationManager.CloseMenuAfterFixedUpdate();
        playerUILevelUpManager.CloseMenuAfterFixedUpdate();
    }

}
