using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace GridSystem
{
    public class GridSpawner : MonoBehaviour
    {
        [SerializeField] private int width;
        [SerializeField] private int height;

        [SerializeField] private Grid gridPrefab;

        private void OnEnable()
        {
            SpawnGrids();
        }

        private void SpawnGrids()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var tempGrid = Instantiate(gridPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    tempGrid.InitializeGrid(x, y) ;
                }
            }
        }
    }
}
