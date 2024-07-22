using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Utilities.Helpers;

namespace Scriptables.QuestionSystem
{
    public abstract class Question : ScriptableObject
    {
        public string CorrectAnswerString;
        public string[] CorrectAnswers;
        public bool IsAnswerStringMUST; 
        private bool _hasSelected = false;
        public int SpawnAmount = 1;
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

        public virtual void ResetSelf()
        {
            
        }

        public abstract void SetQuestionItem(object item);
        public abstract object GetQuestionItem();
        public abstract bool IsEqual(Question question);

        public abstract object GetQuestionItemByType(CommonFields.ButtonType type);

        public virtual void SetQuestionItemByType(CommonFields.ButtonType type, object value)
        {
            
        }
    }
}
