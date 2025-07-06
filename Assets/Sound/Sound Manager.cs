using System;
using System.Collections.Generic;
using UnityEngine;

public enum AudioClips // Important! NEVER change the value of each entry. The values are used so we can delete/shuffle stuff in the enum.(must be unique)
{
    NoClip= -1,
    //BGM
    BackgroundMusic=0,
    //Ambiences
    ParkAmbience=1,
    RestaurantAmbience=14,
    FountainAmbience=16,
    DuckAmbience=24,
    FootballAmbience=25, 
    // General/UI sounds
    MouseClick=5,
    SceneSuccess=7,
    ScissorsCut=9,
    TutorialPop=19,
    // scene start sounds
    CarHonk=3,
    BikeBellRing=11,
    RestaurantBell=13,
    FootballWhistle=15,
    YogaBowl = 17,
    // other scene sounds
    BallKick=6,
    DuckRunOver=10,
    BikeAccident=18,
    FootBallSuccess=20,
    FootBallFail=21,
    FootBallHe=22,
    YogaWin=23,
    //missing sounds - meaning we still need the clip from itai
    YogaFail=4,
    //not sure if we need this
    DuckQuack=2,
    //...
}

public enum AudioSources
{
    BackgroundMusic,
    SoundEffects,
    UI,
    AmbienceMusic
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioClipObject[] sounds;
    [SerializeField] private AudioSourceObject[] audioSources;
    private Dictionary<AudioClips, AudioClipObject> audioClipDictionary;
    private Dictionary<AudioSources, AudioSourceObject> audioSourceDictionary;
    public float musicVolume = 0.75f;
    public float soundEffectsVolume = 0.75f;
    private AudioClipObject currentBackgroundMusicClip = null;
    private AudioClipObject currentAmbienceClip = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log(
                "Audio Manager already exists in scene, probably the one from last scene carried over, this is fine.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeDictionaries();
        audioSourceDictionary[AudioSources.BackgroundMusic].source.loop = true;
        audioSourceDictionary[AudioSources.AmbienceMusic].source.loop = true;
    }

    private void Start()
    {
        PlayAudio(AudioClips.BackgroundMusic);
    }

    private void InitializeDictionaries()
    {
        audioClipDictionary = new Dictionary<AudioClips, AudioClipObject>();
        foreach (var clip in sounds)
        {
            if (!audioClipDictionary.ContainsKey(clip.soundName))
            {
                audioClipDictionary.Add(clip.soundName, clip);
            }
            else
            {
                Debug.LogWarning($"Duplicate sound name found: {clip.soundName}. Only the first one will be used.");
            }
        }

        audioSourceDictionary = new Dictionary<AudioSources, AudioSourceObject>();
        foreach (var source in audioSources)
        {
            if (!audioSourceDictionary.ContainsKey(source.audioSourceName))
            {
                audioSourceDictionary.Add(source.audioSourceName, source);
            }
            else
            {
                Debug.LogWarning(
                    $"Duplicate audio source name found: {source.audioSourceName}. Only the first one will be used.");
            }
        }
    }

