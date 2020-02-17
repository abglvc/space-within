using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager s_inst{get; private set;}
    private AudioSource[] a2Dsrc;

    void Awake(){
        if(s_inst==null) s_inst=this;
        else{
            Destroy(s_inst);
            s_inst = this;
        }
        a2Dsrc=GetComponents<AudioSource>();
    }
    
    public void Play2DSound(int sound){
        if(!AudioListener.pause) a2Dsrc[sound].Play();
    }
    
    public void Stop2DSound(int sound){
        a2Dsrc[sound].Stop();
    }
    
    public bool PlaySounds{
        set{AudioListener.pause=!value;}
    }
}