using System.Collections.Generic;
using Spans.Skeleton;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "StateHolder", menuName = "SpanStates/Holder")]
    public class StateHolder : ScriptableObject
    {
        public SpanInitialState InitialState;
        public SpanQuestionState QuestionState;
        public SpanAnswerState AnswerState;
        public SpanFeedbackState FeedbackState;
        public SpanGameEndState GameEndState;
    }
}
