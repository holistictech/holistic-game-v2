using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using UnityEngine.Serialization;

namespace Spans.ForwardSpan
{
    public class ForwardSpanVoiceDescription : SpanController
    {
        [SerializeField] private ClipQuestion[] clips;
        
        protected override void Start()
        {
            base.Start();
            foreach (var question in clips)
            {
                question.SetHasSelected(false);
            }
        }

        public override List<Question> GetSpanObjects()
        {
            return GetRandomClips();
        }
        
        public override Question[] GetAllAvailableSpanObjects()
        {
            return clips;
        }
        
        public override int GetRoundTime()
        {
            return currentRoundIndex * 3 + 2;
        }

        private List<Question> GetRandomClips()
        {
            List<Question> shuffledSprites = new List<Question>(clips);
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
