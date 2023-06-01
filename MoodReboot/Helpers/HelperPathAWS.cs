namespace MoodReboot.Helpers
{
    public enum Containers { ProfileImages, PublicContent, PrivateContent }

    public static class HelperPathAWS
    {
        public static string MapBucketName(Containers container)
        {
            string carpeta = "";

            if (container == Containers.ProfileImages)
            {
                carpeta = "moodreboot-profile-images";
            }
            else if (container == Containers.PublicContent)
            {
                carpeta = "moodreboot-public-content";
            }
            else if (container == Containers.PrivateContent)
            {
                carpeta = "moodreboot-private-content";
            }

            return carpeta;
        }
    }
}
