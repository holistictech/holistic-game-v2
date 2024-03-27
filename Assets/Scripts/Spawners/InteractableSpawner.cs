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
        [SerializeField] private InteractableObject _spawnable;
        [SerializeField] private Transform _objectParent;
        private GridController _gridController;
        
        
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
        
    }
}
