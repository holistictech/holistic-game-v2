using System;
using System.Collections.Generic;
using Interactables;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using static Utilities.CartesianPoint;

namespace GridSystem
{
    public class GridController
    {
        private Grid[,] _board;
        private FarmData _farmData;

        public GridController(Grid[,] board)
        {
            _board = board;
        }

        public void SetExistingFarmData(FarmData data)
        {
            _farmData = data;
        }
        

        public bool IsGridBlocked(CartesianPoint point)
        {
            return _board[point.GetXCoordinate(), point.GetYCoordinate()].IsBlocked();
        }

        public bool IsPlacementValid(List<CartesianPoint> plan)
        {
            foreach (var point in plan)
            {
                if (IsPointInvalid(point)) return false;

                if (_board[point.GetXCoordinate(), point.GetYCoordinate()].IsBlocked())
                    return false;
            }
            return true;
        }

        private int GetWidth()
        {
            return _board.GetLength(0);
        }

        private int GetHeight()
        {
            return _board.GetLength(1);
        }

        public void BlockCoordinates(List<CartesianPoint> coordinates)
        {
            foreach (var point in coordinates)
            {
                try
                {
                    if (IsPointInvalid(point))
                    {
                        throw new IndexOutOfRangeException("Coordinate invalid: " + point);
                    }

                    _board[point.GetXCoordinate(), point.GetYCoordinate()].SetIsBlocked(true);
                }
                catch (IndexOutOfRangeException ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }
        }

        public void AppendSpawnedInteractables(InteractableData data)
        {
            _farmData.InteractableData.Add(data);
            //SaveLoadManager.SaveFarm(GetFarmData());
        }

        public FarmData GetFarmData()
        {
            return _farmData;
        }
        
        private bool IsPointInvalid(CartesianPoint placedPoint) => 
            placedPoint.GetXCoordinate() >= GetWidth() || placedPoint.GetYCoordinate() >= GetHeight();
    }
    
    [Serializable]
    public class FarmData
    {
        public List<InteractableData> InteractableData = new List<InteractableData>();
    }
}
