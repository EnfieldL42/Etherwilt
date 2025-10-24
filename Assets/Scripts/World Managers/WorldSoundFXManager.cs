using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [SerializeField] AudioClip uiPressToStart;
    [SerializeField] AudioClip uiSlider;
    [SerializeField] AudioClip uiOpenMenu;
    [SerializeField] AudioClip tutorialOpenPopUp;

    [Header("Boss Fight Defeated")]
    [SerializeField] AudioClip bossDefeatedSFX;

    [Header("Main Menu")]
    [SerializeField] AudioSource mainMenuSource;

    [Header("Ether Pickup SFX")]
    [SerializeField] AudioClip pickupSFX;


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
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        // Always unsubscribe to prevent memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
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
        if (uiConfirm == null)
            return;
        audioSource.PlayOneShot(uiConfirm, volume);
    }

    public void PlayUIReturnSound(float volume = 1)
    {
        if (uiReturn == null)
            return;
        audioSource.PlayOneShot(uiReturn, volume);
    }

    public void PlayUIStartGameSound(float volume = 1)
    {
        if (uiStartGame == null)
            return;
        audioSource.PlayOneShot(uiStartGame, volume);
    }

    public void PlayUIPressToStartGameSound(float volume = 1)
    {
        if (uiPressToStart == null)
            return;
        audioSource.PlayOneShot(uiPressToStart, volume);
    }

    public void PlayBossDefeatedSound(float volume = 1)
    {
        if (bossDefeatedSFX == null)
            return;
        audioSource.PlayOneShot(bossDefeatedSFX, volume);
    }

    public void PlayUISliderSound(float volume = 1)
    {
        if (uiSlider == null)
            return;
        audioSource.PlayOneShot(uiSlider, volume);
    }

    public void PlayTutorialPopUpSound(float volume = 1)
    {
        if (tutorialOpenPopUp == null)
            return;
        audioSource.PlayOneShot(tutorialOpenPopUp, volume);
    }

    public void PlayOpenMenuSound(float volume = 1)
    {
        if (uiOpenMenu == null)
            return;
        audioSource.PlayOneShot(uiOpenMenu, volume);
    }

    public void PlayEtherPickSound(float volume = 1)
    {
        if (pickupSFX == null)
            return;
        audioSource.PlayOneShot(pickupSFX, volume);
    }

    public void SetMainMenuMusicVolume()
    {
        StartCoroutine(FadeMainMenuMusic());
    }

    IEnumerator FadeMainMenuMusic()
    {
        while (mainMenuSource.volume > 0.1f)
        {
            mainMenuSource.volume -= Time.deltaTime;
            yield return null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            mainMenuSource = GetComponent<AudioSource>();
            mainMenuSource.volume = 1;

            bossIntroPlayer.Stop();
            bossLoopPlayer.Stop();
        }
    }
}
