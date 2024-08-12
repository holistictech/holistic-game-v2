using System;
using System.Collections.Generic;
using System.IO;
using Spans.Skeleton;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Events.Leaderboard
{
    public class LeaderboardController : MonoBehaviour
    {
        [SerializeField] private Button leaderboardEventButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Button closeButton;
        
        [SerializeField] private LeaderboardView leaderboardView;

        private LeaderboardModel _leaderboardModel;

        private void Start()
        {
            _leaderboardModel = new LeaderboardModel();
        }

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void EnableLeaderboard()
        {
            EventBus.Instance.Trigger(new ToggleSwipeInput(false));
            if (_leaderboardModel == null)
                _leaderboardModel = new LeaderboardModel();
            leaderboardView.ActivateLeaderboard(_leaderboardModel.GetCurrentLeaderboard(), this);
        }
        
        private void HandleOnSpanCompleted(SpanCompletedEvent eventData)
        {
            _leaderboardModel.ConfigureLeaderboard();
            EnableLeaderboard();
            leaderboardView.AnimateLocalEntryToRank(_leaderboardModel.GetCurrentRank());
        }

        private void DisableLeaderboard()
        {
            leaderboardView.DisableLeaderboard();
            EventBus.Instance.Trigger(new ToggleSwipeInput(true));
        }

        private void RedirectUserToSpan()
        {
            EventBus.Instance.Trigger(new SpanRequestedEvent());
        }
        
        private void AddListeners()
        {
            leaderboardEventButton.onClick.AddListener(EnableLeaderboard);
            closeButton.onClick.AddListener(DisableLeaderboard);
            playButton.onClick.AddListener(RedirectUserToSpan);
            EventBus.Instance.Register<SpanCompletedEvent>(HandleOnSpanCompleted);
        }

        private void RemoveListeners()
        {
            leaderboardEventButton.onClick.RemoveListener(EnableLeaderboard);
            closeButton.onClick.RemoveListener(DisableLeaderboard);
            playButton.onClick.RemoveListener(RedirectUserToSpan);
            EventBus.Instance.Unregister<SpanCompletedEvent>(HandleOnSpanCompleted);
        }
    }
}
