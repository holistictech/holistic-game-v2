using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;
using Utilities.Helpers;
using Random = UnityEngine.Random;

namespace GridSystem
{
    public class Grid
    {
        private CartesianPoint _point;
        private bool _isBlocked;

        public Grid(int xCoord, int zCoord)
        {
            _point = new CartesianPoint(xCoord, zCoord);
        }
        public void InitializeGrid(int xCoord, int zCoord)
        {
            _point = new CartesianPoint(xCoord, zCoord);
        }

        public bool IsBlocked()
        {
            return _isBlocked;
        }

        public void SetIsBlocked(bool value)
        {
            _isBlocked = value;
        }
    }
}
