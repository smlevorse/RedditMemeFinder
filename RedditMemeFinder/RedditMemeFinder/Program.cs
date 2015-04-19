using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace RedditMemeFinder
{
    class Program
    {
        public const int nPages = 4;
        public static string[] posts = new string[nPages * 25];
        public static Dictionary<string, int> wordTally;
        static void Main(string[] args)
        {
            DownloadAndParse();
            ReplaceNonAlphaNumeric();
            wordTally = new Dictionary<string, int>();
            SplitAndTally();
            WriteData();

            Console.WriteLine("\nDone!");

        }//main

        public static void DownloadAndParse()
        {

            //set the page
            string reddit = "http://www.reddit.com/";

            WebClient client = new WebClient();
            string htmlFileLoc = AppDomain.CurrentDomain.BaseDirectory;
            string redditHTML;      //the entire html text of the front page of reddit
            int index;              //my current location in the file
            int end;                //the last location in the string that I want to read
            int postCount = 0;      //The number of titles saved
            int pageCount = 0;

            string title = "";

            while (pageCount < nPages)
            {
                //Download Reddit
                try
                {
                    Console.WriteLine("Downloading reddit front page...");
                    redditHTML = client.DownloadString(reddit);
                    end = redditHTML.Length;
                }
                catch (WebException e)
                {
                    Console.WriteLine("AN error occurred while downloading reddit: " + e.Message);
                    pageCount = nPages;
                    continue;
                }
                

                //read the html file
                //redditHTML = File.ReadAllText(htmlFileLoc + "page.html");
                Console.WriteLine("Reddit Downloaded.\nParsing page {0}...", (pageCount + 1));

                //find the titles
                while (postCount < 25 * (pageCount + 1))
                {
                    //find the index of the post in the html file
                    index = redditHTML.IndexOf("<span class=\"rank\">" + (postCount + 1));

                    //find the index of the title
                    index = redditHTML.IndexOf("<a class=\"title", index);
                    index = redditHTML.IndexOf(">", index) + 1;
                    end = redditHTML.IndexOf("<", index);
                    title = redditHTML.Substring(index, end - index);
                    posts[postCount] = title;
                    Console.WriteLine("\tFound title {0}: {1}", (++postCount + pageCount * 25), title);

                }


                Console.WriteLine("Page {0} parsed.", (pageCount + 1));

                //move on to next page
                index = redditHTML.IndexOf("rel=\"nofollow next\"");
                index = index - 100;
                index = redditHTML.IndexOf("href=", index) + 6;
                end = redditHTML.IndexOf('"', index);
                reddit = redditHTML.Substring(index, end - index);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(reddit);
                Console.ForegroundColor = ConsoleColor.Gray;
                //reddit = string.Format("http://www.reddit.com/?count={0}", pageCount * 25);
                pageCount++;
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nPrinting Titles\n");
            Console.ForegroundColor = ConsoleColor.Gray;

            //Test this code by printing out all of the titles
            for (int i = 0; i < posts.Length; i++)
            {
                Console.WriteLine((i + 1) + ": " + posts[i]);
            }
        }//DownloadAndParse()

        public static void ReplaceNonAlphaNumeric()
        {
            Regex rgx = new Regex("[^a-zA-Z0-9 - ]");
            //string title;
            for(int i = 0; i < posts.Length; i++)
            {
                //title = posts[i];
                //for(int j = 0; j < title.Length; j++)
                //{
                //    if(title[j] != ' ' && (title[j] < 48 || (title[j] > 57 && title[j] < 65) || (title[j] > 90 && title[j] < 97) || title[j] > 122))
                //    {
                //        title.Replace(title[j], ' ');
                //    }
                //}
                //posts[i] = title;
                posts[i] = rgx.Replace(posts[i], " ");
            }
        }//replace non alphanumeric()

        public static void SplitAndTally()
        {
            string[] words;
            foreach(string title in posts)
            {
                words = title.Split(' ');
                foreach(string word in words)
                {
                    if (wordTally.ContainsKey(word) && word != " ")
                        wordTally[word]++;
                    else
                        wordTally.Add(word, 1);
                }
            }
            
        }//SplitAndTally()

        public static void WriteData()
        {

            StreamWriter output = null;
            try
            {
                Console.WriteLine("Writing to file...");
                //write to file for debugging
                output = new StreamWriter("values.csv");
                foreach(string word in wordTally.Keys)
                {
                    if(wordTally[word] > 2)
                        output.WriteLine(word + "," + wordTally[word]);
                }

                Console.WriteLine("Written");
            }
            catch (Exception fileE)
            {
                Console.WriteLine("An error occurred probably while writing the file:\n" + fileE.Message);
            }
            finally
            {
                if (output != null)
                    output.Close();
            }
        }//WriteData()
    }//class program
}
