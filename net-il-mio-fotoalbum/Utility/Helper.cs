using System.Text.RegularExpressions;

namespace net_il_mio_fotoalbum.Utility
{
    public class Helper
    {
        public static string GetSlugFromString(string? s)
        {
            if (s is null)
                return "";
            else
            {
                //TODO function with REGEX
                string output = Regex.Replace(s, "[^A-Za-z0-9 ]", "");
                return Regex.Replace(output, @"\s+", "-").ToLower();
            }
        }
    }
}
