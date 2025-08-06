using UnityEngine;

public class PlayerUIEquipmentManagerInput : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerUIEquipmentManager playerUIEquipmentManager;

    [Header("Inputs")]
    [SerializeField] bool unequipItemInput;

    private void Awake()
    {
        playerUIEquipmentManager = GetComponentInParent<PlayerUIEquipmentManager>();

    }
    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMenu.UseItem.performed += i => unequipItemInput = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        HandlePlayerUIEquipmentManagerInput();
    }

    private void HandlePlayerUIEquipmentManagerInput()
    {
        if(unequipItemInput)
        {
            unequipItemInput = false;
            playerUIEquipmentManager.UnequipSelectedEquip();
        }
    }
}
