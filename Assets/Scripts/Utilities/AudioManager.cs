using System;
using DG.Tweening;
using UnityEngine;

namespace Utilities
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource source;

        public void PlayAudioClip(AudioClip clip)
        {
            source.PlayOneShot(clip);
        }
    }
}
