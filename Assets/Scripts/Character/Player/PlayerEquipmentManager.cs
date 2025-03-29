using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;

    public WeaponModelInstantiationSlot rightHandSlot;
    public WeaponModelInstantiationSlot leftHandSlot;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        InstantializeWeaponSlots();
    }

    protected override void Start()
    {
        base.Start();

        LoadWeaponOnBothHands();
    }   

    private void InstantializeWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

        foreach (var weaponSlot in weaponSlots)
        {
            if(weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
            else if(weaponSlot.weaponSlot == WeaponModelSlot.LeftHand)
            {
                leftHandSlot = weaponSlot;
            }


        }
    }

    public void LoadWeaponOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();
    }

    public void LoadRightWeapon()
    {
        if(player.PlayerInventoryManager.currentRightHandWeapon != null)
        {
            rightHandWeaponModel = Instantiate(player.PlayerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
        }
    }

    public void LoadLeftWeapon()
    {
        if (player.PlayerInventoryManager.currentLeftHandWeapon != null)
        {
            leftHandWeaponModel = Instantiate(player.PlayerInventoryManager.currentLeftHandWeapon.weaponModel);
            leftHandSlot.LoadWeapon(leftHandWeaponModel);

        }

    }
}
