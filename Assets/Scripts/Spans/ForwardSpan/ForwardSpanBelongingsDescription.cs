using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;

namespace Spans.ForwardSpan
{
    public class ForwardSpanBelongingsDescription : SpanController
    {
        [SerializeField] private ClipQuestion[] clips;
        public override List<object> GetSpanObjects()
        {
            return GetRandomClips();
        }
        
        public override int GetRoundTime()
        {
            return currentRoundIndex * 3 + 2;
        }

        private List<object> GetRandomClips()
        {
            List<object> shuffledClips = new List<object>(clips);
            for (int i = 0; i < shuffledClips.Count; i++)
            {
                int randomIndex = Random.Range(i, shuffledClips.Count);
                (shuffledClips[i], shuffledClips[randomIndex]) = (shuffledClips[randomIndex], shuffledClips[i]);
            }
            
            List<object> selected = new List<object>();
            for (int i = 0; i < currentRoundIndex; i++)
            {
                selected.Add(shuffledClips[i]);
            }

            currentSpanQuestions = selected;
            return selected;
        }
    }
}
