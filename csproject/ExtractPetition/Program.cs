using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtractPetition
{
    class Program
    {
        static void Main(string[] args)
        {
            Stat stat = new Stat();

            string[] files = Directory.GetFiles("./html", "*.htm");
            foreach (var path in files)
            {                
                stat.AddHtml(path);                
            }
            stat.ShowNames();
        }
    }
}
