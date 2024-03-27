using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tasks
{
    public class TasksPanel : MonoBehaviour
    {
        [Header("UI Attributes")]
        [SerializeField] private GameObject _taskPanel;
        [SerializeField] private Button _taskButton;
        [SerializeField] private Button _closeButton;

        [Header("Functionality")] 
        [SerializeField] private Task _taskPrefab;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void InstantiateTasks()
        {
            
        }

        private void EnableTaskPopup()
        {
            _taskPanel.gameObject.SetActive(true);
        }

        private void DisableTaskPopup()
        {
            _taskPanel.gameObject.SetActive(false);
        }

        private void AddListeners()
        {
            _taskButton.onClick.AddListener(EnableTaskPopup);
            _closeButton.onClick.AddListener(DisableTaskPopup);
        }

        private void RemoveListeners()
        {
            _taskButton.onClick.RemoveListener(EnableTaskPopup);
            _closeButton.onClick.RemoveListener(DisableTaskPopup);
        }
    }
}
