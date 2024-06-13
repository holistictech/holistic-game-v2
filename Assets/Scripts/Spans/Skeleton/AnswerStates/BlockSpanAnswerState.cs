using UI;
using UI.Helpers;
using UnityEngine;
using Utilities;
using Utilities.Helpers;

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
            
            _blockHelper = spanController.GetHelperObject().GetComponent<BlockSpanUIHelper>();
            maxTime = spanController.GetRoundTime();
            EnableUIElements();
            ConfigureBlockHelper();
            if(spanController.GetCurrentMode() != CommonFields.BlockSpanModes.Regular)
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
            var options = spanController.GetCurrentDisplayedQuestions();
            optionPicker.ConfigureOptions(options);
        }

        protected override void PlayTimer(float duration)
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
            _blockHelper.gameObject.SetActive(true);
            _blockHelper.ResetCorsiBlocks();
            base.EnableUIElements();
        }

        public override void DisableUIElements()
        {
            _blockHelper.gameObject.SetActive(false);
            _blockHelper.ResetCorsiBlocks();
            optionPicker.DisableActiveOptions();
            base.DisableUIElements();
        }
    }
}
