namespace BookStore.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime SpecifyKindUtc(this DateTime dt)
        {
            if (dt.Kind == DateTimeKind.Utc)
            {
                return dt;
            }

            return DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }

        public static DateTime? SpecifyKindUtc(this DateTime? dt)
        {
            if (dt == null)
            {
                return null;
            }

            return SpecifyKindUtc(dt.Value);
        }
    }
}
