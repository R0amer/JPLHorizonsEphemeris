# JPL Horizons API Request "Automator"

## Built to pull ephemeris data from JPL's Horizons API, turn it from a .txt into an .json and optionally upload it to GitHub.

### How to Use
Currently you just build it and run ```HorizonsOrchestrator.exe``` from the command line. If it's never been ran before then it'll
create the necessary ```IDs_Planets.txt``` file with the default entries of the 8 planets. It'll then proceed to request these
from the Horizons API and make JSONs out of the resulting txt files.
- All files are created in ```AppData\Local\HorizonsRequestor```.
- As of right now, it only pulls ephemeris information for the next 24 hours in 15 minute increments from 00:00 to 23:45. I'll add a way to select a date range
  and time step in the future.
### Command-Line Arguments
Currently only has one:

```--commit```: Will commit it to your github under ```Owner```, ```RepoName```, and ```RepoPath``` in ```HorizonsAutoCommit.cs```

Change those variables to match your own.
You'll also need to add an environment variable named "HorizonsAccessToken" with your GitHub personal access token.
Alternatively you can replace the ```Environment.GetEnvironmentVariable("HorizonsAccessToken");``` after ```string GitHubPat =```
if you want to hardcode it. Eventually I'll add a check for it and ask for it via command-line, but for now you can either make
the environment variable yourself, or change the string to match.
