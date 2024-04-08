using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class PermissionRequester : MonoBehaviour
    {
        private void Start()
        {
            if (Microphone.devices.Length == 0)
            {
                Debug.LogError("No available microphone on device");
                return;
            }

            StartCoroutine(RequestMicrophoneAccess());
        }

        private IEnumerator RequestMicrophoneAccess()
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
            if (Application.HasUserAuthorization(UserAuthorization.Microphone))
            {
                Debug.Log("Microphone permission granted.");
            }
            else
            {
                Debug.LogError("Microphone permission denied.");
            }
        }
    }
}
