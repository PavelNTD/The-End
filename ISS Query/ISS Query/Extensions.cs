using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace ISS_Client
{
    public static class Extensions
    {
        public static string Replace_(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;

            return s.Replace("_", " ");
        }

        public static string ReplaceSpaces(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;

            var regex = new Regex(@"\s{2,}");
            return regex.Replace(s, " ");
        }

        public static string TrimExp(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;

            s = s.Trim();
            return s.Last() == ';' ? s.Substring(0, s.Length - 1) : s;
        }

        private static readonly Thickness paragraphPadding = new Thickness(6);
        private static readonly double documentPageWidth = 1500;

        private static readonly string lineBreak = "\r\n";
        private static readonly string lineBreak1 = "\r\n\t";
        private static readonly string lineBreak2 = "\r\n\t\t";
        private static readonly string lineBreak3 = "\r\n\t\t\t";
        private static readonly string lineBreak4 = "\r\n\t\t\t\t";

        /*public static List<FlowDocument> DocumentView(this IEnumerable<MNSBankAccountsStruct> source)
        {
            var documentList = new List<FlowDocument>();

            var unpGroup = source.GroupBy(x => new { x.УНП_плательщика, x.Наименование_плательщика, x.Адрес_плательщика });
            for (int unpColl = 0; unpColl < unpGroup.Count(); unpColl++)
            {
                var paragraph = new Paragraph { Padding = paragraphPadding };

                paragraph.Inlines.Add(new Bold(new Run($"{nameof(MNSBankAccountsStruct.УНП_плательщика).Replace_()}:")));
                paragraph.Inlines.Add($"{unpGroup.ElementAt(unpColl).Key.УНП_плательщика}");
                paragraph.Inlines.Add(lineBreak);

                paragraph.Inlines.Add(new Bold(new Run($"{nameof(MNSBankAccountsStruct.Наименование_плательщика).Replace_()}:")));
                paragraph.Inlines.Add($"{unpGroup.ElementAt(unpColl).Key.УНП_плательщика}");
                paragraph.Inlines.Add(lineBreak);

                paragraph.Inlines.Add(new Bold(new Run($"{nameof(MNSBankAccountsStruct.Адрес_плательщика).Replace_()}:")));
                paragraph.Inlines.Add($"{unpGroup.ElementAt(unpColl).Key.УНП_плательщика}");
                paragraph.Inlines.Add(lineBreak);

                paragraph.Inlines.Add(new Bold(new Run("Банковские счета: ")));
            }
        }*/

    }
}
