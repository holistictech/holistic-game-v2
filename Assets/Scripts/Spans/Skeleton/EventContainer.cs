using Interfaces;
using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Spans.Skeleton
{
    public class EventContainer : MonoBehaviour
    {
    }

    public class GameLoadingEvent
    {
        public bool HasFinished;

        public GameLoadingEvent(bool finished)
        {
            HasFinished = finished;
        }
    }

    public class TutorialEvent
    {
        public bool HasFinished;

        public TutorialEvent(bool finished)
        {
            HasFinished = finished;
        }
    }

    public class ToggleUIEventButtons
    {
        public bool Toggle;

        public ToggleUIEventButtons(bool toggle)
        {
            Toggle = toggle;
        }
    }

    public class ToggleSwipeInput
    {
        public bool Toggle;

        public ToggleSwipeInput(bool toggle)
        {
            Toggle = toggle;
        }
    }

    public class SpanRequestedEvent
    {
        
    }

    public class SpanCompletedEvent
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