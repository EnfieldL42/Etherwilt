using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CharacterFootStepSFXMaker : MonoBehaviour
{
    CharacterManager character;

    AudioSource audioSource;
    GameObject steppedOnObject;

    private bool hasTouchdGround = false;
    private bool hasPlayedFootstepSFX = false;
    [SerializeField] float distanceToGround = 0.05f;

    private void Awake()
    {
        character = GetComponentInParent<CharacterManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        CheckForFootSteps();
    }

    private void CheckForFootSteps()
    {
        if(character == null)
        {
            return;
        }

        if(!character.characterNetworkManager.isMoving.Value)
        {
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, character.transform.TransformDirection(Vector3.down), out hit, distanceToGround, WorldUtilityManager.instance.GetEnviroLayers()))
        {
            hasTouchdGround = true;

            if(!hasPlayedFootstepSFX)
            {
                steppedOnObject = hit.transform.gameObject;
            }
            else
            {
                hasTouchdGround = false;
                hasPlayedFootstepSFX = false;
                steppedOnObject = null;
            }

            if(hasTouchdGround && !hasPlayedFootstepSFX)
            {
                hasPlayedFootstepSFX = true;
                PlayerFootStepSoundFX();
            }
        }
    }

    private void PlayerFootStepSoundFX()
    {
        character.characterSoundFXManager.PlayFootStepSoundFX();
    }
}
