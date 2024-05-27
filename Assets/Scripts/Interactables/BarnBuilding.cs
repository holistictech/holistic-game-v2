using System.Collections.Generic;
using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities;

namespace Interactables
{
    public class BarnBuilding : InteractableObject
    {
        public BarnBuilding(GridController gridController, InteractableConfig config) : base(gridController, config)
        {
        }

        public override void BuildSelf(CartesianPoint desiredPoint, bool isFirstTime)
        {
            var points = CalculateCoordinatesForBlocking(desiredPoint);
            BlockCoordinates(points);
            base.BuildSelf(desiredPoint, isFirstTime);
        }

        public override List<CartesianPoint> CalculateCoordinatesForBlocking(CartesianPoint desiredPoint)
        {
            List<CartesianPoint> corners = new List<CartesianPoint>();
            int width = InteractableConfig.Width;
            int height = InteractableConfig.Height;
            
            CartesianPoint bottomLeft = new CartesianPoint(desiredPoint.GetXCoordinate(), desiredPoint.GetYCoordinate());
            CartesianPoint bottomRight = new CartesianPoint(desiredPoint.GetXCoordinate() + width, desiredPoint.GetYCoordinate());
            CartesianPoint topLeft = new CartesianPoint(desiredPoint.GetXCoordinate(), desiredPoint.GetYCoordinate() + height);
            CartesianPoint topRight = new CartesianPoint(desiredPoint.GetXCoordinate() + width, desiredPoint.GetYCoordinate() + height);
            
            corners.Add(bottomLeft);
            corners.Add(bottomRight);
            corners.Add(topLeft);
            corners.Add(topRight);

            return corners;
        }
    }
}
