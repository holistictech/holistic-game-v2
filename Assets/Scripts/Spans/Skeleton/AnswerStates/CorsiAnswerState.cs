using UI.Helpers;
using UnityEngine;

namespace Spans.Skeleton.AnswerStates
{
    public class CorsiAnswerState : SpanAnswerState
    {
        private CorsiBlockUIHelper _corsiHelper;
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                spanController = controller;
                base.Enter(controller);
            }
            _corsiHelper = spanController.GetHelperObject().GetComponent<CorsiBlockUIHelper>();
            maxTime = spanController.GetRoundTime();
            EnableUIElements();
            ConfigureCorsiHelper();
            PlayTimer(maxTime);
        }

        private void ConfigureCorsiHelper()
        {
            var circles = spanController.GetActiveCircles();
            _corsiHelper.SetActiveCircles(circles);
            _corsiHelper.ConfigureInput(true);
        }

        protected override void PlayTimer(float duration)
        {
            timer.StartTimer(duration, SwitchNextState);
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
