using System;
using System.Diagnostics;
using System.IO;
using TextFileParser;
using HorizonsAutoRequestor;
using HorizonsAutoCommiter;
using Microsoft.VisualBasic.FileIO;
using System.Runtime.InteropServices;

namespace HorizonsOrchestrator
{
    class OrchestratorClass
    {
        static string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        static string HorizonsFolder = Path.Combine(LocalAppData + Path.DirectorySeparatorChar + "HorizonsRequestor");

        static async Task Main(string[] args)
        {
            HorizonsDirCreate();
            
            await CheckExist();

            APIrequest.Run();

            Parser.Parse();            
            
            if (args.Length == 0)
            {
                Console.WriteLine("--commit flag not added, skipping commit phase...");
            }
            else if (args[0] == "--commit")
            {
                await AutoCommit.Run();
            }
        }

        static void HorizonsDirCreate()
        {
            Directory.CreateDirectory(HorizonsFolder);
        }

        static async Task CheckExist()
        {
            if (!System.IO.File.Exists(APIrequest.TxtPath))
            {

                Console.WriteLine($"File not found at {APIrequest.TxtPath}. It either doesn't exist or was moved, so it's being created...");
                await using (StreamWriter sw = System.IO.File.CreateText(APIrequest.TxtPath))
                {
                    sw.WriteLine("# Make sure each entry is formatted as <ID>:<PlanetName> as below.");
                    sw.WriteLine("# Must have a COMMAND number. i.e. 199 for Mercury, -170 for JWST, 90377 for Sedna.");
                    sw.WriteLine("# Sometimes this is after the body, sometimes it's before.");
                    sw.WriteLine("");
                    sw.WriteLine("199:Mercury");
                    sw.WriteLine("299:Venus");
                    sw.WriteLine("399:Earth");
                    sw.WriteLine("499:Mars");
                    sw.WriteLine("599:Jupiter");
                    sw.WriteLine("699:Saturn");
                    sw.WriteLine("799:Uranus");
                    sw.WriteLine("899:Neptune");
                }
            } 
        }
    }
}