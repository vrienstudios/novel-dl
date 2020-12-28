using KobeiD.Downloaders;
using KobeiD.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace KobeiD
{
    class Program
    {
        public static Book bk;
        static bool finished;

        private static object[] argLoop(string[] args)
        {
            string uri = string.Empty;
            bool multithreaded = false;
            if (args.Length <= 0)
                args = Console.ReadLine().Split(' ');
            for (int idx = 0; idx < args.Length; idx++)
            {
                switch (args[idx]) {
                    case "-d":
                        idx++;
                        uri = args[idx];
                        break;
                    case "-mt":
                        multithreaded = true;
                        break;
                    case "-h":
                        uri = "-";
                        return new object[] { uri };
                }
            }
            return new object[] { uri, multithreaded };
        }

        static void Main(string[] args)
        {
            object[] argArray;
            A:
            Console.Write("c:");
            argArray = argLoop(args);

            if (((string)argArray[0])[0] == '-')
            {
                Console.WriteLine("     -d {url} - download flag, place the url to the novel toc page.\n     -mt - multi-threading flag. Does not work with odd prime numbers as of right now.");

                if (args.Length > 0)
                    return;
                else
                    goto A;
            }
            bk = new Book((string)argArray[0], true);
            bk.ExportToADL();
            bk.DownloadChapters((bool)argArray[1]);
            bk.onDownloadFinish += DownloadFinish;
            Console.ReadLine();
        }

        private static void DownloadFinish()
        {
            Console.WriteLine("Exporting to EPUB...");
            bk = new Book(Directory.GetCurrentDirectory() + "\\Downloaded\\" + bk.metaData.name);
            bk.ExportToEPUB();
            ZipFile.CreateFromDirectory(Directory.GetCurrentDirectory() + "\\Epubs\\" + bk.metaData.name, Directory.GetCurrentDirectory() + "\\Epubs\\" + bk.metaData.name + ".epub");

        }
    }
}
