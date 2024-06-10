using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    public class DayDoneUIHelper : MonoBehaviour
    {
        [SerializeField] private GameObject informPopup;
        [SerializeField] private Button closeButton;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void EnableSelf()
        {
            informPopup.gameObject.SetActive(true);
        }

        private void DisableSelf()
        {
            informPopup.gameObject.SetActive(false);
        }

        private void AddListeners()
        {
            closeButton.onClick.AddListener(DisableSelf);
        }

        private void RemoveListeners()
        {
            closeButton.onClick.RemoveListener(DisableSelf);
        }
    }
}
