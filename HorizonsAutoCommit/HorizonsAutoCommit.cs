using Octokit;
using System;
using System.IO;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace HorizonsAutoCommiter
{
    class AutoCommit
    {
        public static string GitHubPAT = Environment.GetEnvironmentVariable("HorizonsAccessToken");

        static async Task Main()
        {
            await GitHubUpload();
        }

        public static async Task GitHubUpload()
        {

            var GitHubClient = new GitHubClient(new ProductHeaderValue("HorizonsAutoCommitter"));
            GitHubClient.Credentials = new Credentials(GitHubPAT);
            string JSONFilePath = @"C:\Users\Kaldr\Desktop\TemporaryOutputFolder\JSONs\";
            string[] NewPlanetFiles = Directory.GetFiles(JSONFilePath).Select(Path.GetFileName).ToArray();
            string HorizonsDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)).ToString("yyyy-MM-dd");

            var Owner = "R0amer";
            var RepoName = "JPLHorizonsEphemeris";
            var ExistCheckPath = "JSONs/";


            try
            {
                foreach (string json in NewPlanetFiles)
                {

                    string RepoPath = $"JSONs/{json}";
                    string branch = "main";
                    string FileContent = File.ReadAllText(Path.Combine(JSONFilePath + json));

                    try
                    {
                        var contents = await GitHubClient.Repository.Content.GetAllContents(Owner, RepoName, RepoPath);

                        var ExistingFileInfo = contents.First().Sha;

                        var UpdateFile = new UpdateFileRequest($"Updated {json} for {HorizonsDate}", 
                            FileContent,
                            ExistingFileInfo,
                            branch,
                            true);

                        var update = await GitHubClient.Repository.Content.UpdateFile(Owner, RepoName, RepoPath, UpdateFile);

                    }
                    catch (Octokit.NotFoundException)
                    {
                        var NewFile = new CreateFileRequest($"Added {json} for {HorizonsDate}",
                            FileContent,
                            branch,
                            true);

                        var upload = await GitHubClient.Repository.Content.CreateFile(Owner, RepoName, RepoPath, NewFile);

                        Console.WriteLine($"{json} commited at {upload.Commit.Sha}");
                    }
                }
            }

            catch (Octokit.ApiException ex)
            {
                Console.WriteLine($"GitHub API Error: {ex.Message}");
                Console.WriteLine($"Status Code: {ex.StatusCode}");
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
            }
        }
    }
}