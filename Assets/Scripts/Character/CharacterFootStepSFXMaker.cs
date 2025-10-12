using UnityEngine;

public class CharacterFootStepSFXMaker : MonoBehaviour
{
    CharacterManager character;
    AudioSource audioSource;
    GameObject steppedOnObject;

    private bool hasTouchdGround = false;
    private bool hasPlayedFootstepSFX = false;
    [SerializeField] float distanceToGround = 0.05f;

    [SerializeField] private float footstepCooldown = 0.2f; // small delay between footstep sounds
    private float footstepTimer = 0f;

    private void Awake()
    {
        character = GetComponentInParent<CharacterManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        footstepTimer += Time.fixedDeltaTime;
        CheckForFootSteps();
    }

    private void CheckForFootSteps()
    {
        if (character == null)
            return;

        if (!character.characterNetworkManager.isMoving.Value)
            return;

        if (!character.canMove)
            return;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, character.transform.TransformDirection(Vector3.down), out hit, distanceToGround, WorldUtilityManager.instance.GetEnviroLayers()))
        {
            hasTouchdGround = true;

            if (!hasPlayedFootstepSFX)
            {
                steppedOnObject = hit.transform.gameObject;
            }
            else
            {
                hasTouchdGround = false;
                hasPlayedFootstepSFX = false;
                steppedOnObject = null;
            }

            // ✅ Add cooldown check here
            if (hasTouchdGround && !hasPlayedFootstepSFX && footstepTimer >= footstepCooldown)
            {
                hasPlayedFootstepSFX = true;
                footstepTimer = 0f; // reset timer
                PlayerFootStepSoundFX();
            }
        }
    }

    private void PlayerFootStepSoundFX()
    {
        character.characterSoundFXManager.PlayFootStepSoundFX();
    }
}
