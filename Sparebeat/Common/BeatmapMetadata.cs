namespace Sparebeat.Common
{
    class BeatmapMetadata
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public int Bpm { get; set; }

        public int StartTime { get; set; }

        public BeatmapLevelSet Level { get; set; }

        public NoteSet Map { get; set; } 
    }
}
