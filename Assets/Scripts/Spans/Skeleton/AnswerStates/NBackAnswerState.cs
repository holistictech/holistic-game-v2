using System.Collections.Generic;
using Interfaces;
using Spans.NBack;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Helpers;

namespace Spans.Skeleton.AnswerStates
{
    public class NBackAnswerState : SpanAnswerState
    {
        [SerializeField] private Button[] _modeButtons; 
        private Coroutine _timer;
        private float _maxTime;
        private INBackStrategy _currentStrategy;
        private NBack.NBack _nBackController;
        private List<UnitCircle> _activeUnitCircles;
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                base.Enter(controller);
                _nBackController = controller.gameObject.GetComponent<NBack.NBack>();
            }

            _activeUnitCircles = spanController.GetActiveCircles(); 
            _maxTime = spanController.GetRoundTime();
            _currentStrategy = _nBackController.GetStrategyClass();
            _activeUnitCircles[^1].AnimateCircle();
            EnableUIElements();
            AddListeners();
            PlayTimer(_maxTime);
            if(_currentStrategy is DualNBackMode)
                timer.DisableSliderUI();
        }
        
        public override void SwitchNextState()
        {
            spanController.SwitchState();
        }

        public void ChooseType(int index)
        {
            var type = (CommonFields.ButtonType)index;
            _currentStrategy.AppendChosenButtonType(type);
            /*if (_currentStrategy.IsSwitchable())
            {
                SwitchNextState();
            }*/
        }
        
        public override void Exit()
        {
            if (_timer != null)
            {
                StopCoroutine(_timer);
            }
            _activeUnitCircles[^1].ResetSelf();
            DisableUIElements();
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
                button.interactable = false;
            }
        }
    }
}
