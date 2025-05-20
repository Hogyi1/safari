using System.Collections.Generic;
using UnityEngine;

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
        Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec
    }

    /// <summary>
    /// Current month index (0-based).
    /// </summary>
    public int month { get; private set; }

    /// <summary>
    /// Total days passed since game start.
    /// </summary>
    public int totalDays { get; private set; }

    /// <summary>
    /// Day count within the current month (0-based).
    /// </summary>
    public int days { get; private set; }

    /// <summary>
    /// Current hour within the day (0-23).
    /// </summary>
    public int hours { get; private set; }

    /// <summary>
    /// Current minute within the hour (0-59).
    /// </summary>
    public int minutes { get; private set; }

    /// <summary>
    /// Creates a new GameTime instance by copying another instance's values.
    /// </summary>
    /// <param name="gameTime">Existing GameTime to copy.</param>
    public GameTime(GameTime gameTime)
    {
        this.month = gameTime.month;
        this.totalDays = gameTime.totalDays;
        this.days = gameTime.days;
        this.hours = gameTime.hours;
        this.minutes = gameTime.minutes;
    }

    /// <summary>
    /// Initializes a new GameTime starting at zero.
    /// </summary>
    public GameTime() { }

    /// <summary>
    /// Advances the time by a specified number of minutes, rolling into hours as needed.
    /// </summary>
    /// <param name="min">Minutes to add.</param>
    public void AddMinutes(int min)
    {
        minutes += min;

        if (minutes >= 60)
        {
            minutes -= 60;
            AddHours(1);
        }

    }

    /// <summary>
    /// Advances the time by a specified number of hours, rolling into days as needed.
    /// </summary>
    /// <param name="h">Hours to add.</param>
    public void AddHours(int h)
    {
        hours += h;

        if (hours >= 24)
        {
            hours -= 24;
            AddDays(1);
        }

    }

    /// <summary>
    /// Advances the time by a specified number of days, rolling into months and notifying observers as needed.
    /// </summary>
    /// <param name="d">Days to add.</param>
    public void AddDays(int d)
    {
        totalDays += d;
        days += d;
        TimeEvents.Instance.NotifyObservers(TimeEvent.Day_passed);

        if (days >= 30)
        {
            days -= 30;
            AddMonth(1);
        }

    }

    /// <summary>
    /// Advances the time by a specified number of months, rolling into years and notifying observers.
    /// </summary>
    /// <param name="m">Months to add.</param>
    public void AddMonth(int m)
    {
        month += m;
        TimeEvents.Instance.NotifyObservers(TimeEvent.Month_passed);

        if (month >= 12)
        {
            month -= 12;
            TimeEvents.Instance.NotifyObservers(TimeEvent.Year_passed);
        }

    }

    /// <summary>
    /// Returns a formatted string representation of the current time.
    /// </summary>
    /// <returns>String in format "{hours}h {(Months)(month / 12)}, Month {month}".</returns>
    public override string ToString()
    {
        return $"{hours}h {(Months)(month / 12)}, Month {month}";
    }
}
