namespace MediaCore.Database
{
    public class MediaData
    {

        public int Id { get; set; }
        public byte[] Content { get; set; }
        public byte[] Hash { get; set; }
        public long ContentLength { get; set; }

        public ICollection<MediaItem> MediaItemS { get; set; }
    }
}
