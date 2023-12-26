namespace Sparebeat.Common;

public class BeatmapLevelSet
{
    public int Easy { get; set; }

    public int Normal { get; set; }

    public int Hard { get; set; }

    public override string ToString()
    {
        return $"{Easy} / {Normal} / {Hard}";
    }
}
