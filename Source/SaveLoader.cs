using System.IO;
using System.Linq;
using RimWorld;
using Verse;

namespace ContinueButtonMod
{
    public static class SaveLoader
    {
        public static void LoadLatestSave(FileInfo saveFileInfo)
        {
            // If a saved game file was found
            if (saveFileInfo != null)
            {
                // Display the file name and game version as a silent message if required.
                DisplaySaveGameMessageAndLoadGame(saveFileInfo);
            }
            else
            {
                // If no saved game files were found, and for whatever reason the Continue button is still there, display an in-game message at the top left corner of the screen
                Messages.Message("NoSavedGamesFound".Translate(), MessageTypeDefOf.NeutralEvent);
            }
        }

        // Display a message about the save file when versions mismatch. If not, if mods mismatch.
        // If both don't mismatch it will display a silent message that will last a frame (almost invisible) and run the latest game.
        private static void DisplaySaveGameMessageAndLoadGame(FileInfo saveFileInfo)
        {
            {
                var saveFileVersion = ScribeMetaHeaderUtility.GameVersionOf(saveFileInfo);
                var saveFileMajorMinorVersion = string.Join(".", saveFileVersion.Split('.').Take(2));

                // Compare the save file major and minor version with the current game major and minor version
                var currentGameMajorMinorVersion = string.Join(".", VersionControl.CurrentVersionString.Split('.').Take(2));

                string message = "ContinueLoading".Translate() + " " + Path.GetFileNameWithoutExtension(saveFileInfo.Name);

                // If the save file major and minor version is different from the current game major and minor version, append the "game version mismatch" to the message
                if (saveFileMajorMinorVersion != currentGameMajorMinorVersion)
                {
                    message += " [v" + saveFileMajorMinorVersion + "] {" + "GameVersionMismatch".Translate().ToLower() + "}";

                    // Display the message as a dialog box with a warning message
                    Find.WindowStack.Add(new Dialog_MessageBox(message, 
                                                                "Confirm".Translate(), () => { LoadGame(saveFileInfo); }, 
                                                                "GoBack".Translate(), () => { }, 
                                                                "WantToContinue".Translate(), false, null, null)
                                                                );
                }
                else
                {
                    // Display the message as a silent event and loads the game. It will be stopped by the mod mismatch window as that is vanilla behaviour, allowing to read the message.
                    Messages.Message(message, MessageTypeDefOf.SilentInput);
                    LoadGame(saveFileInfo);
                }
            }
        }

        private static void LoadGame(FileInfo saveFileInfo)
        {
            // Load the saved game by calling the CheckVersionAndLoadGame method of the GameDataSaveLoader class
            // The file name of the saved game is passed as an argument without the file extension
            GameDataSaveLoader.CheckVersionAndLoadGame(Path.GetFileNameWithoutExtension(saveFileInfo.Name));
        }
    }
}
