/// <summary>
/// Represents in-game time, tracking minutes, hours, days, months, and total days passed.
/// Provides methods to advance time and notifies observers on day, month, and year transitions.
/// </summary>

[System.Serializable]
public class GameTime
{
    /// <summary>
    /// Enumeration of calendar months.
    /// </summary>
    [System.Serializable]
    public enum Months
    {
        Jun, Jul, Aug, Sep, Oct, Nov, Dec, Jan, Feb, Mar, Apr, May,
    }

    /// <summary>
    /// Current month index (0-based).
    /// </summary>
    public int Month = 0;

    /// <summary>
    /// Total days passed since game start.
    /// </summary>
    public int TotalDays = 0;

    /// <summary>
    /// Day count within the current month (0-based).
    /// </summary>
    public int Days = 0;

    /// <summary>
    /// Current hour within the day (0-23).
    /// </summary>
    public int Hours = 0;

    /// <summary>
    /// Current minute within the hour (0-59).
    /// </summary>
    public int Minutes = 0;

    /// <summary>
    /// Creates a new GameTime instance by copying another instance's values.
    /// </summary>
    /// <param name="gameTime">Existing GameTime to copy.</param>
    public GameTime(GameTime gameTime)
    {
        Month = gameTime.Month;
        TotalDays = gameTime.TotalDays;
        Days = gameTime.Days;
        Hours = gameTime.Hours;
        Minutes = gameTime.Minutes;
    }

    /// <summary>
    /// Initializes a new GameTime starting at zero.
    /// </summary>
    public GameTime()
    {
        Hours = 8;
    }

    /// <summary>
    /// Advances the time by a specified number of minutes, rolling into hours as needed.
    /// </summary>
    /// <param name="min">Minutes to add.</param>
    public void AddMinutes(int min)
    {
        Minutes += min;

        if (Minutes >= 60)
        {
            Minutes -= 60;
            AddHours(1);
        }

    }

    /// <summary>
    /// Advances the time by a specified number of hours, rolling into days as needed.
    /// </summary>
    /// <param name="h">Hours to add.</param>
    public void AddHours(int h)
    {
        Hours += h;

        if (Hours >= 24)
        {
            Hours -= 24;
            AddDays(1);
        }

    }

    /// <summary>
    /// Advances the time by a specified number of days, rolling into months and notifying observers as needed.
    /// </summary>
    /// <param name="d">Days to add.</param>
    public void AddDays(int d)
    {
        TotalDays += d;
        Days += d;
        TimeEvents.Instance.NotifyObservers(TimeEvent.Day_passed);

        if (Days >= 30)
        {
            Days -= 30;
            AddMonth(1);
        }

    }

    /// <summary>
    /// Advances the time by a specified number of months, rolling into years and notifying observers.
    /// </summary>
    /// <param name="m">Months to add.</param>
    public void AddMonth(int m)
    {
        Month += m;
        TimeEvents.Instance.NotifyObservers(TimeEvent.Month_passed);

        if (Month >= 12)
        {
            Month -= 12;
            TimeEvents.Instance.NotifyObservers(TimeEvent.Year_passed);
        }

    }

    /// <summary>
    /// Returns a formatted string representation of the current time.
    /// </summary>
    /// <returns>String in format "{hours}h {(Months)(month / 12)}, Month {month}".</returns>
    public override string ToString()
    {
        return $"{Hours}h {(Months)(Month / 12)}, Month {Month}";
    }
}