    public void PlayAudio(AudioClips clipToPlay)
    {
        //add sounds here.
        switch (clipToPlay)
        {
            case AudioClips.NoClip:
                return;
            // BGM
            case AudioClips.BackgroundMusic:
                PlaySound(AudioSources.BackgroundMusic, clipToPlay);
                break;
            //Ambience / follies
            case AudioClips.ParkAmbience:
                PlaySound(AudioSources.AmbienceMusic, clipToPlay);
                break;
            // Sound Effects
            case AudioClips.DuckQuack:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.CarHonk:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.YogaFail:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.BallKick:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.DuckRunOver:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.BikeBellRing:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            // UI/Scene Sounds
            case AudioClips.MouseClick:
                PlaySound(AudioSources.UI, clipToPlay);
                break;
            case AudioClips.SceneSuccess:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.ScissorsCut:
                PlaySound(AudioSources.UI, clipToPlay);
                break;
            case AudioClips.RestaurantBell:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.RestaurantAmbience:
                PlaySound(AudioSources.AmbienceMusic, clipToPlay);
                break;
            case AudioClips.FootballWhistle:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.FountainAmbience:
                PlaySound(AudioSources.AmbienceMusic, clipToPlay);
                break;
            case AudioClips.DuckAmbience:
                PlaySound(AudioSources.AmbienceMusic, clipToPlay);
                break;
            case AudioClips.FootballAmbience:
                PlaySound(AudioSources.AmbienceMusic, clipToPlay);
                break;
            case AudioClips.YogaBowl:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.TutorialPop:
                PlaySound(AudioSources.UI, clipToPlay);
                break;
            case AudioClips.FootBallSuccess:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.FootBallFail:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.FootBallHe:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.YogaWin:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
            case AudioClips.BikeAccident:
                PlaySound(AudioSources.SoundEffects, clipToPlay);
                break;
        }
    }

    //function is hella ugly but i just need it to work
    private void PlaySound(AudioSources audioSource, AudioClips clip)
    {
        if (!audioSourceDictionary.ContainsKey(audioSource))
        {
            Debug.LogError($"Audio source {audioSource} is not properly included in the Sound Manager sources list.");
            return;
        }
        if(!audioClipDictionary.ContainsKey(clip) || audioClipDictionary[clip].clips.Length == 0 || audioClipDictionary[clip].clips[0] == null)
        {
            Debug.LogError($"Audio clip {clip} is not properly included in the Sound Manager sounds list.");
            return;
        }
        if (audioSource == AudioSources.BackgroundMusic || audioSource == AudioSources.AmbienceMusic)
        {
            if (audioSource == AudioSources.BackgroundMusic)
            {
                currentBackgroundMusicClip = audioClipDictionary[clip];
            }

            if (audioSource == AudioSources.AmbienceMusic)
            {
                currentAmbienceClip = audioClipDictionary[clip];
            }

            AudioSource source = audioSourceDictionary[audioSource].source;
            source.clip = audioClipDictionary[clip].clips[0];
            source.volume = audioClipDictionary[clip].volume * musicVolume;
            source.Play();
        }
        else //sound effect or ui
        {
            AudioSource source = audioSourceDictionary[audioSource].source;
            AudioClipObject audioClip = audioClipDictionary[clip];
            source.volume = audioClip.volume * soundEffectsVolume;
            source.PlayOneShot(audioClip.clips[UnityEngine.Random.Range(0, audioClip.clips.Length)]);
        }
    }

    public void StopSound(AudioSources audioSource)
    {
        audioSourceDictionary[audioSource].source.Stop();
        if (audioSource == AudioSources.BackgroundMusic)
            currentBackgroundMusicClip = null;
        if (audioSource == AudioSources.AmbienceMusic)
            currentAmbienceClip = null;
    }

    public void ChangeMusicVolume(float newVolume)
    {
        musicVolume = newVolume;
        if (currentBackgroundMusicClip != null)
        {
            audioSourceDictionary[AudioSources.BackgroundMusic].source.volume =
                currentBackgroundMusicClip.volume * musicVolume;
        }

        if (currentAmbienceClip != null)
        {
            audioSourceDictionary[AudioSources.AmbienceMusic].source.volume = currentAmbienceClip.volume * musicVolume;
        }
    }

    public void ChangeSoundEffectsVolume(float newVolume)
    {
        soundEffectsVolume = newVolume;
    }
}

[Serializable]
public class AudioClipObject
{
    public AudioClips soundName;
    public AudioClip[] clips;
    [Range(0f, 1f)] public float volume = 0.5f;
}

[Serializable]
public class AudioSourceObject
{
    public AudioSources audioSourceName;
    public AudioSource source;
}