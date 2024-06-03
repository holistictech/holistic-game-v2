using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utilities
{
    public class CommonFields
    {
        public const float ROUND_DURATION = 180f;
        public const int DAILY_ACTIVITY_COUNT = 10;
        public const int DEFAULT_ROUND_INDEX = 2;
        
        [Serializable]
        public enum InteractableType
        {
            Concrete = 0,
            Barn,
            Animal
        }

        public enum CurrencyType
        {
            Energy = 0,
            Performance
        }

        public enum BlockSpanModes
        {
            Regular = 0,
            ColorChooser,
            ItemChooser
        }

        public enum ButtonType
        {
            Identical = 0,
            NotIdentical,
            Shape,
            Color,
            Count
        }

        public enum Direction
        {
            Up = 0,
            Right,
            Down,
            Left
        }

        public static Dictionary<Direction, Vector2> DirectionVectors = new Dictionary<Direction, Vector2>()
        {
            { Direction.Up, new Vector2(0, 100) },
            { Direction.Right, new Vector2(100, 0) },
            { Direction.Down, new Vector2(0, -100) },
            { Direction.Left, new Vector2(-100, 0) },
        };
    }

    public struct Size
    {
        public int Width;
        public int Height;
    }

    [Serializable]
    public class CartesianPoint
    {
        public int XCoordinate;
        public int ZCoordinate;
        
        
        public CartesianPoint(int x, int z)
        {
            XCoordinate = x;
            ZCoordinate = z;
        }

        public int GetXCoordinate()
        {
            return XCoordinate;
        }

        public int GetYCoordinate()
        {
            return ZCoordinate;
        }
    }

    public class GridConfiguration
    {
        public Vector2Int GridSize { get; set; }
        public CommonFields.BlockSpanModes Mode { get; set; }

        public GridConfiguration(Vector2Int grid, CommonFields.BlockSpanModes mode)
        {
            GridSize = grid;
            Mode = mode;
        }
    }
}
