using System.Collections.Generic;
using ETFXPEL;
using Scriptables.QuestionSystem;
using Spans.NBack;
using Spans.Skeleton.QuestionStates;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.Helpers;

namespace Interfaces
{
    public interface INBackStrategy
    {
        public void InjectQuestionState(NBackQuestionState questionState);
        public void ShowQuestion();
        public void AppendChosenButtonType(CommonFields.ButtonType chosen);
        public bool CheckAnswer();
        public bool IsEmptyRound();
        public List<Question> GetQuestionByCount(List<Question> questions, int count);
        public void SetCorrectType();
        public bool IsSwitchable();

        public int[] GetModeIndexes();
        public CommonFields.NBackModes GetModeEnum(); 
        public void EnableButtons(Button[] allButtons)
        {
            var indexes = GetModeIndexes();
            for (int i = 0; i < indexes.Length; i++)
            {
                if (allButtons.Length > indexes[i])
                {
                    allButtons[indexes[i]].gameObject.SetActive(true);
                    allButtons[indexes[i]].interactable = true;
                }
            }
        }
    }
}
