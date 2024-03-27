using System;
using Spawners;
using UnityEngine;
using UnityEngine.Serialization;

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

        private void OnEnable()
        {
            SpawnGrids();
        }
        
        private void SpawnGrids()
        {
            Grid[,] board = new Grid[width, height];
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tempGrid = Instantiate(gridPrefab, new Vector3(x, 0, z), Quaternion.identity);
                    tempGrid.InitializeGrid(x, z);
                    tempGrid.transform.SetParent(gridParent);
                    board[x, z] = tempGrid;
                }
            }

            _gridController = new GridController(board);
            _spawner.InjectLogicBoard(_gridController);
            //DistributeLogicBoard();
        }

        private void DistributeLogicBoard()
        {
            OnGridReady?.Invoke(_gridController);
        }
    }
}
