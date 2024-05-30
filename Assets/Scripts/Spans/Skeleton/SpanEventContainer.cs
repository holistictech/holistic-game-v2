using Interfaces;
using UnityEngine;
using Utilities;

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
        internal GridConfiguration NewConfig;
        internal IBlockSpanStrategy StrategyClass;
        internal int CircleCount;
        public BlockSpanGridSizeEvent(GridConfiguration config, int count, IBlockSpanStrategy strategy)
        {
            StrategyClass = strategy;
            NewConfig = config;
            CircleCount = count;
        }
    }
}