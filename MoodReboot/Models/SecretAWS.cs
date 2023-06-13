namespace MoodReboot.Models
{
    public class SecretAWS
    {
        public string BucketPublico { get; set; }
        public string BucketPublicoUrl { get; set; }
        public string BucketPrivado { get; set; }
        public string BucketPrivadoUrl { get; set; }
        public string BucketProfiles { get; set; }
        public string BucketProfilesUrl { get; set; }
        public string RDSConnectionString { get; set; }
        public string EmailServiceUrl { get; set; }
        public string TextModerationUrl { get; set; }
        public string ImageModerationUrl { get; set; }
    }
}
