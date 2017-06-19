using Harmony;
using RimWorld;

namespace VerifyStartA17.Patches.ConfigureStartingPawns_Patches {

    [HarmonyPatch]
    [HarmonyPatch(typeof(Page_ConfigureStartingPawns))]
    [HarmonyPatch("PreOpen")]
    public static class PreOpen_Patch {

        public static void Prefix() {
            ConfigureStartingPawns_Controller.PreOpen();
        }
    }
}