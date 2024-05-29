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
        public Vector2Int NewGrid;
        public BlockSpanGridSizeEvent(Vector2Int grid)
        {
            NewGrid = grid;
        }
    }
}