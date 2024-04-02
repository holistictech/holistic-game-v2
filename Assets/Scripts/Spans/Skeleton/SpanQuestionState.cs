using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton
{
    public class SpanQuestionState : ISpanState
    {
        [SerializeField] private Image _questionBox;
        private SpanController _spanController;
        private List<object> _spanObjects;
        public void Enter(SpanController spanController)
        {
            if (_spanController == null) _spanController = spanController;
            _spanObjects = _spanController.GetSpanObjects();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }
    }
}
