using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class MainUIHelper : MonoBehaviour
    {
        [SerializeField] private Button playButton;

        [SerializeField] private GameObject forwardSpan;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void TESTPlaySpan()
        {
            forwardSpan.gameObject.SetActive(true);
        }

        private void AddListeners()
        {
            playButton.onClick.AddListener(TESTPlaySpan);
        }

        private void RemoveListeners()
        {
            playButton.onClick.RemoveListener(TESTPlaySpan);
        }
    }
}
