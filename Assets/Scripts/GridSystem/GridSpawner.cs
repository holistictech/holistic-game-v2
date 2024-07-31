using System;
using System.Collections;
using System.Collections.Generic;
using Interactables;
using Spans.Skeleton;
using Spawners;
using UnityEngine;
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

        private void Start()
        {
            StartCoroutine(SpawnGridsAsync());
        }

        private IEnumerator SpawnGridsAsync()
        {
            EventBus.Instance.Trigger(new GameLoadingEvent(false));

            Grid[,] board = new Grid[width, height];
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    Grid tempGrid = new Grid(x, z);
                    board[x, z] = tempGrid;
                }
                
                yield return new WaitForEndOfFrame();  // Yield control back to the main thread
            }

            EventBus.Instance.Trigger(new GameLoadingEvent(true));
            _gridController = new GridController(board);
            _spawner.InjectLogicBoard(_gridController);

            // DistributeLogicBoard();
            TryLoadInteractables();
            
        }

        private void TryLoadInteractables()
        {
            var data = SaveLoadManager.TryLoadFarm();
            _gridController.SetExistingFarmData(data);
            var interactables = new List<InteractableData>();
            foreach (var item in data.InteractableData)
            {
                var config = item.Config;
                interactables.Add(new InteractableData(config, item.Point, item.GetRotation()));
            }

            foreach (var itemData in interactables)
            {
                _spawner.SpawnSelectedInteractable(itemData.Point, itemData.Config, false, itemData.GetRotation());
            }
        }

        private void DistributeLogicBoard()
        {
            OnGridReady?.Invoke(_gridController);
        }
    }
}
