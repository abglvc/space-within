using UnityEngine;

namespace Other {
    public class AudioManager : MonoBehaviour {
        public static AudioManager sng { get; private set; }
        private bool playSounds, playMusic;

        private AudioSource[] soundSources;
        private AudioSource musicSource;
        public AudioClip loopMusicTrack;

        void Awake() {
            if (sng == null) sng = this;
            else {
                Destroy(sng);
                sng = this;
            }

            Initialize();
        }

        private void Initialize() {
            soundSources = GetComponents<AudioSource>();
            musicSource = soundSources[soundSources.Length - 1];
        }

        public void PlayLoopMusicTrack(AudioClip music) {
            loopMusicTrack = music;
            musicSource.clip = loopMusicTrack;
            musicSource.Play();
        }

        public void Play2DSound(int sound) {
            if (playSounds) soundSources[sound].Play();
        }

        public void Stop2DSound(int sound) {
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

        public AudioSource MusicSource => musicSource;
    }
}