using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace UI.Tasks
{
    public class TasksPanel : MonoBehaviour
    {
        [Header("UI Attributes")] 
        [SerializeField] private RectTransform taskContent;
        [SerializeField] private GameObject taskPanel;
        [SerializeField] private Button taskButton;
        [SerializeField] private Button closeButton;
        
        [Header("Functionality")] 
        [SerializeField] private Task taskPrefab;

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
            List<TaskConfig> tasks = PlayerInventory.Instance.GetAvailableTasks();

            foreach (var task in tasks)
            {
                var temp = Instantiate(taskPrefab, taskContent);
                temp.ConfigureUI(task, this);
            }
        }

        private void EnableTaskPopup()
        {
            if (taskPanel.activeSelf) return;
            taskButton.gameObject.SetActive(false);
            taskPanel.gameObject.SetActive(true);
            InstantiateTasks();
        }

        public void DisableTaskPopup()
        {
            taskButton.gameObject.SetActive(true);
            taskPanel.gameObject.SetActive(false);
            DestroyTasks();
        }

        private void DestroyTasks()
        {
            var tasks = taskContent.GetComponentsInChildren<Task>().ToList();
            foreach (var task in tasks)
            {
                Destroy(task.gameObject);
            }
        }

        private void AddListeners()
        {
            taskButton.onClick.AddListener(EnableTaskPopup);
            closeButton.onClick.AddListener(DisableTaskPopup);
        }

        private void RemoveListeners()
        {
            taskButton.onClick.RemoveListener(EnableTaskPopup);
            closeButton.onClick.RemoveListener(DisableTaskPopup);
        }
    }
}
