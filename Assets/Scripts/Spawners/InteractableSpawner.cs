using System;
using System.Collections.Generic;
using Factory;
using GridSystem;
using Interactables;
using Scriptables;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;
using static Utilities.CommonFields;

namespace Spawners
{
    public class InteractableSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _spawnable;
        [SerializeField] private Transform _objectParent;
        private GridController _gridController;

        [SerializeField] private InteractableConfig _itemConfig;

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

        private void TestSpawn(Vector3 position)
        {
            var point = new CartesianPoint((int)position.x, (int)position.y);
            SpawnSelectedInteractable(point, _itemConfig);
        }


        public void SpawnSelectedInteractable(CartesianPoint desiredPoint, InteractableConfig itemConfig)
        {
            var spawnedInstance = InteractableFactory.SpawnInstance(_spawnable, itemConfig, _objectParent);
            var interactable = spawnedInstance.GetComponent<InteractableObject>();

            if (interactable != null)
            {
                interactable.InjectFields(_gridController, itemConfig);
                interactable.BuildSelf(desiredPoint);
            }
            else
            {
                Debug.LogError("Error while spawning building");
            }
        }

        private void AddListeners()
        {
            SwipeHandler.OnLocationSelected += TestSpawn;
            //GridSpawner.OnGridReady += InjectLogicBoard;
        }

        private void RemoveListeners()
        {
            SwipeHandler.OnLocationSelected -= TestSpawn;
            //GridSpawner.OnGridReady -= InjectLogicBoard;
        }
        
    }
}
