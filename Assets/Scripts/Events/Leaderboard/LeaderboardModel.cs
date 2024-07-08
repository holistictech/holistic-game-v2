using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utilities;

namespace Events.Leaderboard
{
    public class LeaderboardModel
    {
        private List<LeaderboardUserModel> _leaderboard;
        private LeaderboardUserModel _currentLocalEntry;
        private int _currentIndex = 100;
        private int _rankIncrease = 0;
        
        public LeaderboardModel()
        {
            _leaderboard = GetLeaderboard();
            _currentLocalEntry = new LeaderboardUserModel(PlayerInventory.Instance.Username,
                PlayerInventory.Instance.Performance, true);
            AddLocalUserToLeaderboard(_currentLocalEntry);
            _currentIndex = _leaderboard.FindIndex(user => user == _currentLocalEntry);
        }

        private void CalculateRankIncrease()
        {
            var score = PlayerInventory.Instance.Performance;
            var counter = 0;
            for (int i = _currentIndex; i > 0; i--)
            {
                if (score >= _leaderboard[i].Score)
                {
                    counter++;
                }
            }

            _rankIncrease = counter;
        }
        
        public void UpdateLeaderboard()
        {
            _leaderboard.Remove(_currentLocalEntry);
            AddLocalUserToLeaderboard(new LeaderboardUserModel(PlayerInventory.Instance.Username, PlayerInventory.Instance.Performance, true));
        }

        public List<LeaderboardUserModel> GetCurrentLeaderboard()
        {
            return _leaderboard;
        }
        
        private void AddLocalUserToLeaderboard(LeaderboardUserModel localUser)
        {
            int index = _leaderboard.FindIndex(user => user.Score < localUser.Score);
            if (index == -1)
            {
                _leaderboard.Add(localUser);
            }
            else
            {
                _leaderboard.Insert(index, localUser);
            }
        }
        
        private List<LeaderboardUserModel> GetLeaderboard()
        {
            List<LeaderboardUserModel> users = new List<LeaderboardUserModel>();

            TextAsset csvFile = Resources.Load<TextAsset>($"dummyLeaderboard");
            if (csvFile == null)
            {
                Debug.LogError("CSV file not found");
                return users;
            }

            StringReader reader = new StringReader(csvFile.text);
            string line;
            bool headerSkipped = false;
        
            while ((line = reader.ReadLine()) != null)
            {
                // Skip the header
                if (!headerSkipped)
                {
                    headerSkipped = true;
                    continue;
                }

                string[] values = line.Split(',');
                if (values.Length == 2)
                {
                    string username = values[0];
                    int score = int.Parse(values[1]);
                    bool isLocal = bool.Parse(values[2]);
                    users.Add(new LeaderboardUserModel(username, score, isLocal));
                }
            }
            return users;
        }
    }
    
    public class LeaderboardUserModel
    {
        internal string Username;
        internal int Score;
        internal bool IsLocal;

        public LeaderboardUserModel(string userName, int score, bool isLocal)
        {
            Username = userName;
            Score = score;
            IsLocal = isLocal;
        }
    }
}
