using HarmonyLib;
using Verse;

namespace ContinueButtonMod
{
    [StaticConstructorOnStartup]
    public static class ContinueButton
    {
        static ContinueButton()
        {
            var harmony = new Harmony("com.phoenix.continuebuttonmod");
            harmony.PatchAll();
            Log.Message("[ContinueButtonMod] Initialized.");
        }
    }
}
