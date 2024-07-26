using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Scriptables.Tutorial;
using Tutorial;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace UI.Helpers
{
    public class SketchUIHelper : MonoBehaviour, ITutorialElement
    {
        [SerializeField] private List<GameObject> highlightObjects;
        [SerializeField] private List<TutorialStep> tutorialSteps;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button rotateButton;
        [SerializeField] private Button cancelButton;

        private TutorialManager _tutorialManager; 
        private ITutorialElement _tutorialElement;
        private string _tutorialKey = "PlacementTutorial";
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
            rotateButton.gameObject.SetActive(false);
        }

        public void EnableButtons(Vector3 position)
        {
            confirmButton.transition = Selectable.Transition.ColorTint;
            cancelButton.transition = Selectable.Transition.ColorTint;
            confirmButton.interactable = true;
            cancelButton.interactable = true;
            rotateButton.interactable = _sketchObject.GetRotatableStatus();
            confirmButton.gameObject.SetActive(true);
            cancelButton.gameObject.SetActive(true);
            rotateButton.gameObject.SetActive(_sketchObject.GetRotatableStatus());
            
            transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(position.x, position.z);
        }

        public void GetTutorialManager()
        {
            _tutorialManager = _sketchObject.GetTutorialManager();
        }

        public void SetSketchReference(Sketch sketch)
        {
            _sketchObject = sketch;
            ConfigureTutorial();
        }
        
        private void ConfirmPlacement()
        {
            if(_tutorialManager != null)
                _tutorialManager.ClearHighlights();
            _sketchObject.ConfirmPlacement();
        }

        private void CancelPlacement()
        {
            _sketchObject.CancelPlacement();
            DisableButtons();
        }

        private void RotateSketch()
        {
            _sketchObject.RotateSelf();
        }

        public bool ButtonEnabled()
        {
            return confirmButton.isActiveAndEnabled;
        }

        private void ConfigureTutorial()
        {
            _tutorialElement = this;
            if (!_tutorialElement.CanShowStep(_tutorialKey)) return;
            GetTutorialManager();
            var dictionary = new Dictionary<GameObject, TutorialStep>().CreateFromLists(highlightObjects, tutorialSteps);
            TryShowTutorial(dictionary, confirmButton.GetComponent<RectTransform>(), 0);
        }
        
        public void TryShowTutorial(Dictionary<GameObject, TutorialStep> highlights, RectTransform finalHighlight, float offset)
        {
            _tutorialManager.ActivateStateTutorial(highlights, false, () =>
            {
                _tutorialElement.SetStepCompleted(_tutorialKey);
            });
        }

        public IEnumerator WaitInput(RectTransform finalHighlight, float offset)
        {
            throw new NotImplementedException();
        }
        
        private void AddListeners()
        {
            confirmButton.onClick.AddListener(ConfirmPlacement);
            cancelButton.onClick.AddListener(CancelPlacement);
            rotateButton.onClick.AddListener(RotateSketch);
        }

        private void RemoveListeners()
        {
            confirmButton.onClick.RemoveListener(ConfirmPlacement);
            cancelButton.onClick.RemoveListener(CancelPlacement);
            rotateButton.onClick.RemoveListener(RotateSketch);
        }
    }
}
