using System.Collections.Generic;
using Scriptables.Tutorial;
using Spans.Skeleton;
using UnityEngine;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "StateHolder", menuName = "SpanStates/Holder")]
    public class StateHolder : ScriptableObject
    {
        public string TutorialKey;
        public List<GameObject> StatePrefabs;
        public List<TutorialStep> TutorialSteps;
        public SpanInitialState InitialState;
        public SpanQuestionState QuestionState;
        public SpanAnswerState AnswerState;
        public SpanFeedbackState FeedbackState;
        public SpanGameEndState GameEndState;
    }
}
