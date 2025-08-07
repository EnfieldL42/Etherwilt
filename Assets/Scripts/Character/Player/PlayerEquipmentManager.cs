using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;

    [Header("Weapon Model Instantiation Slot")]
    public WeaponModelInstantiationSlot rightHandWeaponSlot;
    public WeaponModelInstantiationSlot leftHandWeaponSlot;
    public WeaponModelInstantiationSlot leftHandShieldSlot;

    [Header("Weapon Managers")]
    public WeaponManager rightWeaponManager;
    public WeaponManager leftWeaponManager;

    [Header("Weapon Models")]
    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>(); 
        InitializeWeaponSlots();

    }

    protected override void Start()
    {
        base.Start();

        LoadWeaponsOnBothHands();
    }

    private void InitializeWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();
        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.weaponSlot == WeaponModelSlot.RightHandWeaponSlot)
            {
                rightHandWeaponSlot = weaponSlot;
            }
            else if(weaponSlot.weaponSlot == WeaponModelSlot.LeftHandWeaponSlot)
            {
                leftHandWeaponSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHandShieldSlot)
            {
                leftHandShieldSlot = weaponSlot;
            }

        }
    }

    public void LoadWeaponsOnBothHands()
    {
        LoadLeftWeapon();
        LoadRightWeapon();
    }


    //QUICK SLOTS
    public void SwitchQuickSlotItem()
    {
        if (!player.IsOwner)
        {
            return;
        }

        if (player.isPerformingAction)
        {
            return;
        }

        if (player.isDead.Value)
        {
            return;
        }


        QuickSlotItem selectedItem = null;

        //add one to index to switch to next potential weapon
        player.playerInventoryManager.quickSlotItemIndex += 1;

        //if index is out of bounds we reset it back to 0
        if (player.playerInventoryManager.quickSlotItemIndex < 0 || player.playerInventoryManager.quickSlotItemIndex > 2)
        {
            player.playerInventoryManager.quickSlotItemIndex = 0;

            //check if we are holding more than one weapon
            float itemCount = 0;
            QuickSlotItem firstItem = null;
            int firstItemPosition = 0;

            for (int i = 0; i < player.playerInventoryManager.quickSlotItemsInQuickSlots.Length; i++)
            {
                if (player.playerInventoryManager.quickSlotItemsInQuickSlots[i] != null)
                {
                    itemCount++;

                    if (firstItem == null)
                    {
                        firstItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[i];
                        firstItemPosition = i;
                    }

                }
            }

            if (itemCount <= 1)
            {
                player.playerInventoryManager.quickSlotItemIndex = -1;
                selectedItem = null;
                player.playerNetworkManager.currentQuickSlotItemID.Value = -1;
            }
            else
            {
                player.playerInventoryManager.quickSlotItemIndex = firstItemPosition;
                player.playerNetworkManager.currentQuickSlotItemID.Value = firstItem.itemID;
            }

            return;
        }

        if (player.playerInventoryManager.quickSlotItemsInQuickSlots[player.playerInventoryManager.quickSlotItemIndex] != null)
        {
            selectedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[player.playerInventoryManager.quickSlotItemIndex];
            //assign network weapon ID

            player.playerNetworkManager.currentQuickSlotItemID.Value = player.playerInventoryManager.quickSlotItemsInQuickSlots[player.playerInventoryManager.quickSlotItemIndex].itemID;
        }
        else
        {
            player.playerNetworkManager.currentQuickSlotItemID.Value = -1;
        }

        if(selectedItem == null && player.playerInventoryManager.quickSlotItemIndex <= 2)
        {
            SwitchQuickSlotItem();
        }
    }


    //RIGHT HAND 
    public void SwitchRightWeapon()
    {
        if(!player.IsOwner)
        {
            return;
        }

        if(player.isPerformingAction)
        {
            return;
        }
        
        if(player.isDead.Value)
        {
            return;
        }

        if(!player.isGrounded)
        {
            return;
        }

        player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Right_Weapon_01", false, false, true, true);

        WeaponItem selectedWeapon = null;
        
        //disable two handing if two handing

        //add one to index to switch to next potential weapon
        player.playerInventoryManager.rightHandWeaponIndex += 1;

        //if index is out of bounds we reset it back to 0
        if(player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex > 2)
        {
            player.playerInventoryManager.rightHandWeaponIndex = 0;

            //check if we are holding more than one weapon
            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPosition = 0;

            for (int i = 0; i < player.playerInventoryManager.weaponsInRightHandSlots.Length; i++)
            {
                if (player.playerInventoryManager.weaponsInRightHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    weaponCount++;

                    if (firstWeapon == null)
                    {
                        firstWeapon = player.playerInventoryManager.weaponsInRightHandSlots[i];
                        firstWeaponPosition = i;
                    }

                }
            }

            if (weaponCount <= 1)
            {
                player.playerInventoryManager.rightHandWeaponIndex = -1;
                selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
            }
            else
            {
                player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosition;
                player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
            }

            return;
        }

        foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInRightHandSlots)
        {
            //if weapon id does not equal to unarmed weapon then..
            if (player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                selectedWeapon = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex];
                //assign network weapon ID

                player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[player.playerInventoryManager.rightHandWeaponIndex].itemID;
                return;
            }
        }

        if (selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
        {
            SwitchRightWeapon();
        }

    }

    public void LoadRightWeapon()
    {
        if(player.playerInventoryManager.currentRightHandWeapon != null)
        {
            //remove old
            rightHandWeaponSlot.UnloadWeapon();

            rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandWeaponSlot.LoadWeapon(rightHandWeaponModel);
            rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
            player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);
        }
    }


    //LEFT HAND
    public void SwitchLeftWeapon()
    {
        if (!player.IsOwner)
        {
            return;
        }

        if (player.isPerformingAction)
        {
            return;
        }

        if (player.isDead.Value)
        {
            return;
        }

        if (!player.isGrounded)
        {
            return;
        }

        player.playerAnimatorManager.PlayTargetActionAnimation("Swap_Left_Weapon_01", false, false, true, true);

        WeaponItem selectedWeapon = null;

        //disable two handing if two handing

        //add one to index to switch to next potential weapon
        player.playerInventoryManager.leftHandWeaponIndex += 1;

        //if index is out of bounds we reset it back to 0
        if (player.playerInventoryManager.leftHandWeaponIndex < 0 || player.playerInventoryManager.leftHandWeaponIndex > 2)
        {
            player.playerInventoryManager.leftHandWeaponIndex = 0;

            //check if we are holding more than one weapon
            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPosition = 0;

            for (int i = 0; i < player.playerInventoryManager.weaponsInLeftHandSlots.Length; i++)
            {
                if (player.playerInventoryManager.weaponsInLeftHandSlots[i].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    weaponCount++;

                    if (firstWeapon == null)
                    {
                        firstWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[i];
                        firstWeaponPosition = i;
                    }

                }
            }

            if (weaponCount <= 1)
            {
                player.playerInventoryManager.leftHandWeaponIndex = -1;
                selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
                player.playerNetworkManager.currentLeftHandWeaponID.Value = selectedWeapon.itemID;
            }
            else
            {
                player.playerInventoryManager.leftHandWeaponIndex = firstWeaponPosition;
                player.playerNetworkManager.currentLeftHandWeaponID.Value = firstWeapon.itemID;
            }

            return;
        }

        foreach (WeaponItem weapon in player.playerInventoryManager.weaponsInLeftHandSlots)
        {
            //if weapon id does not equal to unarmed weapon then..
            if (player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
            {
                selectedWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex];
                //assign network weapon ID

                player.playerNetworkManager.currentLeftHandWeaponID.Value = player.playerInventoryManager.weaponsInLeftHandSlots[player.playerInventoryManager.leftHandWeaponIndex].itemID;
                return;
            }
        }

        if (selectedWeapon == null && player.playerInventoryManager.leftHandWeaponIndex <= 2)
        {
            SwitchLeftWeapon();
        }
    }

    public void LoadLeftWeapon()
    {
        if (player.playerInventoryManager.currentLeftHandWeapon != null)
        {
            if(leftHandWeaponSlot.currentWeaponModel != null)
            {
                leftHandWeaponSlot.UnloadWeapon();
            }
            if (leftHandShieldSlot.currentWeaponModel != null)
            {
                leftHandShieldSlot.UnloadWeapon();
            }

            leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);

            switch (player.playerInventoryManager.currentLeftHandWeapon.weaponModelType)
            {
                case WeaponModelType.Weapon:
                    leftHandWeaponSlot.LoadWeapon(leftHandWeaponModel);
                    break;
                case WeaponModelType.Shield:
                    leftHandShieldSlot.LoadWeapon(leftHandWeaponModel);
                    break;
                default:
                    break;
            }



            leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
            //assgn weapon dmg

        }
    }


    //Damage
    public void OpenDamageCollider()
    {

        //open right hand damage collider
        if(player.playerNetworkManager.isUsingRightHand.Value)
        {
            rightWeaponManager.meleeWeaponDamageCollider.EnableDamageCollider();
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentRightHandWeapon.whooshes));
        }
        //open left hand damage collder
        else if(player.playerNetworkManager.isUsingLeftHand.Value)
        {
            leftWeaponManager.meleeWeaponDamageCollider.EnableDamageCollider();
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentLeftHandWeapon.whooshes));
        }

        //play sword/swoosh sound fx
    }

    public void CloseDamageCollider()
    {
        //close right hand damage collider
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            rightWeaponManager.meleeWeaponDamageCollider.DisableDamageCollider();
        }
        //close left hand damage collder
        else if (player.playerNetworkManager.isUsingLeftHand.Value)
        {
            leftWeaponManager.meleeWeaponDamageCollider.DisableDamageCollider();
        }
    }


    //UNHIDE WEAPONS
    public void UnhideWeapons()
    {
        if (player.playerEquipmentManager.rightHandWeaponSlot != null)
        {
            player.playerEquipmentManager.rightHandWeaponModel.SetActive(true);
        }

        if (player.playerEquipmentManager.leftHandWeaponModel != null)
        {
            player.playerEquipmentManager.leftHandWeaponModel.SetActive(true);
        }
    }
}
