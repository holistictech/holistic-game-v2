using System;
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

        private void OnEnable()
        {
            SpawnGrids();
        }
        
        [ContextMenu("Spawn grids")]
        private void SpawnGrids()
        {
            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tempGrid = Instantiate(gridPrefab, new Vector3(x, 0, z), Quaternion.identity);
                    tempGrid.InitializeGrid(x, z);
                    tempGrid.transform.SetParent(gridParent);
                }
            }
        }
    }
}
