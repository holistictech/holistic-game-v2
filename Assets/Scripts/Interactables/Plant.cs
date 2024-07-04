using System.Collections.Generic;
using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities.Helpers;

namespace Interactables
{
    public class Plant : InteractableObject
    {
        public Plant(GridController controller, InteractableConfig config) : base(controller, config)
        {
        }
        
        public override void BuildSelf(CartesianPoint desiredPoint, bool isFirstTime, float delay, Quaternion rotation)
        {
            //var points = base.CalculateCoordinatesForBlocking(desiredPoint);
            BlockCoordinates(new List<CartesianPoint>{desiredPoint}, GetInteractableType());
            base.BuildSelf(desiredPoint, isFirstTime, delay, rotation);
        }
    }
}
