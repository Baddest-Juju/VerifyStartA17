using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace VerifyStartA17.UI {

    public class Page_VerifyStartFailed : Page {
        private bool areWeHappy = false;

        private Page_ConfigureStartingPawns originalPage = null;

        public override Vector2 InitialSize {
            get {
                return new Vector2(400f, 550f);
            }
        }

        public override string PageTitle {
            get {
                return "Verify Start";
            }
        }

        public Page_VerifyStartFailed(Page_ConfigureStartingPawns callingPage = null) {
            this.layer = WindowLayer.Dialog;
            this.doCloseX = false;
            this.doCloseButton = true;
            this.closeOnClickedOutside = false;
            this.resizeable = false;
            this.draggable = false;
            this.originalPage = callingPage;
            if (VerifyStart.Instance.CheckColonists()) {
                this.soundAppear = SoundDefOf.MessageBenefit;
                this.areWeHappy = true;
            }
            else {
                this.soundAppear = SoundDefOf.MessageSeriousAlert;
                this.areWeHappy = false;
            }
        }

        public override void DoWindowContents(Rect rect) {
            base.DrawPageTitle(rect);
            Rect rect2 = new Rect(0f, 50f, rect.width, 50f);
            if (this.areWeHappy) {
                Widgets.Label(rect2, "The following is a summary of your colonists stats:");
            }
            else {
                Widgets.Label(rect2, "The following stats are not met by your colonists:");
            }
            float num = 80f;
            float num2 = 25f;
            Rect rect3 = new Rect(0f, num, 150f, num2);
            GUIContent gUIContent = new GUIContent();
            Text.Font = GameFont.Tiny;
            gUIContent.text = "Skillname";
            gUIContent.tooltip = null;
            Widgets.Label(rect3, gUIContent);
            rect3.x = 150f;
            rect3.width = 50f;
            gUIContent.text = "High";
            Widgets.Label(rect3, gUIContent);
            rect3.x = 200f;
            rect3.width = 50f;
            gUIContent.text = "Min";
            Widgets.Label(rect3, gUIContent);
            Text.Font = GameFont.Small;
            List<VerifyStartWarning> list = VerifyStart.Instance.ShowWarnings();
            foreach (VerifyStartWarning current in list) {
                num += num2;
                string tooltip;
                if (current.highestPawn != null) {
                    tooltip = "Owner of Highest Skill: " + current.highestPawnName;
                }
                else {
                    tooltip = "Owner of Highest Skill:  No one!";
                }
                rect3 = new Rect(0f, num, 150f, num2);
                gUIContent.text = current.skillName;
                gUIContent.tooltip = tooltip;
                if (current.passed) {
                    GUI.color = Color.green;
                }
                else {
                    GUI.color = Color.red;
                }
                Widgets.Label(rect3, gUIContent);
                rect3.x = 150f;
                rect3.width = 50f;
                gUIContent.text = Convert.ToString(current.highestSkill);
                Widgets.Label(rect3, gUIContent);
                rect3.x = 200f;
                rect3.width = 50f;
                gUIContent.text = Convert.ToString(current.minSkill);
                Widgets.Label(rect3, gUIContent);
                TipSignal tipSignal = new TipSignal(gUIContent.tooltip);
                rect3.x = 0f;
                rect3.width = rect.width;
                TooltipHandler.TipRegion(rect3, tipSignal);
                if (Widgets.ButtonInvisible(rect3, false)) {
                    if (this.originalPage != null) {
                        if (current.highestPawn != null) {
                            this.originalPage.SelectPawn(current.highestPawn);
                            this.Close(true);
                        }
                        else {
                            Log.Warning("Clicked on a skill that did not pass Pawn data.");
                        }
                    }
                    else {
                        Log.Warning("Page_VerifyStartFailed called without passing original Page.");
                    }
                }
            }
            GUI.color = Color.white;
            rect3.x = rect.width / 2f - 100f;
            rect3.y = rect.height - 70f;
            rect3.width = 200f;
            rect3.height = 35f;
            Widgets.CheckboxLabeled(rect3, "Bypass Check", ref VerifyStart.Instance.bypass, false);
            TipSignal tipSignal2 = new TipSignal("Checking this box will bypass the settings and let you drop with any colonists you currently have.");
            TooltipHandler.TipRegion(rect3, tipSignal2);
        }
    }
}