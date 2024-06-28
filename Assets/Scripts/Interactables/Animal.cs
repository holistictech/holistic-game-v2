using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Interactables
{
    public class Animal : InteractableObject
    {
        public Animal(GridController controller, InteractableConfig config) : base(controller, config)
        {
        }
        
        public override void BuildSelf(CartesianPoint desiredPoint, bool isFirstTime, float delay)
        {
            var points = base.CalculateCoordinatesForBlocking(desiredPoint);
            BlockCoordinates(points, GetInteractableType());
            base.BuildSelf(desiredPoint, isFirstTime, delay);
        }
    }
}
