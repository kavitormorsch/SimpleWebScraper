using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleWebScraper.Classes
{
    internal record Game
    {
        public string Name { get; set; }
        public string Link { get; set; }

        public Game(string name, string link)
        {
            Name = name;
            Link = link;
        }

    }
}
