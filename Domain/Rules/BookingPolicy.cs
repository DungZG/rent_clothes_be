namespace Domain.Rules;

public static class BookingPolicy
{
    public static bool IsValidDateRange(DateOnly startDate, DateOnly endDate)
    {
        return endDate >= startDate;
    }

    public static int CalculateTotalDays(DateOnly startDate, DateOnly endDate)
    {
        return endDate.DayNumber - startDate.DayNumber + 1;
    }

    public static bool HasSufficientQuantity(int availableQuantity, int requiredQuantity)
    {
        return availableQuantity >= requiredQuantity;
    }

    public static bool CanCancel(string status)
    {
        return status != "cancelled" && status != "completed" && status != "refunded";
    }
}
