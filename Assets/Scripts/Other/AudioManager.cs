using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager s_inst{get; private set;}
    private bool playSounds, playMusic;
    
    private AudioSource[] soundSources;
    private AudioSource musicSource;
    
    void Awake(){
        if(s_inst==null) s_inst=this;
        else{
            Destroy(s_inst);
            s_inst = this;
        }
        soundSources = GetComponents<AudioSource>();
        musicSource = soundSources[soundSources.Length - 1];
    }
    
    public void Play2DSound(int sound){
        if(playSounds) soundSources[sound].Play();
    }

    public void Stop2DSound(int sound){
        soundSources[sound].Stop();
    }

    public bool PlayMusic {
        get => playMusic;
        set {
            playMusic = value;
            musicSource.mute = !playMusic;
        }
    }

    public bool PlaySounds {
        get => playSounds;
        set => playSounds = value;
    }
}