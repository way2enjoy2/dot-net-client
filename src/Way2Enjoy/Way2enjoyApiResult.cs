namespace Way2enjoy
{
    public class Way2enjoyApiResult
    {
        public Way2enjoyApiInput Input { get; set; }
        public Way2enjoyApiOutput Output { get; set; }
    }

    public class Way2enjoyApiInput
    {
        public int Size { get; set; }
        public string Type { get; set; }
    }

    public class Way2enjoyApiOutput
    {
        public int Size { get; set; }
        public string Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Ratio { get; set; }
        public string Url { get; set; }
    }
}
