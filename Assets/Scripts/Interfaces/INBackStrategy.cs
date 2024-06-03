using System.Collections.Generic;
using ETFXPEL;
using Scriptables.QuestionSystem;
using Spans.NBack;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Interfaces
{
    public interface INBackStrategy
    {
        public void InjectController(NBack controller);
        public void SetChosenButtonType(CommonFields.ButtonType chosen);
        public bool CheckAnswer();
        public List<Question> GetQuestionByCount(List<Question> questions, int count);

        public int GetModeIndex();
        public void EnableButtons(Button[] allButtons)
        {
            for (int i = 0; i < GetModeIndex(); i++)
            {
                allButtons[i].gameObject.SetActive(true);
            }
        }
    }
}
