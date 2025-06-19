using System;
using System.Collections.Generic;
using UnityEngine;

public enum AudioClips
{
    BackgroundMusic,
    BackgroundAmbience,
    DuckQuack,
    CarHonk,
    YogaFail,
    MouseClick,
    BallKick,
    SceneSuccess,
    SceneFail,
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
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Audio Manager already exists in scene, probably the one from last scene carried over, this is fine.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeDictionaries();
    }

    private void Start()
    {
        PlayAudio(AudioClips.BackgroundMusic);
        PlayAudio(AudioClips.BackgroundAmbience);
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
                Debug.LogWarning($"Duplicate audio source name found: {source.audioSourceName}. Only the first one will be used.");
            }
        }
        
    }
    
    public void PlayAudio(AudioClips clipToPlay)
    {
        //add sounds here.
        switch (clipToPlay)
        {
            case AudioClips.BackgroundMusic:
                PlaySound(audioSourceDictionary[AudioSources.BackgroundMusic].source, audioClipDictionary[clipToPlay], true);
                break;
            case AudioClips.BackgroundAmbience:
                PlaySound(audioSourceDictionary[AudioSources.AmbienceMusic].source, audioClipDictionary[clipToPlay], true);
                break;
            case AudioClips.DuckQuack:
                PlaySound(audioSourceDictionary[AudioSources.SoundEffects].source, audioClipDictionary[clipToPlay]);
                break;
            case AudioClips.CarHonk:
                PlaySound(audioSourceDictionary[AudioSources.SoundEffects].source, audioClipDictionary[clipToPlay]);
                break;
            case AudioClips.YogaFail:
                PlaySound(audioSourceDictionary[AudioSources.SoundEffects].source, audioClipDictionary[clipToPlay]);
                break;
            case AudioClips.BallKick:
                PlaySound(audioSourceDictionary[AudioSources.SoundEffects].source, audioClipDictionary[clipToPlay]);
                break;
            case AudioClips.MouseClick:
                PlaySound(audioSourceDictionary[AudioSources.UI].source, audioClipDictionary[clipToPlay]);
                break;
            case AudioClips.SceneFail:
                PlaySound(audioSourceDictionary[AudioSources.SoundEffects].source, audioClipDictionary[clipToPlay]);
                break;
            case AudioClips.SceneSuccess:
                PlaySound(audioSourceDictionary[AudioSources.SoundEffects].source, audioClipDictionary[clipToPlay]);
                break;
        }
        
    }
    
    private void PlaySound(AudioSource audioSource, AudioClipObject clip, bool loops = false)
    {
        if (!loops)
        {
            audioSource.PlayOneShot(clip.clips[UnityEngine.Random.Range(0, clip.clips.Length)]);
        }
        else
        {
            audioSource.loop = true;
            audioSource.clip = clip.clips[0];
            audioSource.Play();
        }
    }
    
    public void StopSound(AudioSources audioSource)
    {
        audioSourceDictionary[audioSource].source.Stop();
    }
}

[Serializable]
public class AudioClipObject
{
    public AudioClips soundName;
    public AudioClip[] clips;
}

[Serializable]
public class AudioSourceObject
{
    public AudioSources audioSourceName;
    public AudioSource source;
    public bool loop = false; //an audio source that loops will only play one sound at a time, 2 looping sounds require 2 seperate sources.
}
