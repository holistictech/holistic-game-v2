using System.Collections.Generic;
using Scriptables.QuestionSystem;
using UI;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton.QuestionStates
{
    public class CorsiQuestionState : ISpanState
    {
        [SerializeField] private CorsiBlockUIHelper blockUIHelper;

        private SpanController _spanController;
        private List<Question> spanObjects = new List<Question>();
        public void Enter(SpanController spanController)
        {
            if (_spanController == null)
            {
                _spanController = spanController;
                blockUIHelper.InjectQuestionState(this);
            }
            
            spanObjects = _spanController.GetSpanObjects();
            DistributeQuestions();
        }

        private void DistributeQuestions()
        {
            blockUIHelper.AssignQuestions(spanObjects);
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }

        public void TryShowStateTutorial()
        {
        }

        public void EnableUIElements()
        {
            throw new System.NotImplementedException();
        }

        public void DisableUIElements()
        {
            throw new System.NotImplementedException();
        }
    }
}
