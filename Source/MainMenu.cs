using System.Collections.Generic;
using System.IO;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ContinueButtonMod
{
    [HarmonyPatch(typeof(OptionListingUtility), "DrawOptionListing")]
    public class OptionListingUtility_Patch
    {
        private static bool? hasSavedGames = null;
        private static FileInfo latestSaveFile = null;
        private static bool justEnteredMainMenu = false;

        [HarmonyPrefix]
        public static void Prefix(ref List<ListableOption> optList)
        {
            if (Current.ProgramState != ProgramState.Entry)
            {
                justEnteredMainMenu = false;
                return;
            }

            if (!justEnteredMainMenu)
            {
                // Get the most recent saved game file.
                latestSaveFile = GenFilePaths.AllSavedGameFiles.FirstOrDefault();
                hasSavedGames = latestSaveFile != null;
                justEnteredMainMenu = true;
            }

            // If there are no saved games, return. This prevents the Continue button from being added to the main menu if there are no saved games.
            if (!hasSavedGames.Value) return;

            // Iterate over each option in the optList collection
            foreach (ListableOption opt in optList)
            {
                // Check if the option has a non-null action, indicating that are the main menu buttons
                if (opt.action != null)
                {
                    ListableOption newOption = new ListableOption(  // Create a new ListableOption object for the new option button
                    "Continue".Translate(),                         // Label for the new option button
                    delegate { SaveLoader.LoadLatestSave(latestSaveFile); }, // Code to execute when the new option button is clicked
                    null                                            // Tooltip (optional)
                    );
                    optList.Insert(0, newOption);
                    break;
                }
            }
        }
    }
}
