using UnityEngine;

namespace Scriptables.QuestionSystem
{
    public abstract class Question : ScriptableObject
    {
        public string CorrectAnswer;
        private bool _hasSelected = false;

        public string GetCorrectAnswer()
        {
            return CorrectAnswer;
        }

        public bool HasSelected()
        {
            return _hasSelected;
        }

        public void SetHasSelected(bool val)
        {
            _hasSelected = val;
        }

        public abstract object GetQuestionItem();
    }
}
