using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    public AudioSource audioSource;

    [Header("Damage Grunts")]
    [SerializeField] protected AudioClip[] damageGrunt;

    [Header("Attack Grunts")]
    [SerializeField] protected AudioClip[] attackGrunt;

    [Header("Footsteps")]
    public AudioClip[] footsteps;
    //public AudioClip[] footstepsDirt;
    //public AudioClip[] footstepsStone;

    [Header("Death")]
    public AudioClip deathSFX;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
    {
    }

    protected virtual void Start()
    {
    }

    public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
    {
        audioSource.PlayOneShot(soundFX, volume);
        audioSource.pitch = 1;
        if (randomizePitch)
        {
            audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
        }
    }

    public void PlayRollSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
    }
    public void PlayBackStepSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.backStepSFX);
    }
    public virtual void PlayDamageGruntSoundFX()
    {
        if(damageGrunt.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunt), 1);
        }
    }
    public virtual void PlayAttackGruntSoundFX()
    {
        if (damageGrunt.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunt), 1);
        }
    }
    public virtual void PlayFootStepSoundFX()
    {
        if (footsteps.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footsteps), 1, false);
        }

        //OR
        //USES TAGS TO DETERMINE TYPE OF FOOTSTEP SFX
        //audioSource.PlayOneShot(WorldSoundFXManager.instance.ChooseRandomFootStepSoundBasedOnGround(steppedOnObject, character));
    }
    public virtual void PlayBlockSoundFX()
    {

    }
    public virtual void PlayStanceBreakSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.stanceBreakSFX);
    }
    public virtual void PlayCriticalStrikeSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.criticalStikeSFX);
    }

    public virtual void PlayDeathSoundFX()
    {
        if (deathSFX != null)
        {
            audioSource.PlayOneShot(deathSFX);

        }
    }
}
