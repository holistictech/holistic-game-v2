using UnityEngine;
using UnityEngine.Serialization;

namespace Scriptables.QuestionSystem
{
    public abstract class Question : ScriptableObject
    {
        public string CorrectAnswerString;
        public bool IsAnswerStringMUST; 
        private bool _hasSelected = false;
        public bool HasSelected()
        {
            return _hasSelected;
        }

        public void SetHasSelected(bool val)
        {
            _hasSelected = val;
        }

        public virtual Sprite GetCorrectSprite()
        {
            return null;
        }

        public virtual string GetCorrectText()
        {
            return "";
        }

        public abstract object GetQuestionItem();
    }
}
