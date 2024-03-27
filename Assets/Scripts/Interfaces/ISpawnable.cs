using System.Collections.Generic;
using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities;

namespace Interactables
{
    public interface ISpawnable
    {
        public abstract void InjectFields(GridController controller, InteractableConfig config);
        public abstract void BuildSelf(CartesianPoint desiredPoint);
        public abstract void BlockCoordinates(List<CartesianPoint> desiredPoints);
        public abstract List<CartesianPoint> CalculateCoordinatesForBlocking(CartesianPoint desiredPoint);

            
    }
}
