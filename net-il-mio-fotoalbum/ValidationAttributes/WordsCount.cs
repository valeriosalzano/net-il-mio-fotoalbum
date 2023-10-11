using System.ComponentModel.DataAnnotations;

namespace net_il_mio_fotoalbum.ValidationAttributes
{
    public class WordsCount : ValidationAttribute
    {
        public int Min { get; set; } = 1;
        public int Max { get; set; } = int.MaxValue;
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            value ??= "";
            string parsedValue = (string)value;
            int wordsCount = parsedValue.Trim().Split(" ").Length;

            if (wordsCount < Min)
                return new ValidationResult($"The field must contain {Min} words at least.");

            if (wordsCount > Max)
                return new ValidationResult($"The field must contain less than {Max} words.");

            return ValidationResult.Success;
        }
    }
}
