using System.Collections.Generic;
using Spans.Skeleton;
using UnityEngine;

namespace Spans
{
    public class ForwardSpanVoiceDescription : SpanController
    {
        [SerializeField] private AudioClip[] clips;

        public override List<object> GetSpanObjects()
        {
            return GetRandomClips();
        }

        private List<object> GetRandomClips()
        {
            List<object> shuffledSprites = new List<object>(clips);
            for (int i = 0; i < shuffledSprites.Count; i++)
            {
                int randomIndex = Random.Range(i, shuffledSprites.Count);
                (shuffledSprites[i], shuffledSprites[randomIndex]) = (shuffledSprites[randomIndex], shuffledSprites[i]);
            }
            
            List<object> selected = new List<object>();
            for (int i = 0; i < currentRoundIndex; i++)
            {
                selected.Add(shuffledSprites[i]);
            }

            currentSpanQuestions = selected;
            return selected;
        }
    }
}
