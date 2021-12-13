using System.ComponentModel.DataAnnotations;

namespace BookStore.Extensions
{
    public class DateTimeRequiredAttribute : RequiredAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is DateTime dt)
            {
                return dt != DateTime.MinValue;
            }

            return base.IsValid(value);
        }
    }
}
