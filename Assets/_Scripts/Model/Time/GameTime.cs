using System.Collections.Generic;
using UnityEngine;

public class GameTime
{
    public int month { get; private set; }
    public int totalDays { get; private set; }
    public int days { get; private set; }
    public int hours { get; private set; }
    public int minutes { get; private set; }

    // Ha egy létező GameTime-ból szeretnénk létrehozni egy új időt
    public GameTime(GameTime gameTime)
    {
        this.month = gameTime.month;
        this.totalDays = gameTime.totalDays;
        this.days = gameTime.days;
        this.hours = gameTime.hours;
        this.minutes = gameTime.minutes;
    }
    // Ha egy teljesen új időt akarunk létrehozni
    public GameTime() { }

    public void AddMinutes(int min)
    {
        minutes += min;

        if (minutes >= 60)
        {
            minutes = 0;
            AddHours(1);
        }

    }

    public void AddHours(int h)
    {
        hours += h;

        if (hours >= 24)
        {
            hours = 0;
            AddDays(1);
        }

    }

    public void AddDays(int d)
    {
        totalDays += d;
        days += d;
        TimeEvents.Instance.NotifyObservers(TimeEvent.DAY_PASSED);

        if (days >= 30)
        {
            days = 0;
            AddMonth(1);
        }

    }

    public void AddMonth(int m)
    {
        month += m;
        TimeEvents.Instance.NotifyObservers(TimeEvent.MONTH_PASSED);

        if (month >= 12)
        {
            month = 0;
            TimeEvents.Instance.NotifyObservers(TimeEvent.YEAR_PASSED);
        }

    }

    public override string ToString()
    {
        return $"{days} | {hours}:{minutes}";
    }
}
