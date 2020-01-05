namespace Sparebeat.Common
{
    class Note : INote
    {
        public string Data { get; set; }

        public Note(string data)
        {
            Data = data;
        }
    }
}
