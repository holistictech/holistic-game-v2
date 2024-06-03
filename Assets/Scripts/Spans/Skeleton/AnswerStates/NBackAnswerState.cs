using Interfaces;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

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
                spanEventBus.Register<NBackModeEvent>(UpdateStrategy);
            }

            _maxTime = spanController.GetRoundTime();
            _currentStrategy = _nBackController.GetStrategyClass();
            EnableUIElements();
            AddListeners();
            PlayTimer(_maxTime);
        }
        
        public override void SwitchNextState()
        {
            spanController.SwitchState();
        }

        public void ChooseType(int index)
        {
            var type = (CommonFields.ButtonType)index;
            _currentStrategy.SetChosenButtonType(type);
            SwitchNextState();
        }
        
        public override void Exit()
        {
            if (_timer != null)
            {
                StopCoroutine(_timer);
            }
            base.Exit();
        }
        
        public override void EnableUIElements()
        {
            _currentStrategy.EnableButtons(_modeButtons);
        }

        public override void DisableUIElements()
        {
            foreach (var button in _modeButtons)
            {
                button.gameObject.SetActive(false);
            }
        }

        private void UpdateStrategy(NBackModeEvent newMode)
        {
            _currentStrategy = newMode.StrategyClass;
        }
    }
}
