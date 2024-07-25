using System;
using System.Collections.Generic;
using Factory;
using GridSystem;
using Interactables;
using Scriptables;
using Spans.Skeleton;
using UI;
using UI.Helpers;
using UI.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Utilities.Helpers;
using static Utilities.Helpers.CommonFields;
using EventBus = Spans.Skeleton.EventBus;

namespace Spawners
{
    public class InteractableSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject spawnable;
        [SerializeField] private Sketch sketch;
        [SerializeField] private Transform objectParent;
        [SerializeField] private SwipeHandler swipeHandler;
        [SerializeField] private ParticleSystem buildingEffect;
        [SerializeField] private AudioClip buildingSFX;

        [SerializeField] private WarningUIHelper warningHelper;
        private GridController _gridController;
        private TaskConfig _currentConfig;
        private Sketch _spawnedSketch;
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
                // Convert screen center to ray
                Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));

                // Use a plane at Y = 0 (ground plane)
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

                // Find where the ray intersects the ground plane
                if (groundPlane.Raycast(ray, out float distance))
                {
                    Vector3 worldCenter = ray.GetPoint(distance);

                    // Instantiate and configure the sketch object
                    _spawnedSketch = Instantiate(sketch, objectParent);
                    _spawnedSketch.ConfigureObjectMesh(
                        MeshContainer.Instance.GetMeshById(_currentConfig.RewardInteractable.MeshId),
                        _currentConfig.Rotatable);
                    _spawnedSketch.transform.position = worldCenter;
                    _spawnedSketch.ConfigureSize(_currentConfig.RewardInteractable);

                    // Enable swipe handling and invoke event
                    swipeHandler.enabled = true;
                    OnPositionChoiceNeeded?.Invoke(_spawnedSketch);
                }
            }
        }

        private void SpawnInteractable(Quaternion rotation)
        {
            var finalPos = swipeHandler.GetFinalPosition();
            SpawnSelectedInteractable(new CartesianPoint((int)finalPos.x, (int)finalPos.z),
                _currentConfig.RewardInteractable, true, rotation);
            PlayerInventory.Instance.ChangeCurrencyAmountByType(_currentConfig);
        }

        public void SpawnSelectedInteractable(CartesianPoint desiredPoint, InteractableConfig config, bool shouldSave,
            Quaternion rotation)
        {
            DisableSwipeHandler();
            var spawnedInstance = InteractableFactory.SpawnInstance(spawnable, config, objectParent);
            var interactable = spawnedInstance.GetComponent<InteractableObject>();

            interactable.InjectFields(_gridController, config);
            var buildingPlan = interactable.InteractableConfig.InteractableType == InteractableType.Plant
                ? new List<CartesianPoint> { desiredPoint }
                : interactable.CalculateCoordinatesForBlocking(desiredPoint);
            if (interactable != null && _gridController.IsPlacementValid(buildingPlan, config.InteractableType))
            {
                if (shouldSave)
                {
                    _spawnedSketch.DestroyObject();
                    PlayBuildingEffect(desiredPoint);
                }

                interactable.BuildSelf(desiredPoint, shouldSave, buildingEffect.main.duration - 2f, rotation);
                if (_currentConfig != null && _currentConfig.CurrencyType == CurrencyType.Energy)
                {
                    _currentConfig.SetHasCompleted(true);
                }

                EventBus.Instance.Trigger(new ToggleSwipeInput(true));
            }
            else
            {
                Destroy(interactable.gameObject);
                warningHelper.ConfigurePopupForUsedSpace();
                swipeHandler.enabled = true;
                EventBus.Instance.Trigger(new ToggleSwipeInput(false));
                OnPositionChoiceNeeded?.Invoke(_spawnedSketch);
            }
        }

        private void PlayBuildingEffect(CartesianPoint target)
        {
            buildingEffect.transform.localPosition = new Vector3(target.GetXCoordinate(), 3f,
                target.GetYCoordinate());
            buildingEffect.gameObject.SetActive(true);
            buildingEffect.Play();
        }

        private void DisableSwipeHandler()
        {
            swipeHandler.enabled = false;
        }

        private void AddListeners()
        {
            Task.OnTaskCompleted += SpawnSketch;
            Sketch.OnPlacementConfirmed += SpawnInteractable;
            Sketch.OnPlacementCancelled += DisableSwipeHandler;
            //GridSpawner.OnGridReady += InjectLogicBoard;
        }

        private void RemoveListeners()
        {
            Task.OnTaskCompleted -= SpawnSketch;
            Sketch.OnPlacementConfirmed -= SpawnInteractable;
            Sketch.OnPlacementCancelled -= DisableSwipeHandler;
            //GridSpawner.OnGridReady -= InjectLogicBoard;
        }
    }
}