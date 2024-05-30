namespace Utilities
{
    public static class StatisticsHelper
    {
        private static int _displayedQuestionCount;
        private static int _trueCount;


        public static void IncrementTrueCount()
        {
            _trueCount++;
        }

        public static int GetTrueCount()
        {
            return _trueCount;
        }

        public static void IncrementDisplayedQuestionCount()
        {
            _displayedQuestionCount++;
        }

        public static int GetTotalQuestionCount()
        {
            return _displayedQuestionCount;
        }
    }
}
