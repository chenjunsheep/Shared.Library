using Microsoft.VisualBasic;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Util.Extension
{
    public static class SecurityExtension
    {
        public static string HexEncrypt(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            string returnValue;
            int stringPos;
            string strTemp;
            string strTemp2;
            string strAnswer = string.Empty;
            stringPos = 1;
            while (stringPos <= value.Length)
            {
                strTemp = value.Substring(stringPos - 1, 1);
                strTemp2 = Conversion.Hex(Strings.Asc(strTemp));
                if (strTemp2.Length == 1)
                    strAnswer = strAnswer + "0" + strTemp2;
                else
                    strAnswer = strAnswer + strTemp2;
                stringPos += 1;
            }

            returnValue = strAnswer;
            return returnValue;
        }

        public static string HexDecrypt(this string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            var strval = string.Empty;
            for (var y = 1; y <= value.Length; y += 2)
            {
                string strtmp = Strings.Mid(value, y, 2);
                strval = strval + Strings.Chr(TypeParser.GetInt32Value(Conversion.Val("&h" + strtmp)));
            }

            return strval;
        }

        public static string Md5Encrypt(this string value)
        {
            using (var md5Hash = MD5.Create())
            {
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(value));

                // Create a new Stringbuilder to collect the bytes and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data and format each one as a hexadecimal string.
                for (var i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
                
        }
    }
}
