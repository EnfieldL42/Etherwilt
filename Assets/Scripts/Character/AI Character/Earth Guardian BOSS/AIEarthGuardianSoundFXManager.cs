using UnityEngine;
using UnityEngine.Animations.Rigging;

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

    [Header("Burring SFX")]
    public AudioClip burrowSFX;

    [Header("Unburring SFX")]
    public AudioClip unburrowSFX;

    public AudioSource audio2DSource;


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

    public void Play2DSoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
    {
        audio2DSource.PlayOneShot(soundFX, volume);
        audio2DSource.pitch = 1;
        if (randomizePitch)
        {
            audio2DSource.pitch += Random.Range(-pitchRandom, pitchRandom);
        }
    }

    public virtual void PlayGroundImpactSoundFX()
    {
        if (attackingImpacts.Length > 0)
        {
            Debug.Log("attack slam sound");
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

    public virtual void PlayBurrowSFX()
    {
        if (burrowSFX != null)
        {
            Play2DSoundFX(burrowSFX);
        }
    }

    public virtual void PlayUnburrowSFX()
    {
        if (unburrowSFX != null)
        {
            Play2DSoundFX(unburrowSFX);
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
