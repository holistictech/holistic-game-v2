using UnityEngine;

namespace Events.Leaderboard
{
    public class LeaderboardModel
    {
        private LeaderboardUserModel[] _leaderboard;
        
        public LeaderboardModel(LeaderboardUserModel[] users)
        {
            _leaderboard = users;
        }

        public void UpdateLeaderboard(LeaderboardUserModel[] users)
        {
            _leaderboard = users;
        }

        public LeaderboardUserModel[] GetCurrentLeaderboard()
        {
            return _leaderboard;
        }
    }
    

    public abstract class LeaderboardUserModel
    {
        internal string Username;
        internal bool IsLocal;
    }
}
