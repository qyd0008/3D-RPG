using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AudioManager : Singleton<AudioManager>
{
    //音效播放器
    public AudioSource SoundPlayer;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    //播放音效
    public void PlaySound(string name)
    {
        AudioClip clip = Resources.Load<AudioClip>(name);
        SoundPlayer.clip = clip;
        SoundPlayer.PlayOneShot(clip);
    }

}
