using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

namespace Events.Leaderboard
{
    public class LeaderboardView : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform scrollParent;
        [SerializeField] private RectTransform viewPort;
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private GameObject blackishPanel;
        [SerializeField] private LeaderboardEntry entryPrefab;

        private List<LeaderboardEntry> _pooledEntries = new List<LeaderboardEntry>();
        private Queue<LeaderboardEntry> _activeEntries = new Queue<LeaderboardEntry>();
        private int _poolAmount = 100;
        private int _topIndex = 0;
        private float _lastScrollPos;

        private float _scrollThreshold;
        private float _accumulatedScrollDelta;
        private void Awake()
        {
            PoolEntries();
            _scrollThreshold = entryPrefab.GetComponent<RectTransform>().rect.height / scrollParent.rect.height;
            Debug.Log("threshold :" + _scrollThreshold);
            scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        private bool _isInitial = true;
        private void OnScrollValueChanged(Vector2 value)
        {
            if (!blackishPanel.gameObject.activeSelf) return;

            var currentScrollPos = value.y;
            if (_isInitial)
            {
                _lastScrollPos = currentScrollPos;
                _isInitial = false;
                return;
            }
            
            float delta = currentScrollPos - _lastScrollPos;
            _accumulatedScrollDelta += Mathf.Abs(delta);

            if (_accumulatedScrollDelta >= _scrollThreshold)
            {
                if (delta > 0)
                {
                    Debug.Log("swiping up");
                    //RecycleBottomToTop();
                }
                else if (delta < 0)
                {
                    Debug.Log("swiping down");
                    //RecycleTopToBottom();
                }

                _accumulatedScrollDelta = 0;
            }
            
            _lastScrollPos = currentScrollPos;
        }
        
        void RecycleBottomToTop()
        {
            if (_topIndex + _poolAmount >=  200)
            {
                return;
            }

            LeaderboardEntry bottomEntry = _activeEntries.Dequeue();
            bottomEntry.gameObject.SetActive(false);

            LeaderboardEntry topEntry = _pooledEntries.Find(e => !e.gameObject.activeSelf);
            if (topEntry != null)
            {
                topEntry.transform.SetAsFirstSibling();
                topEntry.gameObject.SetActive(true);
                _topIndex++;
                topEntry.SetRankField(_topIndex + _poolAmount - 1);
                _activeEntries.Enqueue(topEntry);
            }
        }

        void RecycleTopToBottom()
        {
            if (_topIndex <= 0)
            {
                return;
            }

            LeaderboardEntry topEntry = _activeEntries.Dequeue();
            topEntry.gameObject.SetActive(false);

            LeaderboardEntry bottomEntry = _pooledEntries.Find(e => !e.gameObject.activeSelf);
            if (bottomEntry != null)
            {
                bottomEntry.transform.SetAsLastSibling();
                bottomEntry.gameObject.SetActive(true);
                _topIndex--;
                bottomEntry.SetRankField(_topIndex);
                _activeEntries.Enqueue(bottomEntry);
            }
        }

        public void ActivateLeaderboard(List<LeaderboardUserModel> users)
        {
            if (blackishPanel.gameObject.activeSelf) return;
            leaderboardPanel.gameObject.SetActive(true);
            blackishPanel.gameObject.SetActive(true);
            for (int i = 0; i < users.Count; i++)
            {
                var tempEntry = GetAvailableEntry();
                tempEntry.ConfigureEntry(users[i], i);
                _activeEntries.Enqueue(tempEntry);
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
            _activeEntries.Clear();
        }

        private void PoolEntries()
        {
            for (int i = 0; i < _poolAmount; i++)
            {
                var tempEntry = Instantiate(entryPrefab, scrollParent);
                _pooledEntries.Add(tempEntry);
            }
            
            _lastScrollPos = scrollRect.verticalNormalizedPosition;
        }

        private LeaderboardEntry GetAvailableEntry()
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

        private void OnDestroy()
        {
            scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
        }
    }
}
