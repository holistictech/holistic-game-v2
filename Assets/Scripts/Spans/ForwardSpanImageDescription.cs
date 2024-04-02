using System;
using System.Collections.Generic;
using Spans.Skeleton;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spans
{
    public class ForwardSpanImageDescription : SpanController
    {
        [SerializeField] private Sprite[] _spanSprites;
        
        public override List<object> GetSpanObjects()
        {
            return GetRandomSprites(currentRoundIndex);
        }
        
        private List<object> GetRandomSprites(int count)
        {
            List<object> shuffledSprites = new List<object>(_spanSprites);
            for (int i = 0; i < shuffledSprites.Count; i++)
            {
                int randomIndex = Random.Range(i, shuffledSprites.Count);
                (shuffledSprites[i], shuffledSprites[randomIndex]) = (shuffledSprites[randomIndex], shuffledSprites[i]);
            }
            
            List<object> selected = new List<object>();
            for (int i = 0; i < count; i++)
            {
                selected.Add(shuffledSprites[i]);
            }

            return selected;
        }
        
    }
}
