using UnityEngine;

namespace Final_Survivors.Environment
{
    public static class EnvironmentState
    {
        private static bool isDay = true;

        public static bool GetIsDay()
        {
            return isDay;
        }

        public static void SetIsDay(bool value)
        {
            isDay = value;
        }
    }
}
