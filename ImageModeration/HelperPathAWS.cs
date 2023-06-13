namespace ImageModeration
{
    public enum Containers { ProfileImages, PublicContent, PrivateContent }

    public static class HelperPathAWS
    {
        public static string MapBucketName(Containers container)
        {
            string carpeta = "";

            if (container == Containers.ProfileImages)
            {
                carpeta = "moodreboot-profile-images2";
            }
            else if (container == Containers.PublicContent)
            {
                carpeta = "moodreboot-public-content2";
            }
            else if (container == Containers.PrivateContent)
            {
                carpeta = "moodreboot-private-content2";
            }

            return carpeta;
        }
    }
}
