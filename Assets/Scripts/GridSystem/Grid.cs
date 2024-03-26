using System;
using UnityEngine;
using Utilities;

namespace GridSystem
{
    public class Grid : MonoBehaviour
    {
        private CartesianPoint _point;
        private bool _isBlocked;

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
