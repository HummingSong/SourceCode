using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : PSManager
{
    private AudioSource bgmAudio;

    private float bgmPlayTime = 0f;

    private bool stopLoopSoundEffect = false;

    public override IEnumerator ManagerInitProcessing()
    {
        yield return StartCoroutine(InitManager());

        yield return StartCoroutine(base.ManagerInitProcessing());
    }

    public override IEnumerator InitManager()
    {
        bgmAudio = GetComponent<AudioSource>();

        yield return StartCoroutine(LoadSoundEx());

        yield return StartCoroutine(base.InitManager());
    }

    public void PlayBGM(AudioClip clip, AudioMixerGroup mixer, bool fade = false)
    {
        bgmAudio.clip = clip;
        bgmAudio.loop = true;
        bgmAudio.outputAudioMixerGroup = mixer;

        if (fade)
            bgmAudio.volume = 0;

        bgmAudio.Play();
    }

    public void StopBGM()
    {
        bgmAudio.loop = false;

        bgmAudio.Stop();
    }

    public void SetBGMVolume(float vol)
    {
        bgmAudio.volume = 1f * vol;
    }

    public void PauseBGM()
    {
        bgmPlayTime = bgmAudio.time;
        bgmAudio.Stop();
    }

    public void ResumeBGM()
    {
        bgmAudio.time = bgmPlayTime;
        bgmAudio.Play();
    }

    public void PlayEndBGM(AudioClip clip, AudioMixerGroup mixer, bool fade = false)
    {
        bgmAudio.clip = clip;
        bgmAudio.loop = false;
        bgmAudio.outputAudioMixerGroup = mixer;

        if (fade)
            bgmAudio.volume = 0;

        bgmAudio.Play();
    }

    public void PlayEffectSound(AudioSource source, AudioMixerGroup mixer, AudioClip clip, float volume)
    {
        source.loop = false;
        source.volume = volume;
        source.outputAudioMixerGroup = mixer;

        source.PlayOneShot(clip);
    }

    public void PlayEffectSound(AudioSource source, string mixer, string clip, float volume)
    {
        AudioMixerGroup audioMixerGroup = null;

        source.loop = false;
        source.volume = volume;
        source.outputAudioMixerGroup = audioMixerGroup;
    }

    public void StopEffectSound(AudioSource source)
    {
        source.Stop();
    }

    public IEnumerator LoadSoundEx()
    { }
        yield return true;
    }

    public IEnumerator FadeInBGM(float interval)
    {
        if (bgmAudio.volume == 0)
            yield break;

        if (interval == 0)
            yield break;

        float speed = bgmAudio.volume / interval;
        float originVol = bgmAudio.volume;
        bgmAudio.volume = 0f;

        bgmAudio.Play();

        while (bgmAudio.volume < originVol)
        {
            bgmAudio.volume += speed * Time.deltaTime;

            if(bgmAudio.volume >= originVol)
            {
                bgmAudio.volume = originVol;
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public IEnumerator FadeOutBGM(float interval)
    {
        if (bgmAudio.volume == 0)
            yield break;

        if (interval == 0)
            yield break;

        float speed = bgmAudio.volume / interval;
        while (bgmAudio.volume > 0)
        {
            bgmAudio.volume -= speed * Time.deltaTime;

            if (bgmAudio.volume <= 0)
            {
                bgmAudio.volume = 0f;

                bgmAudio.Stop();
                yield break;
            }

            yield return new WaitForFixedUpdate();
        }
    }

    public void PlayEffectSoundSelf(GameObject obj, string mixer, string clip, float volume, bool loop, bool selfDestroy)
    {
      
    }

    public IEnumerator SoundExSelf(GameObject obj, AudioMixerGroup mixer, AudioClip clip, float volume, bool loop, bool selfDestroy)
    {
        AudioSource audio = obj.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = loop;
        audio.outputAudioMixerGroup = mixer;

        audio.volume = volume;

        audio.PlayOneShot(clip);

        yield return new WaitUntil(() => !audio.isPlaying);

        if (selfDestroy)
            Destroy(audio);

        yield return true;
    }

    public void PlayEffectSoundLoop(GameObject obj, string mixer, string clip, float volume, bool loop, bool selfDestroy)
    {
    }

    public void StopEffectSoundLoop()
    {
        stopLoopSoundEffect = true;
    }

    public IEnumerator SoundExLoop(GameObject obj, AudioMixerGroup mixer, AudioClip clip, float volume, bool loop, bool selfDestroy)
    {
        stopLoopSoundEffect = false;

        AudioSource audio = obj.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = loop;

        audio.clip = clip;
        audio.outputAudioMixerGroup = mixer;

        audio.volume = volume;
        
        audio.Play();

        yield return new WaitUntil(() => stopLoopSoundEffect);

        if (selfDestroy)
            Destroy(audio);

        yield return true;
    }
}
