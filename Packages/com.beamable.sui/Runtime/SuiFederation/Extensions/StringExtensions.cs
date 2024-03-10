namespace Beamable.Microservices.SuiFederation
{
    public static class StringExtensions
    {
        public static string GetContentIdName(this string input)
        {
            int lastIndex = input.LastIndexOf('.');
            if (lastIndex == -1)
                return input;
            return input.Substring(lastIndex + 1);
        }
    }
}