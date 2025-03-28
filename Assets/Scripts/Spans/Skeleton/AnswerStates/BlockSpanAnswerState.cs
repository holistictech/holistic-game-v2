using DG.Tweening;
using TMPro;
using UI;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Helpers;

namespace Spans.Skeleton.AnswerStates
{
    public class BlockSpanAnswerState : SpanAnswerState
    {
        [SerializeField] private OptionPicker optionPicker;

        private Block.BlockSpan _blockSpanController;
        private CorsiBlockUIHelper _blockHelper;
        public override void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                _blockSpanController = controller.GetComponent<Block.BlockSpan>();
                base.Enter(controller);
            }

            if (spanController is Block.BlockSpan)
            {
                _blockHelper = spanController.GetHelperObject().GetComponent<BlockSpanUIHelper>();
            }
            else
            {
                _blockHelper = spanController.GetHelperObject().GetComponent<CorsiBlockUIHelper>();
            }
            maxTime = spanController.GetRoundTime();
            EnableUIElements();
            ConfigureBlockHelper();
            if(spanController.GetCurrentMode() != CommonFields.BlockSpanModes.Regular)
                ConfigureOptionPicker();
            PlayTimer(maxTime);
        }

        private bool _canDisplayBanner;
        private void ConfigureBlockHelper()
        {
            var circles = spanController.GetActiveCircles();
            _blockHelper.SetActiveCircles(circles);
            _blockHelper.ConfigureInput(true);
            if(circles.Count > 1) _canDisplayBanner = true;
        }

        private void ConfigureOptionPicker()
        {
            var options = spanController.GetCurrentDisplayedQuestions();
            optionPicker.ConfigureOptions(options);
        }

        protected override void PlayTimer(float duration)
        {
            if (_blockSpanController != null && _blockSpanController.GetCombineStatus() && _canDisplayBanner)
            {
                hintHelper.SetFieldText("Gördüklerini birleştir!");
                hintHelper.AnimateBanner(() =>
                {
                    timer.StartTimer(duration, SwitchNextState);
                });
            }
            else
            {
                timer.StartTimer(duration, SwitchNextState);
            }
        }
        
        public override void Exit()
        {
            base.Exit();
            _canDisplayBanner = false;
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
