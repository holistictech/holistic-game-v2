using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities;

namespace Interactables
{
    public class Animal : InteractableObject
    {
        public Animal(GridController controller, InteractableConfig config) : base(controller, config)
        {
        }
        
        public override void BuildSelf(CartesianPoint desiredPoint)
        {
            var points = base.CalculateCoordinatesForBlocking(desiredPoint);
            BlockCoordinates(points);
            base.BuildSelf(desiredPoint);
        }

    }
}
