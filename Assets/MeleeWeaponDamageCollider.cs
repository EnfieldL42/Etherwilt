using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager CharacterManager; //when calculating dmg, this is used to check for attackers dmg modifiers, effects, etc

}
