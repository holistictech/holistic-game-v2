using System;
using System.Collections.Generic;
using UnityEngine;

namespace Events.Leaderboard
{
    public class LeaderboardView : MonoBehaviour
    {
        [SerializeField] private RectTransform scrollParent;
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private GameObject blackishPanel;
        [SerializeField] private LeaderboardEntry entryPrefab;

        private List<LeaderboardEntry> _pooledEntries = new List<LeaderboardEntry>();
        private List<LeaderboardEntry> _activeEntries = new List<LeaderboardEntry>();
        private int _poolAmount = 100;

        private void Awake()
        {
            PoolEntries();
        }

        public void ActivateLeaderboard(LeaderboardUserModel[] users)
        {
            if (blackishPanel.gameObject.activeSelf) return;
            leaderboardPanel.gameObject.SetActive(true);
            blackishPanel.gameObject.SetActive(true);
            foreach (var user in users)
            {
                var tempEntry = GetAvailableEntry();
                tempEntry.ConfigureEntry(user);
                _activeEntries.Add(tempEntry);
            }
        }

        public void DisableLeaderboard()
        {
            blackishPanel.gameObject.SetActive(false);
            leaderboardPanel.gameObject.SetActive(false);
            foreach (var element in _activeEntries)
            {
                element.ResetSelf();
                element.gameObject.SetActive(false);
            }
        }

        private void PoolEntries()
        {
            for (int i = 0; i < _poolAmount; i++)
            {
                var tempEntry = Instantiate(entryPrefab, scrollParent);
                _pooledEntries.Add(tempEntry);
            }
        }

        public LeaderboardEntry GetAvailableEntry()
        {
            foreach (var element in _pooledEntries)
            {
                if (!element.gameObject.activeSelf)
                {
                    return element;
                }
            }

            throw new Exception("No suitable leaderboard entry found");
        }
    }
}
