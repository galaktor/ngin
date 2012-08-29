ngin
====

nGin (pronounced: "engine") is a .NET based game engine. I wrote what's there as a proof-of-concept for my thesis. 

The main focus was the loosely coupled architecture, the messaging system and the task scheduling.

It's by no means complete and probably not useful to anyone without serious work. It may however be interesting to poke at or draw inspiration from or learn how not to do certain things.

The project used to be in SVN, hence the directory structure (trunk + tags). Similarly, NuGet did not exist so there's nice little TXT files pointing out which DLL's to put where in order for the build to Just Work.


I just dumped the files here for now, I cannot guarantee that it builds or that the demo runs. It sure used to when I zipped this stuff up, but you never know.

System requirements for demo:
I frankly cannot remember. Nothing crazy. It's compiled for Windows, though. Something along the lines of this should do:
- .NET 3.5 (? not sure, maybe 2.0 would work, too)
- ngin should work under Mono, not sure about the 3rd party deps though. Their newer version should have better Mono support
- Windows (x64 might work, but I'm not sure)
- Any self-respecting ATI or Nvidia graphics device of the past 5 years should handle it with ease. You can tweak Ogre3D in ogre.cfg (i.e. to switch Direct3D/OpenGL, resolution and the likes)
- Mouse and keyboard for input
