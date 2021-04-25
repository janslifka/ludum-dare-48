using UnityEngine;

public static class Utils
{
    const string BestTimeKey = "best_time";

    public static string FormatTime(float time)
    {
        var timeInt = (int) time;
        var minutes = timeInt / 60;
        var seconds = timeInt % 60;
        var milliseconds = (int) (time * 100) % 100;
        var secondsZero = seconds < 10 ? "0" : "";

        return $"{minutes}:{secondsZero}{seconds}.{milliseconds}";
    }

    public static void SaveBestTime(float time)
    {
        var bestTime = GetBestTime();

        if (bestTime <= 0 || time < bestTime)
        {
            PlayerPrefs.SetFloat(BestTimeKey, time);
        }
    }

    public static float GetBestTime()
    {
        return PlayerPrefs.GetFloat(BestTimeKey, 0);
    }
}