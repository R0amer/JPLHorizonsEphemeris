All of this is just to pull information from JPL's Horizons API, push it to a text file, then push it to a JSON. In the end, to upload it to a VRC world.
Mostly for setting planets in an orrery to their locations every 15 minutes, but also as a project to try and learn a bit of C#.
This'll also serve as the location for the JSONs with planetary ephemeris data to be pulled by whatever UdonSharp method I write up to yoink it.
But someone might find it useful to pull 24 hours worth of ephemeris from Horizon's and have it pushed to a JSON without it looked horrendous like JPL's already does.