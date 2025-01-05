# :video_game: Game overview

Tomb of Anubis is a couch-coop game where you play as explorers who
discover a treasure inside a newly found opening in one of the Pyramids of Giza. By taking
some of the treasure, you desecrate the grave and thereby unleash the wrath of Anubis, the
protector of graves. Your goal is to find powerful artifacts that can entomb Anubis.
While navigating through a labyrinth of corridors and chambers, you will encounter traps and obstacles that require cooperation. Can you find all of the artifacts before Anubis finds you?

Visit https://mytombstone.itch.io/toa for a trailer of the game and some screenshots.

The code for the project can be found under src/

![ZDbgwt](https://github.com/user-attachments/assets/eb736f71-89e7-4819-9224-ac0437d2f67c)

# Project build instructions
Clone the project and open the solution file src/TombOfAnubis.sln with Visual Studio 2022 or later. Right-click on Solution 'TombOfAnubis' and click Build solution.

# Building release version

`dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained`

A prebuilt 64-bit x86 executable for Windows can be found under \src\TombOfAnubis\bin\Release\net6.0\win-x64\publish. Select everything in the directory and add to zip file.
