using UnityEngine;

namespace Scriptables.QuestionSystem
{
    public abstract class Question : ScriptableObject
    {
        public string CorrectAnswer;

        public string GetCorrectAnswer()
        {
            return CorrectAnswer;
        }
    }
}
