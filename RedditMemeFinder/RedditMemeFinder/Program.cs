using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Threading;

namespace RedditMemeFinder
{
    class Program
    {
        public const string PATH = "Pages/";
        public const int nPages = 4;
        public string[] posts = new string[nPages * 25];
        static void Main(string[] args)
        {
            //set the page
            string reddit = "http://www.reddit.com";

            WebClient client = new WebClient();
            string html = PATH;

        }
    }
}
