using UnityEngine;

namespace Spans.Skeleton
{
    public class SpanEventContainer : MonoBehaviour
    {
    }

    public class RoundResetEvent
    {
        
    }

    public class BlockSpanGridSizeEvent
    {
        internal Vector2Int NewGrid;
        internal int CircleCount;
        public BlockSpanGridSizeEvent(Vector2Int grid, int count)
        {
            NewGrid = grid;
            CircleCount = count;
        }
    }
}