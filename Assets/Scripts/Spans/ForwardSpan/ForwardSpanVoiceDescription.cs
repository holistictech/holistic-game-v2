using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spans.ForwardSpan
{
    public class ForwardSpanVoiceDescription : SpanController
    {
        public override List<Question> GetSpanObjects()
        {
            return GetRandomClips();
        }
        
        public override float GetRoundTime()
        {
            return currentRoundIndex * 3 + 2;
        }

        private List<Question> GetRandomClips()
        {
            List<Question> shuffledSprites = new List<Question>(GetAllAvailableSpanObjects());
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
