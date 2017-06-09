using Harmony;
using System.Collections.Generic;
using System.Linq;
using VerifyStartA17.Patches.ConfigureStartingPawns_Patches;
using Verse;

namespace VerifyStartA17.Patches {

    public static class HarmonyPatcher {
        private static readonly string[] EdbPrepareCarefullyNames = new string[] { "edb prepare carefully", "preparecarefully" };
        private static readonly string[] EdbPrepareCarefullySteamIDs = new string[] { "735106432", "838528063", "844988411", };

        internal static void ApplyPatches() {
            if (PrepareCarefullyActive()) {
                PatchAllOriginalPages();
            }
            else {
                PatchAllPrepareCarefullyPages();
            }
        }

        private static bool PrepareCarefullyActive() {
            List<ModMetaData> activeMods = ModsConfig.ActiveModsInLoadOrder.ToList().FindAll(m => m.enabled);

            return activeMods != null && activeMods.Find(mod => EdbPrepareCarefullyNames.Contains(mod.Name.ToLower()) ||
                                            EdbPrepareCarefullyNames.Contains(mod.Name.Replace(" ", "").ToLower()) ||
                                            EdbPrepareCarefullySteamIDs.Contains(mod.Identifier) ||
                                            (EdbPrepareCarefullySteamIDs.Contains(mod.GetPublishedFileId().m_PublishedFileId.ToString()))) == null;
        }

        private static void PatchAllPrepareCarefullyPages() {
            VerifyStart.Harmony.Patch(AccessTools.Method(typeof(EdB.PrepareCarefully.Page_ConfigureStartingPawns), "PreOpen"), new HarmonyMethod(AccessTools.Method(typeof(PreOpen_Patch), "Prefix")), null);
            VerifyStart.Harmony.Patch(AccessTools.Method(typeof(EdB.PrepareCarefully.Page_ConfigureStartingPawns), "DoWindowContents"), null, new HarmonyMethod(AccessTools.Method(typeof(DoWindowContents_Patch), "Postfix")));
            VerifyStart.Harmony.Patch(AccessTools.Method(typeof(EdB.PrepareCarefully.Page_ConfigureStartingPawns), "CanDoNext"), null, new HarmonyMethod(AccessTools.Method(typeof(CanDoNext_Patch), "Postfix")));
        }

        private static void PatchAllOriginalPages() {
            VerifyStart.Harmony.Patch(AccessTools.Method(typeof(RimWorld.Page_ConfigureStartingPawns), "PreOpen"), new HarmonyMethod(AccessTools.Method(typeof(PreOpen_Patch), "Prefix")), null);
            VerifyStart.Harmony.Patch(AccessTools.Method(typeof(RimWorld.Page_ConfigureStartingPawns), "DoWindowContents"), null, new HarmonyMethod(AccessTools.Method(typeof(DoWindowContents_Patch), "Postfix")));
            VerifyStart.Harmony.Patch(AccessTools.Method(typeof(RimWorld.Page_ConfigureStartingPawns), "CanDoNext"), null, new HarmonyMethod(AccessTools.Method(typeof(CanDoNext_Patch), "Postfix")));
        }
    }
}