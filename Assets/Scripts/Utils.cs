public static class Utils
{
    public static string FormatTime(float time)
    {
        var timeInt = (int) time;
        var minutes = timeInt / 60;
        var seconds = timeInt % 60;
        var milliseconds = (int) (time * 100) % 100;
        var secondsZero = seconds < 10 ? "0" : "";

        return $"{minutes}:{secondsZero}{seconds}.{milliseconds}";
    }
}
