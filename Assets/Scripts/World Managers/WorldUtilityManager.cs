using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager instance;

    [Header("Layers")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask enviroLayers;
    [SerializeField] LayerMask slipperyEnviroLayers;

    [Header("Forces")]
    public float slopeSlideForce = -15f;

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
        DontDestroyOnLoad(gameObject);
    }

    public LayerMask GetCharacterLayers()
    {
        return characterLayers;
    }

    public LayerMask GetEnviroLayers()
    {
        return enviroLayers;
    }

    public LayerMask GetSlipperyEnviroLayers()
    {
        return slipperyEnviroLayers;
    }

    public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
    {
        if(attackingCharacter == CharacterGroup.Team01)
        {
            switch(targetCharacter)
            {
                case CharacterGroup.Team01: return false;
                case CharacterGroup.Team02: return true;
                 default:
                    break;
            }
        }
        else if(attackingCharacter == CharacterGroup.Team02)
        {
            switch (targetCharacter)
            {
                case CharacterGroup.Team01: return true;
                case CharacterGroup.Team02: return false;
                default:
                    break;
            }
        }

        return false;
    }

    public float GetAngleOfTarget(Transform characterTransform, Vector3 targetsDirection)
    {
        targetsDirection.y = 0;
        float viewableAngle = Vector3.Angle(characterTransform.forward, targetsDirection);
        Vector3 cross = Vector3.Cross(characterTransform.forward, targetsDirection);

        if(cross.y < 0)
        {
            viewableAngle = -viewableAngle;
        }

        return viewableAngle;
    }

    public DamageIntensity GetDamageIntensityBasedOnPoiseDamage(float poinseDamage)
    {
        DamageIntensity damageIntensity = DamageIntensity.Normal;

        return damageIntensity;
    }

    public Vector3 GetRipostingPositionBasedOnWeaponClass(WeaponModelType weaponClass)
    {
        Vector3 position = new Vector3(0.11f, 0, 0.7f);

        switch (weaponClass)
        {
            case WeaponModelType.Weapon:
                break;
            case WeaponModelType.Shield:
                break;
            default:
                break;
        }

        return position;
    }
}
