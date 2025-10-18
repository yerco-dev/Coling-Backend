namespace Coling.Domain.Entities.PartialDateManagement;

public class PartialDate : IComparable<PartialDate>
{
    public int Year { get; private set; }
    public int? Month { get; private set; }
    public int? Day { get; private set; }
    private PartialDate() { }
    public PartialDate(int year, int? month = null, int? day = null)
    {
        if (year < 1900 || year > 2100)
            throw new ArgumentException("Año debe estar entre 1900 y 2100", nameof(year));

        if (month.HasValue && (month < 1 || month > 12))
            throw new ArgumentException("Mes debe estar entre 1 y 12", nameof(month));

        if (day.HasValue && (day < 1 || day > 31))
            throw new ArgumentException("Día debe estar entre 1 y 31", nameof(day));

        // Validar día según mes
        if (month.HasValue && day.HasValue)
        {
            var maxDays = DateTime.DaysInMonth(year, month.Value);
            if (day > maxDays)
                throw new ArgumentException($"Día inválido para {month}/{year}", nameof(day));
        }

        Year = year;
        Month = month;
        Day = day;
    }

    public bool IsComplete => Month.HasValue && Day.HasValue;

    public DateTime ToApproximateDate() => new DateTime(Year, Month ?? 1, Day ?? 1);

    public DateTime ToMinDate() => new DateTime(Year, Month ?? 1, Day ?? 1);

    public DateTime ToMaxDate() => new DateTime(
        Year,
        Month ?? 12,
        Day ?? (Month.HasValue ? DateTime.DaysInMonth(Year, Month.Value) : 31)
    );

    public int CompareTo(PartialDate? other)
    {
        if (other == null) return 1;

        var yearComp = Year.CompareTo(other.Year);
        if (yearComp != 0) return yearComp;

        var monthComp = (Month ?? 0).CompareTo(other.Month ?? 0);
        if (monthComp != 0) return monthComp;

        return (Day ?? 0).CompareTo(other.Day ?? 0);
    }

    public override string ToString()
    {
        if (IsComplete) return $"{Day:00}/{Month:00}/{Year}";
        if (Month.HasValue) return $"{Month:00}/{Year}";
        return $"{Year}";
    }

    public override bool Equals(object? obj)
    {
        if (obj is not PartialDate other) return false;
        return Year == other.Year && Month == other.Month && Day == other.Day;
    }

    public override int GetHashCode() => HashCode.Combine(Year, Month, Day);
}
