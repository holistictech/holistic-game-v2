using System.Collections.Generic;
using GridSystem;
using Scriptables;
using Utilities;

namespace Interfaces
{
    public interface ISpawnable
    {
        public abstract void InjectFields(GridController controller, InteractableConfig config);
        public abstract void BuildSelf(CartesianPoint desiredPoint);
        public abstract void SetObjectMesh();
        public abstract void SetPosition(CartesianPoint point);
        public abstract void BlockCoordinates(List<CartesianPoint> desiredPoints);
        public abstract List<CartesianPoint> CalculateCoordinatesForBlocking(CartesianPoint desiredPoint);

            
    }
}
