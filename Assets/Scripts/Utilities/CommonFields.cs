namespace Utilities
{
    public class CommonFields
    {
        public enum InteractableType
        {
            Concrete = 0,
            Barn,
            Animal
        }
    }

    public struct Size
    {
        public int Width;
        public int Height;
    }

    public class CartesianPoint
    {
        private int _xCoordinate;
        private int _zCoordinate;
        
        
        public CartesianPoint(int x, int z)
        {
            _xCoordinate = x;
            _zCoordinate = z;
        }

        public int GetXCoordinate()
        {
            return _xCoordinate;
        }

        public int GetYCoordinate()
        {
            return _zCoordinate;
        }
    }
}
