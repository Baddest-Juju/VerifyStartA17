using RimWorld;
using UnityEngine;

namespace VerifyStartA17.Patches.ConfigureStartingPawns_Patches {

    public static class DoWindowContents_Patch {

        public static void Postfix(ref Page __instance, ref Rect rect) {
            ConfigureStartingPawns_Controller.DoWindowContents(ref __instance, rect);
        }
    }
}