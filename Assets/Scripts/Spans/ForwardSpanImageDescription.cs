using System;
using System.Collections.Generic;
using Spans.Skeleton;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Spans
{
    public class ForwardSpanImageDescription : SpanController
    {
        [SerializeField] private Sprite[] spanSprites;
        
        public override List<object> GetSpanObjects()
        {
            IncrementRoundIndex();
            return GetRandomSprites();
        }

        public override int GetRoundTime()
        {
            return currentRoundIndex * 3 + 2;
        }
        
        private List<object> GetRandomSprites()
        {
            List<object> shuffledSprites = new List<object>(spanSprites);
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
