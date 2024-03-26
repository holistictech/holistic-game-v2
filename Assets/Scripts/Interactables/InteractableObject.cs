using System.Collections.Generic;
using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities;

namespace Interactables
{
    public abstract class InteractableObject : MonoBehaviour
    {
        private GridController _gridController;
        private InteractableConfig _interactableConfig;
        
        protected GridController GridController => _gridController;
        protected InteractableConfig InteractableConfig => _interactableConfig;

        protected InteractableObject(GridController gridController, InteractableConfig config)
        {
            _gridController = gridController;
            _interactableConfig = config;
        }

        public abstract void BuildSelf(CartesianPoint desiredPoint);

        protected virtual void BlockCoordinates(List<CartesianPoint> desiredPoints)
        {
            _gridController.BlockCoordinates(desiredPoints);
        }

        protected virtual List<CartesianPoint> CalculateCoordinatesForBlocking(CartesianPoint desiredPoint)
        {
            List<CartesianPoint> points = new List<CartesianPoint>();
            
            for (int y = desiredPoint.GetYCoordinate(); y < desiredPoint.GetYCoordinate() + _interactableConfig.Height; y++)
            {
                for (int x = desiredPoint.GetXCoordinate(); x < desiredPoint.GetXCoordinate() + _interactableConfig.Width; x++)
                {
                    CartesianPoint temp = new CartesianPoint(x, y);
                    points.Add(temp);
                }
            }

            return points;
        }
    }
}
