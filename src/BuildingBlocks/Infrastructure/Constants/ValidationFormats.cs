namespace ViaChatServer.BuildingBlocks.Infrastructure.Constants
{
    public static class ValidationFormats
    {
        public const string ValidEmailRegexFormat = @"^[\w!#$%&'*\-\/=?\^_`{|}~]+(\.[\w!#$%&'*\-\/=?\^_`{|}~]+)*@((([\w]+([-\w]*[\w]+)*\.)+[a-zA-Z]+)|((([01]?[0-9]{1,2}|2[0-4][0-9]|25[0-5]).){3}[01]?[0-9]{1,2}|2[0-4][0-9]|25[0-5]))\z";

        public const string ValidCultureCode = @"^[A-Za-z]{2,3}-[A-Za-z]{2,3}\z";

        public const string FormattableStringRegex = @"({\d+}|{\w+})";
    }
}
