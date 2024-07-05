using System;
using Spans.Skeleton;
using UnityEngine;
using UnityEngine.UI;

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
            _leaderboardModel = new LeaderboardModel(GetLeaderboard());
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
            EventBus.Instance.Trigger(new ToggleSwipeInput(true));
            leaderboardView.ActivateLeaderboard(_leaderboardModel.GetCurrentLeaderboard());
            //_eventBus.Trigger(new ToggleSwipeInput(false));
        }

        private LeaderboardUserModel[] GetLeaderboard()
        {
            return new LeaderboardUserModel[] { };
        }

        private void DisableLeaderboard()
        {
            leaderboardView.DisableLeaderboard();
            //_eventBus.Trigger(new ToggleSwipeInput(true));
            EventBus.Instance.Trigger(new ToggleSwipeInput(false));
        }

        private void RedirectUserToSpan()
        {
            //_eventBus.Trigger(new SpanRequestedEvent());
            EventBus.Instance.Trigger(new SpanRequestedEvent());
        }
        
        private void AddListeners()
        {
            leaderboardEventButton.onClick.AddListener(EnableLeaderboard);
            closeButton.onClick.AddListener(DisableLeaderboard);
            playButton.onClick.AddListener(RedirectUserToSpan);
        }

        private void RemoveListeners()
        {
            leaderboardEventButton.onClick.RemoveListener(EnableLeaderboard);
            closeButton.onClick.RemoveListener(DisableLeaderboard);
            playButton.onClick.RemoveListener(RedirectUserToSpan);
        }
    }
}
