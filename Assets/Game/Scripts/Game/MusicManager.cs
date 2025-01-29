using System;
using UnityEngine;

namespace GameJammers.GGJ2025.Audio {
    public class MusicManager: MonoBehaviour {
        FMOD.Studio.EventInstance _gameMusic;

        public static MusicManager Instance;
        void Start () {

            if (Instance != null) {
                this.enabled = false;
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _gameMusic = FMODUnity.RuntimeManager.CreateInstance("Event:/GameMusic");
            _gameMusic.start();
        }

        public void UpdateTrack (int track) {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("Track", track);
        }

        public void PlaySound (string soundName) {
            FMODUnity.RuntimeManager.PlayOneShot(soundName);
        }

        public void PauseMusic () {
            _gameMusic.setPaused(true);
        }

        public void ResumeMusic () {
            _gameMusic.setPaused(false);
        }
    }
}
