using System;
using System.Collections.Generic;
using Factory;
using GridSystem;
using Interactables;
using Scriptables;
using UI;
using UI.Helpers;
using UI.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Utilities.Helpers;
using static Utilities.Helpers.CommonFields;

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
                var transform1 = mainCamera.transform;
                Vector3 cameraPosition = transform1.position;
                Vector3 cameraForward = transform1.forward;
                Vector3 middlePoint = cameraPosition + cameraForward * mainCamera.nearClipPlane;
                _spawnedSketch = Instantiate(sketch, objectParent);
                _spawnedSketch.ConfigureObjectMesh(MeshContainer.Instance.GetMeshById(_currentConfig.RewardInteractable.MeshId), _currentConfig.Rotatable);
                _spawnedSketch.transform.position = new Vector3(middlePoint.x, 0, 0);
                _spawnedSketch.ConfigureSize(_currentConfig.RewardInteractable);
                swipeHandler.enabled = true;
                OnPositionChoiceNeeded?.Invoke(_spawnedSketch);
            }
        }

        private void SpawnInteractable(Quaternion rotation)
        {
            var finalPos = swipeHandler.GetFinalPosition();
            SpawnSelectedInteractable(new CartesianPoint((int)finalPos.x, (int)finalPos.z), _currentConfig.RewardInteractable, true, rotation);
            PlayerInventory.Instance.ChangeCurrencyAmountByType(_currentConfig);
        }

        public void SpawnSelectedInteractable(CartesianPoint desiredPoint, InteractableConfig config, bool shouldSave, Quaternion rotation)
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
            }
            else
            {
                Destroy(interactable.gameObject);
                warningHelper.ConfigurePopupForUsedSpace();
                OnPositionChoiceNeeded?.Invoke(_spawnedSketch);
                swipeHandler.enabled = true;
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
