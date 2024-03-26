using System;
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

        public void BlockCoordinates(CartesianPoint placedPoint, Size size)
        {
            try
            {
                if (placedPoint.GetXCoordinate() + size.Width >= GetWidth() ||
                    placedPoint.GetYCoordinate() + size.Height >= GetHeight())
                {
                    throw new IndexOutOfRangeException("Placement coordinates exceed the board boundaries.");
                }

                for (int y = placedPoint.GetYCoordinate(); y < placedPoint.GetYCoordinate() + size.Height; y++)
                {
                    for (int x = placedPoint.GetXCoordinate(); x < placedPoint.GetXCoordinate() + size.Width; x++)
                    {
                        _board[x, y].SetIsBlocked(true);
                    }
                }
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
}
