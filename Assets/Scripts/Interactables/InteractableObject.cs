using System.Collections.Generic;
using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities;
using Grid = UnityEngine.Grid;
using static Utilities.CommonFields;

namespace Interactables
{
    public class InteractableObject : MonoBehaviour, ISpawnable
    {
        private GridController _gridController;
        private InteractableConfig _interactableConfig;
        
        protected GridController GridController => _gridController;
        protected InteractableConfig InteractableConfig => _interactableConfig;

        public InteractableObject(GridController controller, InteractableConfig config)
        {
            _gridController = controller;
            _interactableConfig = config;
        }

        public void InjectFields(GridController controller, InteractableConfig config)
        {
            _gridController = controller;
            _interactableConfig = config;
        }

        public virtual void BuildSelf(CartesianPoint desiredPoint)
        {
            throw new System.NotImplementedException();
        }

        public void BlockCoordinates(List<CartesianPoint> desiredPoints)
        {
            _gridController.BlockCoordinates(desiredPoints);
        }

        public virtual List<CartesianPoint> CalculateCoordinatesForBlocking(CartesianPoint desiredPoint)
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

        public InteractableType GetInteractableType()
        {
            return InteractableConfig.InteractableType;
        }
    }
}
