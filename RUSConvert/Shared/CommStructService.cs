namespace RUSConvert.Shared
{
    internal static class CommStructService
    {
        public static string Sanitize(string communication)
        {
            var clean = communication;
            clean = clean.Replace("+", string.Empty);
            clean = clean.Replace("/", string.Empty);
            return clean;
        }
    }
}