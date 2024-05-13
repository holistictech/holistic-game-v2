using UI.Helpers;
using UnityEngine;

namespace Spans.Skeleton.AnswerStates
{
    public class CorsiAnswerState : SpanAnswerState
    {
        private CorsiBlockUIHelper _corsiHelper;
        private float _maxTime;
        public override void Enter(SpanController controller)
        {
            base.Enter(controller);
            if (spanController == null)
            {
                spanController = controller;
            }
            _corsiHelper = spanController.GetHelperObject().GetComponent<CorsiBlockUIHelper>();
            _maxTime = spanController.GetRoundTime();
            EnableUIElements();
            ConfigureCorsiHelper();
            PlayTimer(_maxTime);
        }

        private void ConfigureCorsiHelper()
        {
            _corsiHelper.InjectAnswerState(this);
            var circles = spanController.GetActiveCircles();
            _corsiHelper.SetActiveCircles(circles);
            _corsiHelper.EnableInput();
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
            if (spanController.GetTutorialStatus())
            {
                spanController.ClearTutorialHighlights();
            }
            spanController.SetSelectedAnswers(_corsiHelper.GetGivenAnswers());
            spanController.SwitchState();
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
