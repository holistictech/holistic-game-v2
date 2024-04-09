using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Spans.ForwardSpan
{
    public class ForwardSpanImageDescription : SpanController
    {
        [SerializeField] private ImageQuestion[] imageQuestions;
        
        public override List<Question> GetSpanObjects()
        {
            return GetRandomSprites();
        }

        public override int GetRoundTime()
        {
            return currentRoundIndex * 3 + 2;
        }
        
        private List<Question> GetRandomSprites()
        {
            List<Question> shuffledSprites = new List<Question>(imageQuestions);
            for (int i = 0; i < shuffledSprites.Count; i++)
            {
                int randomIndex = Random.Range(i, shuffledSprites.Count);
                (shuffledSprites[i], shuffledSprites[randomIndex]) = (shuffledSprites[randomIndex], shuffledSprites[i]);
            }
            
            List<Question> selected = new List<Question>();
            for (int i = 0; i < fetchedQuestionCount; i++)
            {
                selected.Add(shuffledSprites[i]);
            }

            currentSpanQuestions = selected;
            return selected;
        }
        
    }
}
