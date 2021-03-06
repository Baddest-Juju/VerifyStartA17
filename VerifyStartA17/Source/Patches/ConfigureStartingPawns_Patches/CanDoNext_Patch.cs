﻿using Harmony;
using RimWorld;

namespace VerifyStartA17.Patches.ConfigureStartingPawns_Patches {

    [HarmonyPatch]
    [HarmonyPatch(typeof(Page_ConfigureStartingPawns))]
    [HarmonyPatch("CanDoNext")]
    public static class CanDoNext_Patch {

        public static void Postfix(ref Page __instance, ref bool __result) {
            if (__result == true) {
                __result = ConfigureStartingPawns_Controller.CanDoNext(ref __instance);
            }
        }
    }
}