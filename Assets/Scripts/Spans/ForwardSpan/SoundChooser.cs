using System.Collections.Generic;
using Scriptables.QuestionSystem;
using UnityEngine;

namespace Spans.ForwardSpan
{
    public class SoundChooser : ForwardSpanVoiceDescription
    {
        public override List<Question> GetSpanObjects()
        {
            List<Question> shuffledSprites = new List<Question>(GetAllAvailableSpanObjects());
            
            foreach (var item in shuffledSprites)
            {
                item.IsAnswerStringMUST = false;
            }
            return GetRandomClips();
        }
        
        public override List<Question> GetChoices()
        {
            var allClips = base.GetAllAvailableSpanObjects();
            int choiceCount = currentRoundIndex;
            if (currentRoundIndex >= 4)
            {
                choiceCount = 9 - currentRoundIndex;
            }
            
            List<Question> choices = new List<Question>(GetCurrentDisplayedQuestions());
            
            for (int i = 0; i < choiceCount; i++)
            {
                var index = Random.Range(0, allClips.Length);
                var question = allClips[index];
                while (choices.Contains(question))
                {
                    index = Random.Range(0, allClips.Length);
                    question = allClips[index];
                }
                choices.Add(question);
            }
            choices.Shuffle();
            return choices;
        }

        public override bool IsAnswerCorrect()
        {
            if (currentGivenAnswers.Count == 0 || currentDisplayedQuestions.Count != currentGivenAnswers.Count)
            {
                IncrementFailStreak();
                return false;
            }

            if (isBackwards)
            {
                currentDisplayedQuestions.Reverse();
            }

            for (int i = 0; i < currentDisplayedQuestions.Count; i++)
            {
                if (currentDisplayedQuestions[i].GetQuestionItem() != currentGivenAnswers[i].GetQuestionItem())
                {
                    IncrementFailStreak();
                    return false;
                }
            }
            
            IncrementSuccessStreak();
            return true;
        }
    }
}
