namespace Utilities
{
    public class CommonFields
    {
        
    }

    public struct Size
    {
        public int Width;
        public int Height;
    }

    public class CartesianPoint
    {
        private int _xCoordinate;
        private int _yCoordinate;
        
        
        public CartesianPoint(int x, int y)
        {
            _xCoordinate = x;
            _yCoordinate = y;
        }

        public int GetXCoordinate()
        {
            return _xCoordinate;
        }

        public int GetYCoordinate()
        {
            return _yCoordinate;
        }
    }
}
