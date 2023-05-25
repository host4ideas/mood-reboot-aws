namespace MoodReboot.Helpers
{
    public enum Containers { ProfileImages, PublicContent, PrivateContent }

    public static class HelperPathAzure
    {
        public static string MapContainerPath(Containers container)
        {
            string carpeta = "";

            if (container == Containers.ProfileImages)
            {
                carpeta = "profileimages";
            }
            else if (container == Containers.PublicContent)
            {
                carpeta = "publiccontent";
            }
            else if (container == Containers.PrivateContent)
            {
                carpeta = "privatecontent";
            }

            return carpeta;
        }
    }
}
