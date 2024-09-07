namespace WORLDGAMDEVELOPMENT
{
    internal sealed class AppConfig
    {
        public long FirstAdmin { get; set; }
        public string? BotKeyRelease { get; set; }
        public Dictionary<string, string>? ConnectionStrings { get; set; }
    }
}
