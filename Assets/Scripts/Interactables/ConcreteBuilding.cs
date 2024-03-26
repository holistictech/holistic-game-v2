using System.Collections.Generic;
using GridSystem;
using Scriptables;
using Utilities;

namespace Interactables
{
    public class ConcreteBuilding : InteractableObject
    {
        public ConcreteBuilding(GridController gridController, InteractableConfig config) : base(gridController, config)
        {
        }

        public override void BuildSelf(CartesianPoint desiredPoint)
        {
            var points = base.CalculateCoordinatesForBlocking(desiredPoint);
            base.BlockCoordinates(points);
        }
    }
}
