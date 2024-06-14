using System;
using System.Collections.Generic;
using System.Linq;
using Scriptables;
using TMPro;
using UI.Helpers;
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
        [SerializeField] private TextMeshProUGUI headerField;
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

        private void InstantiateTasks(bool type)
        {
            List<TaskConfig> tasks = PlayerInventory.Instance.GetTasksByType(type);
            
            foreach (var task in tasks)
            {
                var temp = Instantiate(taskPrefab, taskContent);
                temp.ConfigureUI(task, this);
            }
        }

        public void EnableTaskPopup(bool type)
        {
            if (taskPanel.activeSelf) return;
            taskButton.gameObject.SetActive(false);
            taskPanel.gameObject.SetActive(true);
            headerField.text = type ? "Görevler" : "Araçlar";
            InstantiateTasks(type);
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
            //taskButton.onClick.AddListener(EnableTaskPopup);
            closeButton.onClick.AddListener(DisableTaskPopup);
            WarningUIHelper.OnRedirectToTask += EnableTaskPopup;
        }

        private void RemoveListeners()
        {
            //taskButton.onClick.RemoveListener(EnableTaskPopup);
            closeButton.onClick.RemoveListener(DisableTaskPopup);
            WarningUIHelper.OnRedirectToTask -= EnableTaskPopup;
        }
    }
}
