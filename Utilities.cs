namespace spotify_chroma
{
    internal class SpotifySongDTO
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string ImageUrl { get; set; }
        public string ID { get; set; }

        public SpotifySongDTO(string id, string name, string author, string imageUrl)
        {
            ID = id;
            Name = name;
            Artist = author;
            ImageUrl = imageUrl;
        }
    }
}