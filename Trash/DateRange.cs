namespace Coling.Domain.Entities.PartialDateManagement;

public class DateRange
{
    public PartialDate StartDate { get; private set; } = null!;
    public PartialDate? EndDate { get; private set; }
    private DateRange() { }

    public DateRange(PartialDate startDate, PartialDate? endDate = null)
    {
        StartDate = startDate ?? throw new ArgumentNullException(nameof(startDate));

        if (endDate != null && endDate.CompareTo(startDate) < 0)
            throw new ArgumentException("Fecha de fin debe ser posterior a fecha de inicio", nameof(endDate));

        EndDate = endDate;
    }

    public bool IsCurrent => EndDate == null;

    public int ApproximateDurationInMonths
    {
        get
        {
            var end = EndDate?.ToApproximateDate() ?? DateTime.Now;
            var start = StartDate.ToApproximateDate();
            var totalMonths = ((end.Year - start.Year) * 12) + end.Month - start.Month;
            return totalMonths > 0 ? totalMonths : 0;
        }
    }

    public override string ToString()
    {
        return IsCurrent
            ? $"{StartDate} - Actual"
            : $"{StartDate} - {EndDate}";
    }
}
