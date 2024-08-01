using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables;
using Scriptables.Tutorial;
using Spans.Skeleton;
using TMPro;
using Tutorial;
using UI.Helpers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace UI.Tasks
{
    public class TasksPanel : MonoBehaviour, ITutorialElement
    {
        [Header("UI Attributes")] 
        [SerializeField] private RectTransform taskContent;
        [SerializeField] private GameObject taskPanel;
        [SerializeField] private TextMeshProUGUI headerField;
        [SerializeField] private Button taskButton;
        [SerializeField] private Button marketButton;
        [SerializeField] private Button closeButton;

        [Header("Functionality")]
        [SerializeField] private WarningUIHelper warningHelper;
        [SerializeField] private Task taskPrefab;
        [SerializeField] private TutorialManager tutorialManager;
        [SerializeField] private List<TutorialStep> tutorialSteps;
        [SerializeField] private List<GameObject> tutorialObjects;
        [SerializeField] private TutorialStep taskStepConfig;
        private string _tutorialKey = "TaskPanel";
        private ITutorialElement _tutorialElement;

        private List<Task> _spawnedTasks = new List<Task>();
        
        private void OnEnable()
        {
            AddListeners();
            
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void TriggerTutorial(GameLoadingEvent eventData)
        {
            if (!eventData.HasFinished) return;
            _tutorialElement = this;
            taskButton.interactable = false;
            taskButton.transition = Selectable.Transition.None;
            if (!_tutorialElement.CanShowStep(_tutorialKey))
            {
                taskButton.interactable = true;
                taskButton.transition = Selectable.Transition.ColorTint;
                return;
            }
            EventBus.Instance.Trigger(new TutorialEvent(false));
            var highlightDictionary =
                new Dictionary<GameObject, TutorialStep>().CreateFromLists(tutorialObjects, tutorialSteps);
            TryShowTutorial(highlightDictionary, taskButton.GetComponent<RectTransform>(), 0);
        }

        private void InstantiateTasks(bool type)
        {
            List<TaskConfig> tasks = PlayerInventory.Instance.GetTasksByType(type);
            
            foreach (var task in tasks)
            {
                var temp = Instantiate(taskPrefab, taskContent);
                _spawnedTasks.Add(temp);
                temp.ConfigureUI(task, this);
            }
            
        }

        public void EnableTaskPopup(bool type)
        {
            if (taskPanel.activeSelf) return;
            EventBus.Instance.Trigger(new ToggleSwipeInput(false));
            //taskButton.gameObject.SetActive(false);
            taskPanel.gameObject.SetActive(true);
            headerField.text = type ? "Görevler" : "Araçlar";
            InstantiateTasks(type);
            if (_waitInput)
            {
                closeButton.interactable = false;
                ResetCoroutine();
                List<GameObject> goButton = new List<GameObject>() { _spawnedTasks[0].gameObject};
                List<TutorialStep> goStep = new List<TutorialStep>() { taskStepConfig };
                var goHighlight = new Dictionary<GameObject, TutorialStep>().CreateFromLists(goButton, goStep);
                TryShowTutorial(goHighlight, GetComponent<RectTransform>(), -130);
                EventBus.Instance.Trigger(new TutorialEvent(true));
                PlayerSaveManager.SavePlayerAttribute(1, _tutorialKey);
            }
        }

        public void TriggerWarningHelper(TaskConfig config)
        {
            warningHelper.ConfigurePopup(config);
        }

        public void DisableTaskPopup()
        {
            EventBus.Instance.Trigger(new ToggleSwipeInput(true));
            taskButton.gameObject.SetActive(true);
            taskPanel.gameObject.SetActive(false);
            _spawnedTasks.Clear();
            DestroyTasks();
        }

        public void RequestSpan()
        {
            DisableTaskPopup();
            EventBus.Instance.Trigger(new SpanRequestedEvent());
        }

        private void DestroyTasks()
        {
            var tasks = taskContent.GetComponentsInChildren<Task>().ToList();
            foreach (var task in tasks)
            {
                Destroy(task.gameObject);
            }
        }
        
        private bool _waitInput;
        private Coroutine _waitingInput;
        public void TryShowTutorial(Dictionary<GameObject, TutorialStep> highlights, RectTransform finalHighlight, float offset)
        {
            tutorialManager.ActivateStateTutorial(highlights, true, () =>
            {
                _waitInput = true;
                _waitingInput = StartCoroutine(WaitInput(finalHighlight, offset));
            }, false);
        }

        public IEnumerator WaitInput(RectTransform finalHighlight, float offset)
        {
            tutorialManager.AnimateSpawnedHand();
            taskButton.interactable = true;
            taskButton.transition = Selectable.Transition.ColorTint;
            yield return new WaitUntil(() => !_waitInput);
            closeButton.interactable = true;
        }

        private void ResetCoroutine()
        {
            StopCoroutine(_waitingInput);
            _waitInput = false;
            tutorialManager.ClearHighlights();
        }

        private void AddListeners()
        {
            closeButton.onClick.AddListener(DisableTaskPopup);
            WarningUIHelper.OnRedirectToTask += EnableTaskPopup;
            EventBus.Instance.Register<GameLoadingEvent>(TriggerTutorial);
        }

        private void RemoveListeners()
        {
            closeButton.onClick.RemoveListener(DisableTaskPopup);
            WarningUIHelper.OnRedirectToTask -= EnableTaskPopup;
            EventBus.Instance.Unregister<GameLoadingEvent>(TriggerTutorial);
        }
    }
}
