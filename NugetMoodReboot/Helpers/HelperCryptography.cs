using System.Security.Cryptography;
using System.Text;

namespace NugetMoodReboot.Helpers
{
    public class HelperCryptography
    {
        public static string GenerateSalt()
        {
            Random random = new();
            string salt = "";
            for (int i = 1; i <= 50; i++)
            {
                int aleatorio = random.Next(0, 255);
                char letra = Convert.ToChar(aleatorio);
                salt += letra;
            }
            return salt;
        }

        public static bool CompareArrays(byte[] a, byte[] b)
        {
            bool iguales = true;

            if (a.Length != b.Length)
            {
                iguales = false;
            }
            else
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i].Equals(b[i]) == false)
                    {
                        iguales = false;
                        return iguales;
                    }
                }
            }
            return iguales;
        }

        public static byte[] EncryptPassword(string password, string salt)
        {
            string contenido = password + salt;
            SHA512 sHA512 = SHA512.Create();

            // Convertimos nuestro contenido a bytes
            byte[] salida = Encoding.UTF8.GetBytes(contenido);

            // Iteraciones para nuestro password
            for (int i = 0; i < 107; i++)
            {
                salida = sHA512.ComputeHash(salida);
            }
            sHA512.Clear();
            return salida;
        }
    }
}
