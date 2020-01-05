namespace Sparebeat.Common
{
    class BeatmapInfo
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public BeatmapLevelSet Level { get; set; }

        public int Score { get; set; }
    }
}
