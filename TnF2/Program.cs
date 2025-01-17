using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace TnF2
{
    
    internal class Program
    {
        // Defaults
        static string strPath = @".\";
        static int currentCheckSum = 0;
        static FileSystemWatcher fsw;
        static List<int> CacheList = new List<int> { };


        // Skapa hash på [filnamn + storlek + skapad tid] för att komma runt "buggar" i komponenten FileSystemWatcher.
        private static int CalculateChecksum(string input)
        {
            var _f = new FileInfo(input);

            string _s = _f.FullName + _f.Length.ToString() + _f.CreationTime.ToString();

            Byte[] _b = Encoding.UTF8.GetBytes(_s);

            //Console.WriteLine(_s);

            int sum = 0;
            foreach (Byte c in _b)
            {
                sum += c; // Add ASCII value of each character
            }

            return sum;
        }

        static void PrintHelpAndExit()
        {
            Console.WriteLine("  Commandline arguments:");
            Console.WriteLine("  ======================");
            Console.WriteLine("  -h : Prints this help.");
            Console.WriteLine("  -p : Set working directory for filewatch. (E.g. .\\Dir\\ or full path).");
            Console.WriteLine("");

            System.Environment.Exit(0);
        }

        static bool ReadFile(string FullPath)
        {
            /* Läs in textfil */
            return true;
        }

        static bool WriteToDB()
        {
            /* Spara till databas */
            return true;
        }

        static void UpdateCache(int NewChecksum)
        {
            CacheList.Append(NewChecksum);
            
        }

        static void UpdateCache(int NewChecksum, int OldChecksum)
        {
            if (CacheList.Contains(OldChecksum))
            {
                CacheList.Remove(OldChecksum);
            }
            CacheList.Append(NewChecksum);

           
        }

        static void GetArgs(string[] args)
        {
            
            for (uint i=0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-h":
                        PrintHelpAndExit();
                        break;
                    case "-p":
                        if(i < (args.Length - 1))
                        {
                            i++;
                            strPath = args[i];
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                GetArgs(args);
            }

            fsw = new FileSystemWatcher(strPath);

            Console.Out.WriteLine("Press enter to exit.");
            Log($"Reading directory: {strPath}");

            WatchIt();

            _ = Console.ReadLine();
        }

        static void WatchIt()
        {
            

            fsw.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 |  NotifyFilters.FileName
                                 //| NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 //| NotifyFilters.Size
                                 ;
                                
            fsw.Changed += OnChanged;
            fsw.Created += OnCreated;
            //fsw.Deleted += OnDeleted;
            fsw.Renamed += OnRenamed;
            fsw.Error += OnError;

            fsw.Filter = "*.txt";
            fsw.IncludeSubdirectories = false;
            fsw.EnableRaisingEvents = true;

        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            int _cs = CalculateChecksum(e.FullPath);

            if (_cs != currentCheckSum)
            {
                if (ReadFile(e.FullPath))
                {
                    if(WriteToDB())
                        UpdateCache(_cs, currentCheckSum);
                    currentCheckSum = _cs;
                }
                else
                {
                    Log($"Error reading file {e.FullPath}");
                }
                
                Log($"Changed: {e.Name} :: [{currentCheckSum}]");
                
            }
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {            
            Thread.Sleep(400);
            int _cs = CalculateChecksum(e.FullPath);
            //currentCheckSum = CalculateChecksum(e.FullPath);

            if (_cs != currentCheckSum)
            {
                
                if(ReadFile(e.FullPath))
                {
                    if(WriteToDB())
                        UpdateCache(_cs);
                    currentCheckSum = _cs;
                }
                else
                {
                    Log($"Error reading file {e.FullPath}");
                }

                Log($"Created: {e.Name} :: [{currentCheckSum}]");
                
            }
        }

        private static void OnDeleted(object sender, FileSystemEventArgs e)
        {
           // Används inte
        }
            

        private static void OnRenamed(object sender, RenamedEventArgs e)
        {
            int _cs = CalculateChecksum(e.FullPath);

            if (_cs != currentCheckSum)
            {
                if (ReadFile(e.FullPath))
                {
                    if (WriteToDB())
                        UpdateCache(_cs, currentCheckSum);
                    currentCheckSum = _cs;
                }
                else
                {
                    Log($"Error reading file {e.FullPath}");
                }

                Log($"Renamed:");
                Log($"    Old: {e.OldFullPath}");
                Log($"    New: {e.FullPath} :: [{currentCheckSum}]");
                
            }
        }

        private static void Log(string Message)
        {
            Console.WriteLine(Message);
        }

        private static void OnError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        private static void PrintException(Exception ex)
        {
            if (ex != null)
            {
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine("Stacktrace:");
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine();
                PrintException(ex.InnerException);
            }
        }
    }
}
