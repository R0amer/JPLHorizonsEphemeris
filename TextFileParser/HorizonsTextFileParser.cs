using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;

namespace TextFileParser
{
    class EphemInformation
    {
        public string Date {  get; set; }
        public string Time { get; set; }
        public string Ra {  get; set; }
        public string Declination { get; set; }
        public float HelioLon { get; set; }
        public float HelioLat { get; set; }
        public float RadiusAU { get; set; }

    }
    public class Parser
    {
        public static void Main()
        {            

        }

        public static void Parse()
        {
            string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string JSONFilePath = Path.Combine(LocalAppData, "HorizonsRequestor", "JSONs");
            string FilePath = Path.Combine(LocalAppData, "HorizonsRequestor", "Ephemeris");

            Directory.CreateDirectory(JSONFilePath);

            string[] NewPlanetFiles = Directory.GetFiles(FilePath).Select(Path.GetFileName).ToArray();

            var EphemInfo = new List<EphemInformation>();

            string StartText = "$$SOE";
            string EndText = "$$EOE";

            for (int PlanetCount = 0; PlanetCount < NewPlanetFiles.Length; PlanetCount++)
            {
                string FullFilePath = Path.Combine(FilePath+ Path.DirectorySeparatorChar + NewPlanetFiles[PlanetCount]);
                string[] EphemFile = File.ReadAllLines(FullFilePath);
                bool WriteLineToggle = false;
                string date = string.Empty;
                string time = string.Empty;
                string ra = string.Empty;
                string dec = string.Empty;
                float helioLon = 0;
                float helioLat = 0;
                float radiusAU = 0;
                EphemInfo.Clear();

                for (int Indexer = 0; Indexer < EphemFile.Length; Indexer++)
                {
                    string line = EphemFile[Indexer];

                    if (line.Contains(StartText))
                    {
                        WriteLineToggle = true;
                        continue;
                    }
                    if (line.Contains(EndText))
                    {
                        WriteLineToggle = false;
                        break;
                    }
                    if (WriteLineToggle && !string.IsNullOrEmpty(line))
                    {
                        date = line.Substring(1, 12).Trim();
                        time = line.Substring(13, 6).Trim();
                        ra = line.Substring(23, 12).Trim();
                        dec = line.Substring(35, 11).Trim();
                        helioLon = float.Parse(line.Substring(49, 8));
                        helioLat = float.Parse(line.Substring(58, 8));
                        radiusAU = float.Parse(line.Substring(68, 14));

                        EphemInfo.Add(new EphemInformation
                        {
                            Date = date,
                            Time = time,
                            Ra = ra,
                            Declination = dec,
                            HelioLon = helioLon,
                            HelioLat = helioLat,
                            RadiusAU = radiusAU,
                        });

                        if (EphemInfo.Count == 96) // Horizon's Ephemeris output for 24hrs at 15m intervals is always 96 lines not including 00:00 the next day.
                        {
                            var FormattedJSON = new JsonSerializerOptions { WriteIndented = true };
                            string FormattedEphemInfo = JsonSerializer.Serialize(EphemInfo, FormattedJSON);
                            string JSONRenamedFiles = Path.GetFileNameWithoutExtension(NewPlanetFiles[PlanetCount]);
                            string FullJSONFilePath = Path.Combine(JSONFilePath + Path.DirectorySeparatorChar + JSONRenamedFiles + ".json");
                            Console.WriteLine($"JSON for {JSONRenamedFiles} created!");
                            System.IO.File.WriteAllText(FullJSONFilePath, FormattedEphemInfo);
                        }
                    }                   
                }
            }
        }
    }
}