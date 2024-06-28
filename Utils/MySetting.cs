using System.Linq.Expressions;
using System.Text;

namespace HShop.Utils
{
    public class MySetting
    {
        // Create Key to hash password
        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM!~?-+";

            var sb = new StringBuilder();
            var rd = new Random();

            for (int i = 0; i < length; i++)
            {
                sb.Append(pattern[rd.Next(0, pattern.Length)]);
            }

            return sb.ToString();
        }

        public static string UploadImage(IFormFile Hinh, string folder)
        {
            try
            {
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Hinh", folder, Hinh.FileName);

                using (var myfile = new FileStream(fullPath, FileMode.CreateNew))
                {
                    Hinh.CopyTo(myfile);
                }

                return Hinh.FileName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }
    }
}
