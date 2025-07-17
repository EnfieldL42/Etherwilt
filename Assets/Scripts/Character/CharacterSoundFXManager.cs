using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Damage Grunts")]
    [SerializeField] protected AudioClip[] damageGrunt;

    [Header("Attack Grunts")]
    [SerializeField] protected AudioClip[] attackGrunt;

    [Header("Footsteps")]
    public AudioClip[] footsteps;
    //public AudioClip[] footstepsDirt;
    //public AudioClip[] footstepsStone;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void Update()
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
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunt));
        }
    }
    public virtual void PlayAttackGruntSoundFX()
    {
        if (damageGrunt.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunt));
        }
    }
    public virtual void PlayFootStepSoundFX()
    {
        if (footsteps.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footsteps));
        }
    }
}
