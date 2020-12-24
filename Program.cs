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
            for (int idx = 0; idx < args.Length; idx++)
            {
                switch(args[idx]) {
                    case "-d":
                        idx++;
                        uri = args[idx];
                        break;
                    case "-mt":
                        multithreaded = true;
                        break;
                }
            }
            return new object[] { uri, multithreaded };
        }

        static void Main(string[] args)
        {
            object[] argArray;
            if (args.Length > 0)
                argArray = argLoop(args);
            else
                argArray = argLoop(Console.ReadLine().Split(' '));
            
            bk = new Book((string)argArray[0], true);
            bk.ExportToADL();
            bk.DownloadChapters((bool)argArray[1]);
            bk.onThreadFinish += Bk_onThreadFinish;
            Console.ReadLine();
        }

        private static void Bk_onThreadFinish()
        {
            if (finished)
                return;
            else
                finished = true;

            Console.WriteLine("Exporting to EPUB...");
            bk = new Book(Directory.GetCurrentDirectory() + "\\Downloaded\\" + bk.metaData.name);
            bk.ExportToEPUB();
            ZipFile.CreateFromDirectory(Directory.GetCurrentDirectory() + "\\Epubs\\" + bk.metaData.name, Directory.GetCurrentDirectory() + "\\Epubs\\" + bk.metaData.name + ".epub");

        }
    }
}
