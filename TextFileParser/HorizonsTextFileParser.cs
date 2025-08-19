using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;

namespace TextFileParser
{
    class BoundaryTokens
    {
        public int StartTokenLine { get; set; }
        public int EndTokenLine { get; set; }

    }
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
    class TextFileParser
    {
        public static void Main()
        {            
            Parse();
        }

        public static void Parse()
        {            


            string FilePath = @"C:\Users\Kaldr\Desktop\TemporaryOutputFolder\";

            //string[] FullPlanetPaths = Directory.GetFiles(FilePath);

            string[] NewPlanetFiles = Directory.GetFiles(FilePath).Select(Path.GetFileName).ToArray();

            var Tokens = new List<BoundaryTokens>();
            var EphemInfo = new List<EphemInformation>();

            string JSONFilePath = @"C:\Users\Kaldr\Desktop\TemporaryOutputFolder\JSONs\";

            string StartText = "$$SOE";
            string EndText = "$$EOE";

            for (int PlanetCount = 0; PlanetCount < NewPlanetFiles.Length; PlanetCount++)
            {
                string FullFilePath = Path.Combine(FilePath + NewPlanetFiles[PlanetCount]);
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

                        //Console.WriteLine($"Date: {date} | Time: {time} | RA: {ra} | Dec: {dec} | HelioLon: {helioLon:F4} | HelioLat: {helioLat:F4} | Radius: {radiusAU:F7}");
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
                        //Console.WriteLine($"Current Entries in EphemInfo: {EphemInfo.Count}");
                        if (EphemInfo.Count == 96) // Horizon's Ephemeris output for 24hrs at 15m intervals is always 96 lines not including 00:00 the next day.
                        {
                            var FormattedJSON = new JsonSerializerOptions { WriteIndented = true };
                            string FormattedEphemInfo = JsonSerializer.Serialize(EphemInfo, FormattedJSON);
                            string JSONRenamedFiles = Path.GetFileNameWithoutExtension(NewPlanetFiles[PlanetCount]);
                            string FullJSONFilePath = Path.Combine(JSONFilePath + JSONRenamedFiles + ".json");
                            Console.WriteLine($"JSON for {JSONRenamedFiles} created!");
                            System.IO.File.WriteAllText(FullJSONFilePath, FormattedEphemInfo);
                        }
                    }                   
                }
            }
        }
    }
}