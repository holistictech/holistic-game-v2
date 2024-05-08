using UI.Helpers;
using UnityEngine;

namespace Spans.Skeleton.AnswerStates
{
    public class CorsiAnswerState : SpanAnswerState
    {
        private CorsiBlockUIHelper _corsiHelper;
        private SpanController _spanController;
        private float _maxTime;
        public override void Enter(SpanController spanController)
        {
            base.Enter(spanController);
            if (_spanController == null)
            {
                _spanController = spanController;
                _corsiHelper = _spanController.GetHelperObject().GetComponent<CorsiBlockUIHelper>();
            }
            _maxTime = _spanController.GetRoundTime();
            EnableUIElements();
            ConfigureCorsiHelper();
            PlayTimer(_maxTime);
        }

        private void ConfigureCorsiHelper()
        {
            _corsiHelper.InjectAnswerState(this);
            var circles = _spanController.GetActiveCircles();
            _corsiHelper.SetActiveCircles(circles);
        }
        
        public override void PlayTimer(float maxTime)
        {
            timer.StartTimer(maxTime, SwitchNextState);
        }

        public override void Exit()
        {
            base.Exit();
            _corsiHelper.DisableUnitCircles();
            DisableUIElements();
        }
        
        public override void SwitchNextState()
        {
            if (_spanController.GetTutorialStatus())
            {
                _spanController.ClearTutorialHighlights();
            }
            _spanController.SetSelectedAnswers(_corsiHelper.GetGivenAnswers());
            _spanController.SwitchState();
        }

        public override void RevertLastAnswer()
        {
            _corsiHelper.RevokeLastSelection();
        }

        public override void EnableUIElements()
        {
            _corsiHelper.gameObject.SetActive(true);
            _corsiHelper.ResetCorsiBlocks();
            base.EnableUIElements();
        }

        public override void DisableUIElements()
        {
            _corsiHelper.gameObject.SetActive(false);
            _corsiHelper.ResetCorsiBlocks();
            base.DisableUIElements();
        }
    }
}
