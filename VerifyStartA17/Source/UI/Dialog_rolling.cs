using RimWorld;
using UnityEngine;
using Verse;

namespace VerifyStartA17.UI {

    public class Dialog_rolling : Page {
        private Page_VerifyStartConfiguration callingpage;

        public override Vector2 InitialSize {
            get {
                return new Vector2(300f, 100f);
            }
        }

        public Dialog_rolling(Page_VerifyStartConfiguration calling) {
            this.callingpage = calling;
        }

        public override void DoWindowContents(Rect inRect) {
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(inRect, "Rolling...(" + this.callingpage.iterations + ")");
            Text.Anchor = TextAnchor.UpperLeft;
            if (!this.callingpage.searching) {
                this.Close(true);
            }
        }

        public override void PreClose() {
            this.callingpage.searching = false;
            base.PreClose();
        }
    }
}