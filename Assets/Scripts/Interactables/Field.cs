using System.Collections.Generic;
using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities.Helpers;

namespace Interactables
{
    public class Field : InteractableObject
    {
        public Field(GridController controller, InteractableConfig config) : base(controller, config)
        {
        }
        
        public override void BuildSelf(CartesianPoint desiredPoint, bool isFirstTime)
        {
            var points = base.CalculateCoordinatesForBlocking(desiredPoint);
            BlockCoordinates(points, GetInteractableType());
            base.BuildSelf(desiredPoint, isFirstTime);
        }
    }
}
