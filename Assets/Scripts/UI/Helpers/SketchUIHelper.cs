using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class SketchUIHelper : MonoBehaviour
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        
        private Sketch _sketchObject;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void DisableButtons()
        {
            confirmButton.gameObject.SetActive(false);
            cancelButton.gameObject.SetActive(false);
            confirmButton.transition = Selectable.Transition.None;
            cancelButton.transition = Selectable.Transition.None;
            confirmButton.interactable = false;
            cancelButton.interactable = false;
        }

        public void EnableButtons(Vector3 position)
        {
            confirmButton.transition = Selectable.Transition.ColorTint;
            cancelButton.transition = Selectable.Transition.ColorTint;
            confirmButton.interactable = true;
            cancelButton.interactable = true;
            confirmButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            
            transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(position.x, position.z);
        }

        public void SetSketchReference(Sketch sketch)
        {
            _sketchObject = sketch;
        }
        
        private void ConfirmPlacement()
        {
            _sketchObject.ConfirmPlacement();
        }

        private void CancelPlacement()
        {
            _sketchObject.CancelPlacement();
            DisableButtons();
        }

        public bool ButtonEnabled()
        {
            return confirmButton.isActiveAndEnabled;
        }
        
        private void AddListeners()
        {
            confirmButton.onClick.AddListener(ConfirmPlacement);
            cancelButton.onClick.AddListener(CancelPlacement);
        }

        private void RemoveListeners()
        {
            confirmButton.onClick.RemoveListener(ConfirmPlacement);
            cancelButton.onClick.RemoveListener(CancelPlacement);
        }
    }
}
