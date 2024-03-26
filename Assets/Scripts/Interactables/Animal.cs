using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities;

namespace Interactables
{
    public class Animal : InteractableObject
    {
        public Animal(GridController gridController, InteractableConfig config) : base(gridController, config)
        {
        }

        public override void BuildSelf(CartesianPoint desiredPoint)
        {
            var points = base.CalculateCoordinatesForBlocking(desiredPoint);
            base.BlockCoordinates(points);
        }
    }
}
