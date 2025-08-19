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
    class APIrequest
    {
        private static int Command = 000;
        private static string StartTime = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd");
        private static string StopTime = DateOnly.FromDateTime(DateTime.Today.AddDays(2)).ToString("yyyy-MM-dd");
        static private string EphemStorage = @"C:\Users\Kaldr\Desktop\TemporaryOutputFolder";
        private static HttpClient HorizonClient = null;

        public static Dictionary<int, string> PlanetEphemID = new Dictionary<int, string>()
        {
            {199, "Mercury"},
            {299, "Venus"},
            {399, "Earth"},
            {499, "Mars"},
            {599, "Jupiter"},
            {699, "Saturn"},
            {799, "Uranus"},
            {899, "Neptune"},
        };

        static void Main()
        {
            CheckForEphemFiles();
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

            if(HorizonClient == null)
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

        static private void CheckForEphemFiles()
        {
            string PlanetName;
            int PlanetCode;

            foreach (KeyValuePair<int, string> PlanetEphemCombo in PlanetEphemID)        
            {

                PlanetName = PlanetEphemCombo.Value;
                PlanetCode = PlanetEphemCombo.Key;

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
    }
}