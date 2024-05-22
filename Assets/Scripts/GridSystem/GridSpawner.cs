using System;
using System.Collections.Generic;
using Interactables;
using Scriptables;
using Spawners;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace GridSystem
{
    public class GridSpawner : MonoBehaviour
    {
        [SerializeField] private int width;
        [SerializeField] private int height;
        [SerializeField] private Transform gridParent;
        [SerializeField] private Grid gridPrefab;
        [SerializeField] private InteractableSpawner _spawner;

        private GridController _gridController;

        public static event Action<GridController> OnGridReady;

        private void Awake()
        {
            SpawnGrids();
        }

        private void OnDestroy()
        {
            
        }
        
        private void SpawnGrids()
        {
            Grid[,] board = new Grid[width, height];
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Grid tempGrid = new Grid(x, z);
                    board[x, z] = tempGrid;
                }
            }

            _gridController = new GridController(board);
            _spawner.InjectLogicBoard(_gridController);
            //DistributeLogicBoard();
            TryLoadInteractables();
        }

        private void TryLoadInteractables()
        {
            var data = SaveLoadManager.TryLoadFarm();
            _gridController.SetExistingFarmData(data);
            //var interactables = new List<InteractableData>(data.InteractableData);
            var interactables = new List<InteractableData>();
            foreach (var item in data.InteractableData)
            {
                var config = ScriptableObject.CreateInstance<InteractableConfig>();
                config = item.Config;
                interactables.Add(new InteractableData(config, item.Point));
            }

            foreach (var itemData in interactables)
            {
                _spawner.SpawnSelectedInteractable(itemData.Point, itemData.Config, false);
            }
        }

        private void DistributeLogicBoard()
        {
            OnGridReady?.Invoke(_gridController);
        }
    }
}
