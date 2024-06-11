using System;
using DG.Tweening;
using UnityEngine;

namespace Utilities
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource source;

        private static AudioManager _instance;

        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();

                    if (_instance == null)
                    {
                        GameObject singleton = new GameObject(nameof(AudioManager));
                        _instance = singleton.AddComponent<AudioManager>();
                    }
                }

                return _instance;
            }
        }
        
        protected void Awake()
        {
            if (_instance == null)
            {
                _instance = this as AudioManager;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlayAudioClip(AudioClip clip)
        {
            if (clip == null) return;
            source.PlayOneShot(clip);
        }
    }
}
