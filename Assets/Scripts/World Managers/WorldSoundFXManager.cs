using System.Collections;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;

    [Header("Boss Track")]
    [SerializeField] AudioSource bossIntroPlayer;
    [SerializeField] AudioSource bossLoopPlayer;

    [Header("Damage Sounds")]
    public AudioClip[] physicalDamageSFX;


    [Header("Action Sounds")]
    public AudioClip rollSFX;
    public AudioClip backStepSFX;
    public AudioClip pickUpItemSFX;
    public AudioClip stanceBreakSFX;
    public AudioClip criticalStikeSFX;
    public AudioClip healingFlaskSFX;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void PlayBossTrack(AudioClip introTrack, AudioClip loopTrack)
    {
        bossIntroPlayer.volume = 1;
        bossIntroPlayer.clip = introTrack;
        bossIntroPlayer.loop = false;
        bossIntroPlayer.Play();

        bossLoopPlayer.volume = 1;
        bossLoopPlayer.clip = loopTrack;
        bossLoopPlayer.loop = true;
        bossLoopPlayer.PlayDelayed(bossIntroPlayer.clip.length);
    }

    public void StopBossMusic()
    {
        StartCoroutine(FadeOutBossMusicThenStop());
    }

    private IEnumerator FadeOutBossMusicThenStop()
    {

        while(bossLoopPlayer.volume > 0f)
        {
            bossLoopPlayer.volume -= Time.deltaTime;
            bossIntroPlayer.volume -= Time.deltaTime;
            yield return null;
        }

        bossIntroPlayer.Stop();
        bossLoopPlayer.Stop();
    }

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
    { 
        int index = Random.Range(0, array.Length);
        
        return array[index];
    }

    //public AudioClip ChooseRandomFootStepSoundBasedOnGround(GameObject steppedOnObject, CharacterManager character)
    //{
    //    if(steppedOnObject.tag == "Dirt")
    //    {
    //        return ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepsDirt);
    //    }
    //    else if (steppedOnObject.tag == "Stone")
    //    {
    //        return ChooseRandomSFXFromArray(character.characterSoundFXManager.footstepsStone);
    //    }

    //    return null;
    //}


}
