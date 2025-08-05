using UnityEngine;
using Unity.Netcode;


public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;
    public WeaponItem currentWeaponBeingUsed;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;
    //public bool canComboWithOffHandWeapon = false;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {

        if(player.IsOwner)
        {
            //perform action

            weaponAction.AttemptToPerformAction(player, weaponPerformingAction);

            //also perform hosts action to clients
            player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
        }

    }

    public override void AttemptRiposte(RaycastHit hit)
    {
        CharacterManager targetCharacter = hit.transform.gameObject.GetComponent<CharacterManager>();

        if (targetCharacter == null)
        {
            return;
        }

        if (!targetCharacter.characterNetworkManager.isRipostable.Value)
        {
            return;
        }

        if (targetCharacter.characterNetworkManager.isBeingCriticallyDamaged.Value)
        {
            return;
        }

        MeleeWeaponItem riposteWeapon;
        MeleeWeaponDamageCollider riposteCollider;

        riposteWeapon = player.playerInventoryManager.currentRightHandWeapon as MeleeWeaponItem;
        riposteCollider = player.playerEquipmentManager.rightWeaponManager.meleeWeaponDamageCollider;

        character.characterAnimatorManager.PlayTargetActionAnimationInstantly("Riposte_01", true);

        if(character.IsOwner)
        {
            character.characterNetworkManager.isInvulnerable.Value = true;
        }

        TakeCriticalDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeCriticalDamageEffect);

        damageEffect.physicalDamage = riposteCollider.physicalDamage;
        damageEffect.magicDamage = riposteCollider.magicDamage;
        damageEffect.poiseDamage = riposteCollider.poiseDamage;

        damageEffect.physicalDamage *= riposteWeapon.riposteAttackModifer01;
        damageEffect.magicDamage *= riposteWeapon.riposteAttackModifer01;
        damageEffect.poiseDamage *= riposteWeapon.riposteAttackModifer01;

        targetCharacter.characterNetworkManager.NotifyServerOfRiposteServerRpc(
            targetCharacter.NetworkObjectId,
            character.NetworkObjectId,
            "Riposted_01",
            riposteWeapon.itemID,
            damageEffect.physicalDamage,
            damageEffect.magicDamage,
            damageEffect.poiseDamage);
    }

    public virtual void DrainStaminaBasedOnAttack()
    {
        if(!player.IsOwner)
        {
            return;
        }
        if(currentWeaponBeingUsed == null)
        {
            return;
        }

        float staminaDeducted = 0;

        switch(currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                break;
            case AttackType.LightAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                break;
            case AttackType.LightAttack03:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                break;
            case AttackType.LightAttack04:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                break;
            case AttackType.MeleeAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.meleeAttackStaminaCostMultiplier;
                break;
            case AttackType.HeavyAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                break;
            case AttackType.HeavyAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                break;
            case AttackType.HeavyAttack03:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                break;
            case AttackType.ChargedAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargedAttackStaminaCostMultiplier;
                break;
            case AttackType.ChargedAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargedAttackStaminaCostMultiplier;
                break;
            case AttackType.ChargedAttack03:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargedAttackStaminaCostMultiplier;
                break;
            case AttackType.RunningAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.runningAttackStaminaCostMultiplier;
                break;
            case AttackType.RollingAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.rollingAttackStaminaCostMultiplier;
                break;
            case AttackType.BackstepAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.backstepAttackStaminaCostMultiplier;
                break;
            default:
                break;
        }

        player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
    }

    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);

        if (player.IsOwner)
        {
            PlayerCamera.instance.SetLockCameraHeight();
        }
    }

    public override void EnableCanDoCombo()
    {
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            player.playerCombatManager.canComboWithMainHandWeapon = true;
        }
        else
        {
            //enable off hand combo

        }
    }

    public override void DisableCanDoCombo()
    {
        player.playerCombatManager.canComboWithMainHandWeapon = false;
        //player.playerCombatManager.canComboWithOffHandWeapon = false;

    }

}
