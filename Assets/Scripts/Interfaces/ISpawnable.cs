using System.Collections.Generic;
using GridSystem;
using Scriptables;
using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Interfaces
{
    public interface ISpawnable
    {
        public abstract void InjectFields(GridController controller, InteractableConfig config);
        public abstract void BuildSelf(CartesianPoint desiredPoint, bool isFirstTime, float delay, Quaternion rotation);
        public abstract void SetObjectMesh();
        public abstract void SetPosition(CartesianPoint point);
        public abstract void SetScale();
        public abstract void BlockCoordinates(List<CartesianPoint> desiredPoints, CommonFields.InteractableType type);
        public abstract List<CartesianPoint> CalculateCoordinatesForBlocking(CartesianPoint desiredPoint);

            
    }
}
