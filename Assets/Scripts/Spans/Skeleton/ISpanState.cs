using System.Collections.Generic;
using UnityEngine;

namespace Spans.Skeleton
{
    public interface ISpanState
    {
        public abstract void Enter(SpanController spanController);
        public abstract void Exit();
        public abstract void SwitchNextState();
        public abstract List<GameObject> GetTutorialObjects();
        public abstract void TryShowStateTutorial();
        public abstract void EnableUIElements();
        public abstract void DisableUIElements();
    }
}
