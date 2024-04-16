using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Samples.Whisper;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Spans.Skeleton
{
    public class SpanAnswerState : MonoBehaviour, ISpanState
    {
        private SpanController _spanController;
        
        public virtual void Enter(SpanController spanController)
        {
        }


        public virtual void Exit()
        {
        }

        public virtual void SwitchNextState()
        {
            _spanController.SwitchState();
        }
        

        public virtual void EnableUIElements()
        {
        }

        public virtual void DisableUIElements()
        {
        }
    }
}
