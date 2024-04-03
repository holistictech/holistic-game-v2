using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;

namespace Spans.ForwardSpan
{
    public class ForwardSpanBelongingsDescription : SpanController
    {
        [SerializeField] private ClipQuestion[] clips;
        public override List<Question> GetSpanObjects()
        {
            return GetRandomClips();
        }
        
        public override int GetRoundTime()
        {
            return currentRoundIndex * 3 + 2;
        }

        private List<Question> GetRandomClips()
        {
            List<Question> shuffledClips = new List<Question>(clips);
            for (int i = 0; i < shuffledClips.Count; i++)
            {
                int randomIndex = Random.Range(i, shuffledClips.Count);
                (shuffledClips[i], shuffledClips[randomIndex]) = (shuffledClips[randomIndex], shuffledClips[i]);
            }
            
            List<Question> selected = new List<Question>();
            for (int i = 0; i < currentRoundIndex; i++)
            {
                selected.Add(shuffledClips[i]);
            }

            currentSpanQuestions = selected;
            return selected;
        }
    }
}
