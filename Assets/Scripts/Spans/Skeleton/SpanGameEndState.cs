using System.Collections.Generic;
using UnityEngine;

namespace Spans.Skeleton
{
    public class SpanGameEndState : MonoBehaviour, ISpanState
    {
        public void Enter(SpanController spanController)
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
            throw new System.NotImplementedException();
        }

        public void SwitchNextState()
        {
            throw new System.NotImplementedException();
        }

        public List<GameObject> GetTutorialObjects()
        {
            return new List<GameObject>()
            {

            };
        }

        public void TryShowStateTutorial()
        {
            throw new System.NotImplementedException();
        }

        public void EnableUIElements()
        {
            throw new System.NotImplementedException();
        }

        public void DisableUIElements()
        {
            throw new System.NotImplementedException();
        }
    }
}
