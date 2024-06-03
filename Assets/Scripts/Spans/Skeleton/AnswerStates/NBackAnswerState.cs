using Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton.AnswerStates
{
    public class NBackAnswerState : SpanAnswerState
    {
        [SerializeField] private Button[] _modeButtons; 
        private Coroutine _timer;
        private float _maxTime;
        private INBackStrategy _currentStrategy;
        private NBack.NBack _nBackController;
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                base.Enter(controller);
                _nBackController = controller.gameObject.GetComponent<NBack.NBack>();
            }

            _maxTime = spanController.GetRoundTime();
            AddListeners();
            EnableUIElements();
            PlayTimer(_maxTime);
            _currentStrategy = _nBackController.GetStrategyClass();
        }

        public override void EnableUIElements()
        {
            _currentStrategy.EnableButtons(_modeButtons);
        }
    }
}
