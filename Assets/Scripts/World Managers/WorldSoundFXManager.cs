using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;

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


    [Header("UI SFX")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip uiSwitch;
    [SerializeField] AudioClip uiReturn;
    [SerializeField] AudioClip uiStartGame;
    [SerializeField] AudioClip uiConfirm;

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


    public void AlertNearbyCharactersToSound(Vector3 positionOfSound, float rangeOfSound)
    {
        if (!NetworkManager.Singleton.IsServer)
        {
            return;
        }

        Collider[] characterColliders = Physics.OverlapSphere(positionOfSound, rangeOfSound, WorldUtilityManager.instance.GetCharacterLayers());
        List<AICharacterManager> charactersToAlert = new List<AICharacterManager>();

        for (int i = 0; i < characterColliders.Length; i++)
        {
            AICharacterManager aiCharacter = characterColliders[i].GetComponent<AICharacterManager>();

            if (aiCharacter == null)
            {
                continue;
            }

            if (charactersToAlert.Contains(aiCharacter))
            {
                continue;
            }

            charactersToAlert.Add(aiCharacter);
        }

        for (int i = 0; i < charactersToAlert.Count; i++)
        {
            charactersToAlert[i].aICharacterCombatManager.AlertCharacterToSound(positionOfSound);
        }
    }


    public void PlayUISwitchSound(float volume = 1)
    {
        if (uiSwitch == null)
            return;
        audioSource.PlayOneShot(uiSwitch, volume);
    }

    public void PlayUIConfirmSound(float volume = 1)
    {
        if (uiSwitch == null)
            return;
        audioSource.PlayOneShot(uiConfirm, volume);
    }

    public void PlayUIReturnSound(float volume = 1)
    {
        if (uiSwitch == null)
            return;
        audioSource.PlayOneShot(uiReturn, volume);
    }

    public void PlayUIStartGameSound(float volume = 1)
    {
        if (uiSwitch == null)
            return;
        audioSource.PlayOneShot(uiStartGame, volume);
    }
}
