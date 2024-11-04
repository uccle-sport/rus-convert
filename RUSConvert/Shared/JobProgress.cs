namespace RUSConvert.Shared
{
    public class JobProgress
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public int Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}