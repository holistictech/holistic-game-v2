using System.Collections.Generic;
using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Interactables
{
    public class ConcreteBuilding : InteractableObject
    {
        public ConcreteBuilding(GridController gridController, InteractableConfig config) : base(gridController, config)
        {
        }

        public override void BuildSelf(CartesianPoint desiredPoint, bool isFirstTime, float delay, Quaternion rotation)
        {
            var points = base.CalculateCoordinatesForBlocking(desiredPoint);
            BlockCoordinates(points, GetInteractableType());
            base.BuildSelf(desiredPoint, isFirstTime, delay, rotation);
        }
    }
}
