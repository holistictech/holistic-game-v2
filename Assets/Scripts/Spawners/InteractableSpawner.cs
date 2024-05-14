using System;
using System.Collections.Generic;
using Factory;
using GridSystem;
using Interactables;
using Scriptables;
using UI;
using UI.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using static Utilities.CommonFields;

namespace Spawners
{
    public class InteractableSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject spawnable;
        [SerializeField] private Sketch sketch;
        [SerializeField] private Transform objectParent;
        [SerializeField] private SwipeHandler swipeHandler;
        private GridController _gridController;
        private TaskConfig _currentConfig;

        [SerializeField] private InteractableConfig itemConfig;

        public static event Action<Sketch> OnPositionChoiceNeeded;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }
        
        public void InjectLogicBoard(GridController controller)
        {
            _gridController = controller;
        }

        private void SpawnSketch(TaskConfig config)
        {
            _currentConfig = config;
            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                var transform1 = mainCamera.transform;
                Vector3 cameraPosition = transform1.position;
                Vector3 cameraForward = transform1.forward;
            
                Vector3 middlePoint = cameraPosition + cameraForward * mainCamera.nearClipPlane;
                var spawnedSketch = Instantiate(sketch, objectParent);
                spawnedSketch.ConfigureObjectMesh(_currentConfig.RewardInteractable.ObjectMesh);
                spawnedSketch.transform.position = new Vector3(middlePoint.x, 0, 0);
                swipeHandler.enabled = true;
                OnPositionChoiceNeeded?.Invoke(spawnedSketch);
            }
        }

        private void SpawnInteractable()
        {
            var finalPos = swipeHandler.GetFinalPosition();
            SpawnSelectedInteractable(new CartesianPoint((int)finalPos.x, (int)finalPos.z), _currentConfig.RewardInteractable);
        }


        public void SpawnSelectedInteractable(CartesianPoint desiredPoint, InteractableConfig config)
        {
            var spawnedInstance = InteractableFactory.SpawnInstance(spawnable, config, objectParent);
            var interactable = spawnedInstance.GetComponent<InteractableObject>();

            interactable.InjectFields(_gridController, config);
            var buildingPlan = interactable.CalculateCoordinatesForBlocking(desiredPoint);
            if (interactable != null && _gridController.IsPlacementValid(buildingPlan))
            {
                interactable.BuildSelf(desiredPoint);
                swipeHandler.enabled = false;
                _currentConfig.SetHasCompleted(true);
            }
            else
            {
                Destroy(interactable.gameObject);
                Debug.LogError("Error while spawning building");
            }
        }

        private void CancelPlacementProcess()
        {
            swipeHandler.enabled = false;
        }

        private void AddListeners()
        {
            Task.OnTaskCompleted += SpawnSketch;
            Sketch.OnPlacementConfirmed += SpawnInteractable;
            Sketch.OnPlacementCancelled += CancelPlacementProcess;
            //GridSpawner.OnGridReady += InjectLogicBoard;
        }

        private void RemoveListeners()
        {
            Task.OnTaskCompleted -= SpawnSketch;
            Sketch.OnPlacementConfirmed -= SpawnInteractable;
            Sketch.OnPlacementCancelled -= CancelPlacementProcess;
            //GridSpawner.OnGridReady -= InjectLogicBoard;
        }
        
    }
}
