using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spans.ForwardSpan
{
    public class ForwardSpanImageDescription : SpanController
    {
        [SerializeField] private ImageQuestion[] spanSprites;
        
        public override List<Question> GetSpanObjects()
        {
            IncrementRoundIndex();
            return GetRandomSprites();
        }

        public override int GetRoundTime()
        {
            return currentRoundIndex * 3 + 2;
        }
        
        private List<Question> GetRandomSprites()
        {
            List<Question> shuffledSprites = new List<Question>(spanSprites);
            for (int i = 0; i < shuffledSprites.Count; i++)
            {
                int randomIndex = Random.Range(i, shuffledSprites.Count);
                (shuffledSprites[i], shuffledSprites[randomIndex]) = (shuffledSprites[randomIndex], shuffledSprites[i]);
            }
            
            List<Question> selected = new List<Question>();
            for (int i = 0; i < currentRoundIndex; i++)
            {
                selected.Add(shuffledSprites[i]);
            }

            currentSpanQuestions = selected;
            return selected;
        }
        
    }
}
