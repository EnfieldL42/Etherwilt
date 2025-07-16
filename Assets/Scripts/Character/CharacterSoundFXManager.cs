using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Damage Grunts")]
    [SerializeField] protected AudioClip[] damageGrunt;

    [Header("Attack Grunts")]
    [SerializeField] protected AudioClip[] attackGrunt;

    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayRollSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
    }

    public void PlayBackStepSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.backStepSFX);
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

    public virtual void PlayDamageGrunt()
    {
        if(damageGrunt.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunt));
        }
    }

    public virtual void PlayAttackGrunt()
    {
        if (damageGrunt.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunt));
        }
    }

}
