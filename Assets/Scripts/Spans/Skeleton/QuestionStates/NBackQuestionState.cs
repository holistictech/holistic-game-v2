using System.Collections.Generic;
using Scriptables.QuestionSystem;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.Skeleton.QuestionStates
{
    public class NBackQuestionState : SpanQuestionState
    {
        [SerializeField] private Image questionBox;
        
        private List<Question> _spanObjects; 
        private List<Question> _currentQuestions = new List<Question>();
        
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                base.Enter(controller);
            }

            _spanObjects = spanController.GetSpanObjects();
            EnableUIElements();
            SetCircleUI(spanController.GetRoundIndex());
            StatisticsHelper.IncrementDisplayedQuestionCount();
        }
    }
}
