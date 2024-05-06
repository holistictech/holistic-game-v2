using System.Collections.Generic;
using GridSystem;
using Interfaces;
using Scriptables;
using UnityEngine;
using Utilities;
using Grid = UnityEngine.Grid;
using static Utilities.CommonFields;

namespace Interactables
{
    public class InteractableObject : MonoBehaviour, ISpawnable
    {
        private MeshFilter _objectMeshFilter;
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
            SetObjectMesh();
            SetPosition(desiredPoint);
        }

        public void BlockCoordinates(List<CartesianPoint> desiredPoints)
        {
            _gridController.BlockCoordinates(desiredPoints);
        }

        public virtual List<CartesianPoint> CalculateCoordinatesForBlocking(CartesianPoint desiredPoint)
        {
            List<CartesianPoint> points = new List<CartesianPoint>();

            for (int z = desiredPoint.GetYCoordinate() - _interactableConfig.Height / 2; z < desiredPoint.GetYCoordinate() + _interactableConfig.Height / 2; z++)
            {
                for (int x = desiredPoint.GetXCoordinate() - _interactableConfig.Width / 2 ; x < desiredPoint.GetXCoordinate() + _interactableConfig.Width / 2; x++)
                {
                    CartesianPoint temp = new CartesianPoint(x, z);
                    points.Add(temp);
                }
            }
            return points;
        }

        public void SetObjectMesh()
        {
            _objectMeshFilter = GetComponent<MeshFilter>();
            _objectMeshFilter.mesh = _interactableConfig.ObjectMesh;
        }

        public void SetPosition(CartesianPoint point)
        {
            transform.position = new Vector3(point.GetXCoordinate(), 0, point.GetYCoordinate());
        }
    }
}
