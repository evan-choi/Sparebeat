using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sparebeat.Common;

internal sealed class BeatmapInfo : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    public string Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }

    public string Artist
    {
        get => _artist;
        set => SetField(ref _artist, value);
    }

    public BeatmapLevelSet Level
    {
        get => _level;
        set => SetField(ref _level, value);
    }

    public RecordSet RecordSet
    {
        get => _recordSet;
        set => SetField(ref _recordSet, value);
    }

    private string _id;
    private string _title;
    private string _artist;
    private BeatmapLevelSet _level;
    private RecordSet _recordSet;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
