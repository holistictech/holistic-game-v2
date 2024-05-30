using UI;
using UI.Helpers;
using UnityEngine;

namespace Spans.Skeleton.AnswerStates
{
    public class BlockSpanAnswerState : SpanAnswerState
    {
        [SerializeField] private OptionPicker optionPicker;
        private CorsiBlockUIHelper _blockHelper;
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                base.Enter(controller);
            }

            maxTime = spanController.GetRoundTime();
            EnableUIElements();
            ConfigureBlockHelper();
            ConfigureOptionPicker();
            PlayTimer(maxTime);
        }
        
        private void ConfigureBlockHelper()
        {
            var circles = spanController.GetActiveCircles();
            _blockHelper.SetActiveCircles(circles);
            _blockHelper.ConfigureInput(true);
        }

        private void ConfigureOptionPicker()
        {
            var options = spanController.GetCurrentSpanQuestions();
            optionPicker.ConfigureOptions(options);
        }
        
        public override void PlayTimer(float duration)
        {
            timer.StartTimer(duration, SwitchNextState);
        }
        
        public override void Exit()
        {
            base.Exit();
            _blockHelper.DisableUnitCircles();
            DisableUIElements();
        }
        
        public override void RevertLastAnswer()
        {
            _blockHelper.RevokeLastSelection();
        }
        
        public override void SwitchNextState()
        {
            spanController.SetSelectedAnswers(_blockHelper.GetGivenAnswers());
            spanController.SwitchState();
        }

        public override void EnableUIElements()
        {
            _blockHelper.ResetCorsiBlocks();
            base.EnableUIElements();
        }

        public override void DisableUIElements()
        {
            _blockHelper.ResetCorsiBlocks();
            base.DisableUIElements();
        }
    }
}
