namespace MyBestJob.DAL.Constants;

public static partial class Constants
{
    public static class RegexPatterns
    {
        /// <summary>
        /// Minimum 6, maximum 20 characters, at least one uppercase, one lowercase, one number and one special character(@$!%*_=():.?&)
        /// </summary>
        public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*_=():.?&])[A-Za-z\d@$!%*_=():.?&]{6,20}$";

        /// <summary>
        /// Minimum 6, maximum 20 characters, at least one uppercase, one lowercase and one number
        /// </summary>
        public const string EmailPassword = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d]{6,20}$";

        /// <summary>
        /// Pattern for email 'something.some@example.com'
        /// </summary>
        public const string Email = @"[\w'+-]+(\.[\w'+-]+)*@\w+([-.]\w+)*\.\w{2,24}";

        /// <summary>
        /// Between 1 and 30 characters
        /// </summary>
        public const string Name = "^.{1,30}$";

        /// <summary>
        /// Between 1 and 200 characters
        /// </summary>
        public const string Description = "^.{1,200}$";

        /// <summary>
        /// Pattern for hex color
        /// Example: #000 or #000000
        /// </summary>
        public const string HexColor = "^#([a-fA-F0-9]{6}|[a-fA-F0-9]{3})$";

        /// <summary>
        /// Positive numbers only
        /// </summary>
        public const string Number = "^[0-9]+$";

        /// <summary>
        /// Thousand separator for numbers
        /// </summary>
        public const string ThousandSeparator = @"(\d)(?=(\d{3})+(?!\d))";

        /// <summary>
        /// Numbers between 1-1800
        /// </summary>
        public const string Duration = @"^([1-9]|[1-9]\d{1,2}|1[0-7]\d{2}|1800)$";

        /// <summary>
        /// Numbers between 0-60
        /// </summary>
        public const string Wait = "^(60|[1-5]?[0-9])$";

        /// <summary>
        /// Smtp hosts
        /// </summary>
        public const string SmtpHost = @"^(smtp)\.\w+\.[a-zA-z]{1,5}$";

        /// <summary>
        /// Smtp port numbers
        /// </summary>
        public const string SmtpPort = "^(25|465|587|2525)$";

        /// <summary>
        /// Numbers between 0-8
        /// </summary>
        public const string Hours = "^[0-8]$";

        /// <summary>
        /// Numbers between 0-59
        /// </summary>
        public const string Minutes = "^[1-5]?[0-9]$";

        /// <summary>
        /// Phone numbers 6 or 7 digits with format 123-456 or 123-45-67
        /// </summary>
        public const string Phone = "^[0-9]{3}-[0-9]{3}$|^[0-9]{3}-[0-9]{2}-[0-9]{2}$";

        /// <summary>
        /// Digits or hyphen
        /// </summary>
        public const string PhoneNumbers = "^[0-9]$|^-$";

        /// <summary>
        /// Zip code
        /// </summary>
        public const string ZipCode = "^[1-9][0-9]{3}$";

        /// <summary>
        /// Numbers between 10-800
        /// </summary>
        public const string CalculationSize = "^(1[0-9]|[2-9][0-9]|[1-7][0-9]{2}|800)$";

        /// <summary>
        /// Between 1 and 100 characters
        /// </summary>
        public const string EmailTemplateName = "^.{1,100}$";

        /// <summary>
        /// Between 1 and 300 characters
        /// </summary>
        public const string EmailTemplateValue = "^.{1,300}$";

        /// <summary>
        /// Must start with -- and end with -- and between 1 and 100 characters without -- chars
        /// </summary>
        public const string EmailTemplateKey = "^--.{1,100}--$";

        /// <summary>
        /// Between 1 and 50 characters
        /// </summary>
        public const string EmailTemplateSubject = "^.{1,50}$";

        public static Dictionary<string, string> GetRegexPatterns()
            => new()
            {
                ["password"] = Password,
                ["emailPassword"] = EmailPassword,
                ["email"] = Email,
                ["name"] = Name,
                ["hexColor"] = HexColor,
                ["number"] = Number,
                ["thousandSeparator"] = ThousandSeparator,
                ["duration"] = Duration,
                ["wait"] = Wait,
                ["smtpHost"] = SmtpHost,
                ["smtpPort"] = SmtpPort,
                ["hours"] = Hours,
                ["minutes"] = Minutes,
                ["phone"] = Phone,
                ["phoneNumbers"] = PhoneNumbers,
                ["zipCode"] = ZipCode,
                ["description"] = Description,
                ["calculationSize"] = CalculationSize,
                ["emailTemplateName"] = EmailTemplateName,
                ["emailTemplateValue"] = EmailTemplateValue,
                ["emailTemplateKey"] = EmailTemplateKey,
                ["emailTemplateSubject"] = EmailTemplateSubject,
            };
    }
}
