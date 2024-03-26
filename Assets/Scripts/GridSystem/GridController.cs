using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;
using static Utilities.CartesianPoint;

namespace GridSystem
{
    public class GridController
    {
        private Grid[,] _board;

        public bool IsGridBlocked(CartesianPoint point)
        {
            return _board[point.GetXCoordinate(), point.GetYCoordinate()].IsBlocked();
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
        
        private bool IsPointInvalid(CartesianPoint placedPoint) => 
            placedPoint.GetXCoordinate() >= GetWidth() || placedPoint.GetYCoordinate() >= GetHeight();
    }
}
