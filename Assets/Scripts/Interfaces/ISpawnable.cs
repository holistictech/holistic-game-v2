using System.Collections.Generic;
using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities;

namespace Interfaces
{
    public interface ISpawnable
    {
        public abstract void InjectFields(GridController controller, InteractableConfig config, ParticleSystem effect);
        public abstract void BuildSelf(CartesianPoint desiredPoint, bool isFirstTime);
        public abstract void SetObjectMesh();
        public abstract void SetPosition(CartesianPoint point);
        public abstract void BlockCoordinates(List<CartesianPoint> desiredPoints);
        public abstract List<CartesianPoint> CalculateCoordinatesForBlocking(CartesianPoint desiredPoint);

            
    }
}
