# SaveScreenshotToDifferentDirectory

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for [Neos VR](https://neos.com/).

Created due to severe issues with Steam saving screenshots unreliably on Linux, but should work on Windows as well. 

## Usage
1. Install [NeosModLoader](https://github.com/neos-modding-group/NeosModLoader).
2. Place [SaveScreenshotToDifferentDirectory.dll](https://github.com/Meister1593/SaveScreenshotToDifferentDirectory/releases/latest) into your `nml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR` on windows or `$HOME/.steam/steam/steamapps/common/NeosVR` for a default install on linux.
3. Start the game. Change default folder to something you would like in NeosModLoader settings under mod tab.

Do note that this might not work with Standalone client, as it hooks up to Steam part of the NeosVR codebase.

If you use Proton on Linux, any type of path will work as long as it can reach that folder: 
 * with forward slashes only
 * backward slashes only 
 * full Windows-like path)