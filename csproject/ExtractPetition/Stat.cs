using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ExtractPetition
{
    class PetitionData
    {
        public string count;
    }

    class PetitionDataHolder
    {
        public Dictionary<string, PetitionData> dicTime = new Dictionary<string, PetitionData>();
    }

    class Stat
    {
        Dictionary<string, string> dicSubjects = new Dictionary<string, string>();
        Dictionary<string, PetitionDataHolder> dicData = new Dictionary<string, PetitionDataHolder>();
        List<string> listTime = new List<string>();

        public Stat()
        {

        }

        public void AddHtml(string path)
        {
            string time = Path.GetFileNameWithoutExtension(path);
            listTime.Add(time);

            string fn = Path.GetFileName(path);
            string html = File.ReadAllText(path);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            string xPath_li = "/html[1]/body[1]/div[2]/div[2]/section[2]/div[2]/div[1]/div[2]/div[2]/div[2]/div[2]/ul[1]/li[*]";
            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes(xPath_li);
            Console.WriteLine("items : " + nodes.Count);

            foreach (var linode in nodes)
            {
                var nodea = linode.SelectSingleNode("./div[1]/div[3]/a[1]");
                var nodecount = linode.SelectSingleNode(".//div[1]/div[6]");
                string count = nodecount.InnerText.Replace(",", "").Replace("명", "").Trim();
                string subject = nodea.InnerText;
                string line = time + "," + count + "," + subject;
                Console.WriteLine(line);

                string url = nodea.Attributes["href"].Value;

                PetitionData data = new PetitionData();
                data.count = count;

                dicSubjects[url] = subject;

                if (!dicData.ContainsKey(url))
                {
                    dicData[url] = new PetitionDataHolder();
                }
                dicData[url].dicTime[time] = data;
            }

        }

        public void ShowNames()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("time");
            foreach (string time in listTime)
            {
                string timestr = time.Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":").Substring(0, 16) + ":00";
                sb.Append("," + timestr);
                //sb.Append("," + time.Substring(4, 8));
            }
            sb.AppendLine();

            StringBuilder sbCount = new StringBuilder();

            foreach (string number in dicSubjects.Keys)
            {
                sbCount.Append("\"" + dicSubjects[number].Replace(",","").ToCSVCell() + "\"");
                foreach (string time in listTime)
                {
                    try
                    {
                        PetitionData data = dicData[number].dicTime[time];
                        sbCount.Append("," + data.count);
                    }
                    catch (KeyNotFoundException)
                    {
                        sbCount.Append(",");
                    }
                }
                sbCount.AppendLine();
            }
            File.WriteAllText("bluehouse.csv", sb.ToString() + sbCount.ToString(), new UTF8Encoding(true));
        }
    }

    public static class Ext
    {
        public static string Between(this string source, string left, string right)
        {
            return Regex.Match(source,string.Format("{0}(.*){1}", left, right)).Groups[1].Value;
        }

        // https://stackoverflow.com/questions/6377454/escaping-tricky-string-to-csv-format
        public static string ToCSVCell(this string str)
        {
            bool mustQuote = (str.Contains(",") || str.Contains("\"") || str.Contains("\r") || str.Contains("\n"));
            if (mustQuote)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\"");
                foreach (char nextChar in str)
                {
                    sb.Append(nextChar);
                    if (nextChar == '"')
                        sb.Append("\"");
                }
                sb.Append("\"");
                return sb.ToString();
            }

            return str;
        }
    }

}
