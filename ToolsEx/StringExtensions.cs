using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Diacritics.Extensions;

namespace ToolsEx
{
     public static class StringExtensions
    {
        public static string StringToHexString(this string str)
        {
            StringBuilder hex = new StringBuilder();
            foreach (char c in str)
            {
                hex.AppendFormat("{0:X2}", Convert.ToUInt32(c));
            }
            return hex.ToString();
        }
        public static string HexStringToString(this string hex)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < hex.Length; i += 2)
            {
                string hs = hex.Substring(i, 2);
                str.Append((char)Convert.ToByte(hs, 16));
            }
            return str.ToString();
        }
        public static string compound2Unicode(string str)
        {
            str = str.Replace("\u0065\u0309", "\u1EBB"); //ẻ
            str = str.Replace("\u0065\u0301", "\u00E9"); //é
            str = str.Replace("\u0065\u0300", "\u00E8"); //è
            str = str.Replace("\u0065\u0323", "\u1EB9"); //ẹ
            str = str.Replace("\u0065\u0303", "\u1EBD"); //ẽ
            str = str.Replace("\u00EA\u0309", "\u1EC3"); //ể
            str = str.Replace("\u00EA\u0301", "\u1EBF"); //ế
            str = str.Replace("\u00EA\u0300", "\u1EC1"); //ề
            str = str.Replace("\u00EA\u0323", "\u1EC7"); //ệ
            str = str.Replace("\u00EA\u0303", "\u1EC5"); //ễ
            str = str.Replace("\u0079\u0309", "\u1EF7"); //ỷ
            str = str.Replace("\u0079\u0301", "\u00FD"); //ý
            str = str.Replace("\u0079\u0300", "\u1EF3"); //ỳ
            str = str.Replace("\u0079\u0323", "\u1EF5"); //ỵ
            str = str.Replace("\u0079\u0303", "\u1EF9"); //ỹ
            str = str.Replace("\u0075\u0309", "\u1EE7"); //ủ
            str = str.Replace("\u0075\u0301", "\u00FA"); //ú
            str = str.Replace("\u0075\u0300", "\u00F9"); //ù
            str = str.Replace("\u0075\u0323", "\u1EE5"); //ụ
            str = str.Replace("\u0075\u0303", "\u0169"); //ũ
            str = str.Replace("\u01B0\u0309", "\u1EED"); //ử
            str = str.Replace("\u01B0\u0301", "\u1EE9"); //ứ
            str = str.Replace("\u01B0\u0300", "\u1EEB"); //ừ
            str = str.Replace("\u01B0\u0323", "\u1EF1"); //ự
            str = str.Replace("\u01B0\u0303", "\u1EEF"); //ữ
            str = str.Replace("\u0069\u0309", "\u1EC9"); //ỉ
            str = str.Replace("\u0069\u0301", "\u00ED"); //í
            str = str.Replace("\u0069\u0300", "\u00EC"); //ì
            str = str.Replace("\u0069\u0323", "\u1ECB"); //ị
            str = str.Replace("\u0069\u0303", "\u0129"); //ĩ
            str = str.Replace("\u006F\u0309", "\u1ECF"); //ỏ
            str = str.Replace("\u006F\u0301", "\u00F3"); //ó
            str = str.Replace("\u006F\u0300", "\u00F2"); //ò
            str = str.Replace("\u006F\u0323", "\u1ECD"); //ọ
            str = str.Replace("\u006F\u0303", "\u00F5"); //õ
            str = str.Replace("\u01A1\u0309", "\u1EDF"); //ở
            str = str.Replace("\u01A1\u0301", "\u1EDB"); //ớ
            str = str.Replace("\u01A1\u0300", "\u1EDD"); //ờ
            str = str.Replace("\u01A1\u0323", "\u1EE3"); //ợ
            str = str.Replace("\u01A1\u0303", "\u1EE1"); //ỡ
            str = str.Replace("\u00F4\u0309", "\u1ED5"); //ổ
            str = str.Replace("\u00F4\u0301", "\u1ED1"); //ố
            str = str.Replace("\u00F4\u0300", "\u1ED3"); //ồ
            str = str.Replace("\u00F4\u0323", "\u1ED9"); //ộ
            str = str.Replace("\u00F4\u0303", "\u1ED7"); //ỗ
            str = str.Replace("\u0061\u0309", "\u1EA3"); //ả
            str = str.Replace("\u0061\u0301", "\u00E1"); //á
            str = str.Replace("\u0061\u0300", "\u00E0"); //à
            str = str.Replace("\u0061\u0323", "\u1EA1"); //ạ
            str = str.Replace("\u0061\u0303", "\u00E3"); //ã
            str = str.Replace("\u0103\u0309", "\u1EB3"); //ẳ
            str = str.Replace("\u0103\u0301", "\u1EAF"); //ắ
            str = str.Replace("\u0103\u0300", "\u1EB1"); //ằ
            str = str.Replace("\u0103\u0323", "\u1EB7"); //ặ
            str = str.Replace("\u0103\u0303", "\u1EB5"); //ẵ
            str = str.Replace("\u00E2\u0309", "\u1EA9"); //ẩ
            str = str.Replace("\u00E2\u0301", "\u1EA5"); //ấ
            str = str.Replace("\u00E2\u0300", "\u1EA7"); //ầ
            str = str.Replace("\u00E2\u0323", "\u1EAD"); //ậ
            str = str.Replace("\u00E2\u0303", "\u1EAB"); //ẫ
            str = str.Replace("\u0045\u0309", "\u1EBA"); //Ẻ
            str = str.Replace("\u0045\u0301", "\u00C9"); //É
            str = str.Replace("\u0045\u0300", "\u00C8"); //È
            str = str.Replace("\u0045\u0323", "\u1EB8"); //Ẹ
            str = str.Replace("\u0045\u0303", "\u1EBC"); //Ẽ
            str = str.Replace("\u00CA\u0309", "\u1EC2"); //Ể
            str = str.Replace("\u00CA\u0301", "\u1EBE"); //Ế
            str = str.Replace("\u00CA\u0300", "\u1EC0"); //Ề
            str = str.Replace("\u00CA\u0323", "\u1EC6"); //Ệ
            str = str.Replace("\u00CA\u0303", "\u1EC4"); //Ễ
            str = str.Replace("\u0059\u0309", "\u1EF6"); //Ỷ
            str = str.Replace("\u0059\u0301", "\u00DD"); //Ý
            str = str.Replace("\u0059\u0300", "\u1EF2"); //Ỳ
            str = str.Replace("\u0059\u0323", "\u1EF4"); //Ỵ
            str = str.Replace("\u0059\u0303", "\u1EF8"); //Ỹ
            str = str.Replace("\u0055\u0309", "\u1EE6"); //Ủ
            str = str.Replace("\u0055\u0301", "\u00DA"); //Ú
            str = str.Replace("\u0055\u0300", "\u00D9"); //Ù
            str = str.Replace("\u0055\u0323", "\u1EE4"); //Ụ
            str = str.Replace("\u0055\u0303", "\u0168"); //Ũ
            str = str.Replace("\u01AF\u0309", "\u1EEC"); //Ử
            str = str.Replace("\u01AF\u0301", "\u1EE8"); //Ứ
            str = str.Replace("\u01AF\u0300", "\u1EEA"); //Ừ
            str = str.Replace("\u01AF\u0323", "\u1EF0"); //Ự
            str = str.Replace("\u01AF\u0303", "\u1EEE"); //Ữ
            str = str.Replace("\u0049\u0309", "\u1EC8"); //Ỉ
            str = str.Replace("\u0049\u0301", "\u00CD"); //Í
            str = str.Replace("\u0049\u0300", "\u00CC"); //Ì
            str = str.Replace("\u0049\u0323", "\u1ECA"); //Ị
            str = str.Replace("\u0049\u0303", "\u0128"); //Ĩ
            str = str.Replace("\u004F\u0309", "\u1ECE"); //Ỏ
            str = str.Replace("\u004F\u0301", "\u00D3"); //Ó
            str = str.Replace("\u004F\u0300", "\u00D2"); //Ò
            str = str.Replace("\u004F\u0323", "\u1ECC"); //Ọ
            str = str.Replace("\u004F\u0303", "\u00D5"); //Õ
            str = str.Replace("\u01A0\u0309", "\u1EDE"); //Ở
            str = str.Replace("\u01A0\u0301", "\u1EDA"); //Ớ
            str = str.Replace("\u01A0\u0300", "\u1EDC"); //Ờ
            str = str.Replace("\u01A0\u0323", "\u1EE2"); //Ợ
            str = str.Replace("\u01A0\u0303", "\u1EE0"); //Ỡ
            str = str.Replace("\u00D4\u0309", "\u1ED4"); //Ổ
            str = str.Replace("\u00D4\u0301", "\u1ED0"); //Ố
            str = str.Replace("\u00D4\u0300", "\u1ED2"); //Ồ
            str = str.Replace("\u00D4\u0323", "\u1ED8"); //Ộ
            str = str.Replace("\u00D4\u0303", "\u1ED6"); //Ỗ
            str = str.Replace("\u0041\u0309", "\u1EA2"); //Ả
            str = str.Replace("\u0041\u0301", "\u00C1"); //Á
            str = str.Replace("\u0041\u0300", "\u00C0"); //À
            str = str.Replace("\u0041\u0323", "\u1EA0"); //Ạ
            str = str.Replace("\u0041\u0303", "\u00C3"); //Ã
            str = str.Replace("\u0102\u0309", "\u1EB2"); //Ẳ
            str = str.Replace("\u0102\u0301", "\u1EAE"); //Ắ
            str = str.Replace("\u0102\u0300", "\u1EB0"); //Ằ
            str = str.Replace("\u0102\u0323", "\u1EB6"); //Ặ
            str = str.Replace("\u0102\u0303", "\u1EB4"); //Ẵ
            str = str.Replace("\u00C2\u0309", "\u1EA8"); //Ẩ
            str = str.Replace("\u00C2\u0301", "\u1EA4"); //Ấ
            str = str.Replace("\u00C2\u0300", "\u1EA6"); //Ầ
            str = str.Replace("\u00C2\u0323", "\u1EAC"); //Ậ
            str = str.Replace("\u00C2\u0303", "\u1EAA"); //Ẫ
            return str;
        }

        public static string NonUnicode(this string text, bool justLetterAndNumber = false)
        {
            //text = compound2Unicode(text);
            if (!text.IsNormalized())
                text = text.Normalize();
            text = text.RemoveDiacritics();
            text = text.NonUnicode_old();
            if (justLetterAndNumber) text = Regex.Replace(text, "[^a-zA-Z0-9]", "");

            return text;
        }

        public static string NonUnicode_old(this string text)
        {
            string[] arr1 =
            {
                "á",
                "à",
                "ả",
                "ã",
                "ạ",
                "ấ",
                "ầ",
                "ẩ",
                "ẫ",
                "ậ",
                "â",
                @"ắ",
                "ằ",
                "ẳ",
                "ẵ",
                "ặ",
                "ă",
                "đ",
                "é",
                "è",
                "ẻ",
                "ẽ",
                "ẹ",
                "ế",
                "ề",
                "ể",
                "ễ",
                "ệ",
                "ê",
                "í",
                "ì",
                "ỉ",
                "ĩ",
                "ị",
                "ó",
                "ò",
                "ỏ",
                "õ",
                "ọ",
                "ố",
                "ồ",
                "ổ",
                "ỗ",
                "ộ",
                "ô",
                "ớ",
                "ờ",
                "ở",
                "ỡ",
                "ợ",
                "ơ",
                "ú",
                "ù",
                "ủ",
                "ũ",
                "ụ",
                "ứ",
                "ừ",
                "ử",
                "ữ",
                "ự",
                "ư",
                "ý",
                "ỳ",
                "ỷ",
                "ỹ",
                "ỵ"
            };
            string[] arr2 =
            {
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "a",
                "d",
                "e",
                "e",
                "e",
                "e",
                "e",
                "e",
                "e",
                "e",
                "e",
                "e",
                "e",
                "i",
                "i",
                "i",
                "i",
                "i",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "o",
                "u",
                "u",
                "u",
                "u",
                "u",
                "u",
                "u",
                "u",
                "u",
                "u",
                "u",
                "y",
                "y",
                "y",
                "y",
                "y"
            };
            for (var i = 0; i < arr1.Length; i++)
            {
                text = text.Replace(arr1[i], arr2[i]);
                text = text.Replace(arr1[i].ToUpper(), arr2[i].ToUpper());
            }

            return text;
        }
    }
}