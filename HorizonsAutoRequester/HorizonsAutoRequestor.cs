using System;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace HorizonsAutoRequestor
{
    public class APIrequest
    {
        private static string StartTime = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd");
        private static string StopTime = DateOnly.FromDateTime(DateTime.Today.AddDays(2)).ToString("yyyy-MM-dd");
        private static string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static string EphemStorage = Path.Combine(LocalAppData, "HorizonsRequestor", "Ephemeris");
        private static HttpClient HorizonClient = null;

        public static void Run()
        {
            TxtToDictionary();
        }

        public static void TxtToDictionary()
        {
            Dictionary<string, string> PlanetEphemID = new Dictionary<string, string>();

            string TxtPath = Path.Combine(LocalAppData, "HorizonsRequestor" + Path.DirectorySeparatorChar + "IDs_Planets.txt");

            try
            {
                string[] lines = System.IO.File.ReadAllLines(TxtPath);

                foreach (string line in lines)
                {
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    string[] parts = line.Split(':');

                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();

                        if (!PlanetEphemID.ContainsKey(key))
                        {
                            PlanetEphemID.Add(key, value);
                        }
                        else
                        {
                            Console.WriteLine($"Duplicate key {key} found!");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Bad entry format: {line}");
                    }
                }
            }

            catch (FileNotFoundException)
            {
                Console.WriteLine($"File not found at {TxtPath}, did you move it?");
            }

            CheckForEphemFiles(PlanetEphemID);
        }

        static private void CheckForEphemFiles(Dictionary<string, string> PlanetEphemID)
        {
            Directory.CreateDirectory(EphemStorage);
            string PlanetName;
            int PlanetCode;

            foreach (KeyValuePair<string, string> PlanetEphemCombo in PlanetEphemID)        
            {

                PlanetName = PlanetEphemCombo.Value;
                int KeyToInt = int.Parse(PlanetEphemCombo.Key);
                PlanetCode = KeyToInt;

                if (System.IO.File.Exists(Path.Combine(EphemStorage, PlanetName + ".txt")))
                {
                    Console.WriteLine("File For: " + PlanetName + " exists. Overwriting...");
                    HTTPRequest(PlanetCode, PlanetName, APIrequest.StartTime, APIrequest.StopTime);
                }
                else
                {
                    //using StreamWriter sw = new StreamWriter(Path.Combine(EphemStorage, PlanetName + "_EPHEM.txt"));
                    Console.WriteLine("File for: " + PlanetName + " does not exist. Creating...");
                    HTTPRequest(PlanetCode, PlanetName, APIrequest.StartTime, APIrequest.StopTime);
                }
            }
        }
        static async void HTTPRequest(int Command, string PlanetName, string StartTime, string StopTime)
        {
            string APIURL = "https://ssd.jpl.nasa.gov/api/horizons.api";
            string FullCommand = "?format=text&COMMAND='" + Command + "'";
            string ExtraOptions = "&OBJ_DATA='YES'&MAKE_EPHEM='YES'&EPHEM_TYPE='OBSERVER'&CENTER='500@0'";
            string FullStepSize = "&STEP_SIZE='15%20m'&QUANTITIES='1,18,19'";
            string CompleteAPIURL = APIURL + FullCommand + ExtraOptions + "&START_TIME=" + StartTime + "&STOP_TIME=" + StopTime + FullStepSize;
            string FullFilePath = Path.Combine(EphemStorage, PlanetName + ".txt");


            Console.WriteLine("Sending GET Request to JPL Horizon for: " + PlanetName);

            if (HorizonClient == null)
            {
                HttpClientHandler HorizonHandler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.All
                };
                HorizonClient = new HttpClient(HorizonHandler);
            }

            HttpResponseMessage HorizonResponse = HorizonClient.GetAsync($"{CompleteAPIURL}").Result;

            HorizonResponse.EnsureSuccessStatusCode();

            string HorizonResponseBody = HorizonResponse.Content.ReadAsStringAsync().Result;

            await System.IO.File.WriteAllTextAsync(FullFilePath, HorizonResponseBody);

            Console.WriteLine("GET Request for " + PlanetName + " successful!");
        }
    }
}