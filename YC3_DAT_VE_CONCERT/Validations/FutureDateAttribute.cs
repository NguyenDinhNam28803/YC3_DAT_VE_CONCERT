using System.ComponentModel.DataAnnotations;

namespace YC3_DAT_VE_CONCERT.Validations
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public FutureDateAttribute()
        {
            ErrorMessage = "Event date must be in the future";
        }

        public override bool IsValid(object value)
        {
            if (value == null)
                return true; // [Required] sẽ handle null

            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.UtcNow;
            }

            return false;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsValid(value))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "Date must be in the future");
        }
    }
}
