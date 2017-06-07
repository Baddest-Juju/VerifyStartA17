using RimWorld;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Verse;

namespace VerifyStartA17.UI {

    public class Page_VerifyStartAutoRoll : Page {
        private Page_ConfigureStartingPawns callingPage;

        private bool searching = true;

        private List<string> flavorText = new List<string>();

        private string curFlavorText = null;

        private Timer timer;

        private int iterations = 0;

        private int flavorTextChange = 3000;

        public override Vector2 InitialSize {
            get {
                return new Vector2(450f, 150f);
            }
        }

        public override string PageTitle {
            get {
                return "Auto Rolling Colonists";
            }
        }

        public Page_VerifyStartAutoRoll(Page_ConfigureStartingPawns callingPage) {
            this.callingPage = callingPage;
            this.FillFlavorText();
            this.timer = new Timer(delegate (object e) {
                this.ChangeFlavorText();
            }, null, 0, this.flavorTextChange);
        }

        public void ChangeFlavorText() {
            this.curFlavorText = GenCollection.RandomElement<string>(this.flavorText);
        }

        public override void DoWindowContents(Rect rect) {
            Rect rect2 = new Rect(0f, 0f, rect.width, rect.height - 60f);
            Rect rect3 = new Rect(rect.width / 2f - 50f, rect.height - 60f, 100f, 50f);
            string text;
            string text2;
            if (this.searching) {
                checked {
                    if (VerifyStart.Instance.CheckColonists()) {
                        this.searching = false;
                    }
                    else {
                        this.iterations++;
                        this.RollForPawns();
                    }
                    text = string.Concat(new object[]
                    {
                        "Searching for a set of colonists that match your criteria.",
                        Environment.NewLine,
                        "(",
                        this.iterations,
                        ") ",
                        this.curFlavorText
                    });
                    text2 = "Stop";
                }
            }
            else {
                text = "Match found. (" + this.iterations + ")";
                rect3.x = rect3.x - rect3.width / 2f;
                if (Widgets.ButtonText(rect3, "Roll Again", true, false, true)) {
                    this.RollForPawns();
                    this.searching = true;
                }
                rect3.x = rect3.x + rect3.width;
                text2 = "Close";
            }
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect2, text);
            if (Widgets.ButtonText(rect3, text2, true, false, true)) {
                this.Close(true);
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private void RollForPawns() {
            checked {
                for (int i = 0; i < Find.GameInitData.startingPawns.Count; i++) {
                    Pawn pawn = Find.GameInitData.startingPawns[i];
                    pawn = StartingPawnUtility.RandomizeInPlace(pawn);
                }
                this.callingPage.SelectPawn(Find.GameInitData.startingPawns[0]);
            }
        }

        private void FillFlavorText() {
            this.flavorText.Clear();
            this.flavorText.Add("No.");
            this.flavorText.Add("Nope.");
            this.flavorText.Add("Uh..no.");
            this.flavorText.Add("I don't think so.");
            this.flavorText.Add("She's useless.");
            this.flavorText.Add("He's useless");
            this.flavorText.Add("That one will set everything on fire.");
            this.flavorText.Add("She's got one foot in the grave.");
            this.flavorText.Add("He's got one foot in the grave.");
            this.flavorText.Add("Yes!  Wait...no.");
            this.flavorText.Add("Almost.");
            this.flavorText.Add("That one will be Thrumbo kibble.");
            this.flavorText.Add("Worse than the last one.");
            this.flavorText.Add("Crap.  Crap.  More crap.");
            this.flavorText.Add("I'd rather kiss a wookie");
            this.flavorText.Add("Really, Tynan?  Really?");
            this.flavorText.Add("The RNGods don't like you today.");
            this.flavorText.Add("Oh look...another loser!");
            this.flavorText.Add("Reticulating Spli...wait, this isn't the Sims");
            this.flavorText.Add("That one will die to the first raid.");
            this.flavorText.Add("Who let that guy in?");
            this.flavorText.Add("LOL!  Wait, that one wasn't a joke?");
            this.flavorText.Add("I think they are trying to out loser each other.");
            this.flavorText.Add("I don't think we're every going to find the right ones.");
        }
    }
}