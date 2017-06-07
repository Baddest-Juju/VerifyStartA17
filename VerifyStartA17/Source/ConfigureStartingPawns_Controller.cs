using Harmony;
using RimWorld;
using System;
using UnityEngine;
using VerifyStartA17.UI;
using Verse;

namespace VerifyStartA17 {

    public static class ConfigureStartingPawns_Controller {

        public static bool CanDoNext(ref Page original) {
            bool result;
            if (!CheckColonistStats()) {
                Find.WindowStack.Add(new Page_VerifyStartConfiguration(original));
                result = false;
            }
            else {
                foreach (Pawn current in Find.GameInitData.startingPawns) {
                    if (!current.Name.IsValid) {
                        Messages.Message(Translator.Translate("EveryoneNeedsValidName"), MessageSound.RejectInput);
                        result = false;
                        return result;
                    }
                }
                PortraitsCache.Clear();
                result = true;
            }
            return result;
        }

        public static void DoWindowContents(ref Page original, Rect rect) {
            Rect rect2 = new Rect(rect.width - 110f, 2f, 110f, 30f);
            Text.Font = GameFont.Tiny;
            if (Widgets.ButtonText(rect2, "Verify Start", true, false, true)) {
                Find.WindowStack.Add(new Page_VerifyStartConfiguration(original));
            }
            Text.Font = GameFont.Small;
        }

        public static void PreOpen() {
            Log.Message("Loading settings for Verify Start.");
            VerifyStart.Instance.LoadSettings();
        }

        internal static void SelectPawnOnPage(Page original, Pawn pawn) {
            var meth = AccessTools.Method(original.GetType(), "SelectPawn", new Type[] { typeof(Pawn) });
            if (meth == null) {
                Log.Error(string.Format("Could not find method 'SelectPawn' in type of {0}", original.GetType().Name));
            }
            else {
                meth.Invoke(original, new object[] { pawn });
            }
        }

        private static bool CheckColonistStats() {
            return VerifyStart.Instance.CheckColonists();
        }
    }
}