using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CorsiBlock : MonoBehaviour
    {
        [SerializeField] private Image blockImage;
        [SerializeField] private Button blockButton;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void SetSelected()
        {
            
        }

        private void AddListeners()
        {
            blockButton.onClick.AddListener(SetSelected);
        }

        private void RemoveListeners()
        {
            blockButton.onClick.RemoveListener(SetSelected);
        }
    }
}
