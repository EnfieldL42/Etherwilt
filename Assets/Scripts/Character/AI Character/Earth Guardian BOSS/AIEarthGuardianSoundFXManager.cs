using UnityEngine;

public class AIEarthGuardianSoundFXManager : CharacterSoundFXManager
{
    CharacterManager character;
    [SerializeField] AudioSource movingAudioSource;

    [Header("Attacking Whooshes SFX")]
    public AudioClip[] attackingWhooshes;

    [Header("Attacking Ground Impacts SFX")]
    public AudioClip[] attackingImpacts;

    [Header("Attacking AOE SFX")]
    public AudioClip[] attackingAOE;

    [Header("Moving SFX")]
    public AudioClip movingSFX;

    override protected void Awake()
    {
        base.Awake();
        character = GetComponent<CharacterManager>();
    }

    protected override void Update()
    {
        if(character.characterNetworkManager.isMoving.Value)
        {
            PlayMovingSoundFX();
        }
        else
        {
            if (movingAudioSource.isPlaying)
            {
                movingAudioSource.Stop();
            }
        }
    }


    public virtual void PlayGroundImpactSoundFX()
    {
        if (attackingImpacts.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackingImpacts));
        }
    }

    public virtual void PlayAOESoundFX()
    {
        if (attackingAOE.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackingAOE));
        }
    }

    public virtual void PlayMovingSoundFX()
    {
        if(movingSFX != null)
        {
            // Play dragging SFX if not already playing
            if (!movingAudioSource.isPlaying)
            {
                movingAudioSource.clip = movingSFX;
                movingAudioSource.loop = true;
                movingAudioSource.Play();
            }
        }

    }

}
