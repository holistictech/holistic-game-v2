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
            leaderboardView.ActivateLeaderboard(_leaderboardModel.GetCurrentLeaderboard());
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

        private void ToggleButton(ToggleUIEvent eventData)
        {
            leaderboardEventButton.gameObject.SetActive(eventData.Toggle);
        }
        
        private void AddListeners()
        {
            leaderboardEventButton.onClick.AddListener(EnableLeaderboard);
            closeButton.onClick.AddListener(DisableLeaderboard);
            playButton.onClick.AddListener(RedirectUserToSpan);
            EventBus.Instance.Register<ToggleUIEvent>(ToggleButton);
        }

        private void RemoveListeners()
        {
            leaderboardEventButton.onClick.RemoveListener(EnableLeaderboard);
            closeButton.onClick.RemoveListener(DisableLeaderboard);
            playButton.onClick.RemoveListener(RedirectUserToSpan);
            EventBus.Instance.Unregister<ToggleUIEvent>(ToggleButton);
        }
    }
}
