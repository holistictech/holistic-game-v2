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
        private int _userPreviousRank = 99;
        private int _userCurrentRank;
        private int _rankIncrease = 0;

        public LeaderboardModel()
        {
            _leaderboard = GetLeaderboard();

            foreach (var element in _leaderboard)
            {
                Debug.Log($"Leaderboard user: {element.Username}");
            }
        }

        public void ConfigureLeaderboard()
        {
            _currentLocalEntry = new LeaderboardUserModel(PlayerInventory.Instance.Username,
                PlayerInventory.Instance.Performance, true);
            DecideLocalUserRank();
            DecideRankIncrease();
            SetLowerUsersScore();
            SetUpperUsersScore();
            InsertUserToLeaderboard(_userPreviousRank);
        }

        private void DecideLocalUserRank()
        {
            _userPreviousRank = Random.Range(70, 95);
        }

        private void DecideRankIncrease()
        {
            _rankIncrease = Random.Range(1, 5);
        }

        private void SetLowerUsersScore()
        {
            _userCurrentRank = _userPreviousRank - _rankIncrease;
            for (int i = _userCurrentRank + 1; i < _leaderboard.Count; i++)
            {
                var value = PlayerInventory.Instance.Performance - 2;
                _leaderboard[i].Score =  value >= 0 ? value : 1;
            }
        }

        private void SetUpperUsersScore()
        {
            for (int i = _userCurrentRank - 1; i >= 0; i--)
            {
                _leaderboard[i].Score = PlayerInventory.Instance.Performance + 10;
            }
        }

        public int GetCurrentRank()
        {
            return _userCurrentRank;
        }

        public List<LeaderboardUserModel> GetCurrentLeaderboard()
        {
            return _leaderboard;
        }

        private void InsertUserToLeaderboard(int rank)
        {
            _leaderboard[rank] = _currentLocalEntry;
        }

        private List<LeaderboardUserModel> GetLeaderboard()
        {
            List<LeaderboardUserModel> users = new List<LeaderboardUserModel>();

            TextAsset csvFile = Resources.Load<TextAsset>($"dummy_leaderboard");
            if (csvFile == null)
            {
                //Debug.LogError("CSV file not found");
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
                    Debug.Log("Length match");
                    string username = values[0];
                    users.Add(new LeaderboardUserModel(username, 0, false));
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