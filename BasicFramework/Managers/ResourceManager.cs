using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ResourceManager : PSManager
{
    
    public override IEnumerator ManagerInitProcessing()
    {
        yield return StartCoroutine(InitManager());

        yield return StartCoroutine(base.ManagerInitProcessing());
    }

    public override IEnumerator InitManager()
    {
        yield return StartCoroutine(base.InitManager());
    }

    public AudioClip LoadAudioClip(string path, string name)
    {
        return Resources.Load(path + name) as AudioClip;
    }

    public AudioMixerGroup LoadAudioMixer(string path, string mixer, string name)
    {
        return (Resources.Load(path + mixer) as AudioMixer).FindMatchingGroups(name)[0];
    }
}