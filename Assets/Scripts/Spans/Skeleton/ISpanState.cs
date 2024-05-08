using System;
using System.Collections.Generic;
using Scriptables.Tutorial;
using UnityEngine;

namespace Spans.Skeleton
{
    public interface ISpanState
    {
        public abstract void Enter(SpanController controller);
        public abstract void Exit();
        public abstract void SwitchNextState();
        public abstract void TryShowStateTutorial();
        public abstract void EnableUIElements();
        public abstract void DisableUIElements();
    }
}
