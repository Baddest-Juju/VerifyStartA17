using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VerifyStartA17.UI {

    public class Page_VerifyStartConfiguration : Page {
        private Page originalPage = null;

        private static List<ConfigurationLine> visibleWorkTypesInPriorityOrder = new List<ConfigurationLine>();

        private static DefMap<SkillDef, Vector2> cachedLabelSizes = new DefMap<SkillDef, Vector2>();

        private float workColumnSpacing = -1f;

        private Vector2 scrollPosition = default(Vector2);

        private List<Pawn> pawns;

        public bool searching = false;

        public int iterations = 0;

        public override Vector2 InitialSize {
            get {
                return new Vector2(1200f, 400f);
            }
        }

        public override string PageTitle {
            get {
                return "Verify Start Settings";
            }
        }

        public Page_VerifyStartConfiguration(Page originalPage) {
            this.originalPage = originalPage;
            this.closeOnEscapeKey = true;
        }

        public override void PreClose() {
            base.PreClose();
            VerifyStart.Instance.SaveSettings();
        }

        public override void PreOpen() {
            base.PreOpen();
            Page_VerifyStartConfiguration.visibleWorkTypesInPriorityOrder = new List<ConfigurationLine>();
            using (List<ConfigurationLine>.Enumerator enumerator = VerifyStart.Instance.Config.GetEnumerator()) {
                while (enumerator.MoveNext()) {
                    ConfigurationLine current = enumerator.Current;
                    if (DefDatabase<SkillDef>.AllDefsListForReading.FindIndex((SkillDef x) => (int)x.index == current.skillId) >= 0) {
                        SkillDef skillDef = DefDatabase<SkillDef>.AllDefsListForReading.Find((SkillDef x) => (int)x.index == current.skillId);
                        Page_VerifyStartConfiguration.cachedLabelSizes[skillDef] = Text.CalcSize(current.skillName);
                        Page_VerifyStartConfiguration.visibleWorkTypesInPriorityOrder.Add(current);
                    }
                }
            }
        }

        public override void DoWindowContents(Rect rect) {
            if (this.searching) {
                this.RollColonists(-1);
            }
            base.DrawPageTitle(rect);
            Widgets.Checkbox(new Vector2(rect.width - 235f, 5f), ref VerifyStart.Instance.bypass, 24f, false);
            Widgets.Label(new Rect(rect.width - 205f, 5f, 200f, 25f), "Bypass Check");
            Rect rect2 = new Rect(0f, 60f, rect.width, rect.height - 100f);
            GUI.BeginGroup(rect2);
            Text.Font = GameFont.Small;
            GUI.color = Color.white;
            Rect rect3 = new Rect(0f, 70f, rect2.width, rect2.height - 70f);
            this.workColumnSpacing = (rect2.width - 16f - 201f) / (float)Page_VerifyStartConfiguration.visibleWorkTypesInPriorityOrder.Count;
            float num = 201f;
            for (int i = 0; i < Page_VerifyStartConfiguration.visibleWorkTypesInPriorityOrder.Count; i = checked(i + 1)) {
                ConfigurationLine cline = Page_VerifyStartConfiguration.visibleWorkTypesInPriorityOrder[i];
                if (DefDatabase<SkillDef>.AllDefsListForReading.FindIndex((SkillDef x) => (int)x.index == cline.skillId) >= 0) {
                    SkillDef skillDef = DefDatabase<SkillDef>.AllDefsListForReading.Find((SkillDef x) => (int)x.index == cline.skillId);
                    Vector2 vector = Page_VerifyStartConfiguration.cachedLabelSizes[skillDef];
                    float num2 = num + 15f;
                    Rect rect4 = new Rect(num2 - vector.x / 2f, 0f, vector.x + 20f, vector.y + 5f);
                    if (i % 2 == 1) {
                        rect4.y += 20f;
                    }
                    if (Mouse.IsOver(rect4)) {
                        Widgets.DrawHighlight(rect4);
                    }
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(rect4, cline.skillName);
                    string text = cline.skillName + " " + cline.skillId;
                    TooltipHandler.TipRegion(rect4, new TipSignal(text));
                    GUI.color = new Color(1f, 1f, 1f, 0.3f);
                    Widgets.DrawLineVertical(num2, rect4.yMax - 3f, 50f - rect4.yMax + 3f);
                    Widgets.DrawLineVertical(num2 + 1f, rect4.yMax - 3f, 50f - rect4.yMax + 3f);
                    GUI.color = Color.white;
                    num += this.workColumnSpacing;
                }
            }
            this.pawns = Find.GameInitData.startingPawns;
            Rect rect5 = rect2;
            rect5.height = rect5.height - 100f;
            Rect rect6 = new Rect(0f, 0f, rect5.width - 16f, (float)(checked(this.pawns.Count + 1)) * 30f);
            Widgets.BeginScrollView(rect5, ref this.scrollPosition, rect6);
            float num3 = 0f;
            Rect rect7 = new Rect(0f, num3, rect6.width, 30f);
            GUI.color = new Color(1f, 1f, 1f, 0.2f);
            Widgets.DrawLineHorizontal(0f, num3, rect6.width);
            GUI.color = Color.white;
            this.PreDrawPawnRow(rect7, null, true, -1);
            this.DrawPawnRow(rect7, null, true, -1);
            this.PostDrawPawnRow(rect7, null, true, -1);
            num3 += 30f;
            for (int i = 0; i < this.pawns.Count; i = checked(i + 1)) {
                Pawn p = this.pawns[i];
                rect7 = new Rect(0f, num3, rect6.width, 30f);
                if (num3 - this.scrollPosition.y + 30f >= 0f && num3 - this.scrollPosition.y <= rect5.height) {
                    GUI.color = new Color(1f, 1f, 1f, 0.2f);
                    Widgets.DrawLineHorizontal(0f, num3, rect6.width);
                    GUI.color = Color.white;
                    this.PreDrawPawnRow(rect7, p, false, i);
                    this.DrawPawnRow(rect7, p, false, i);
                    this.PostDrawPawnRow(rect7, p, false, i);
                }
                num3 += 30f;
            }
            Widgets.EndScrollView();
            GUI.EndGroup();
            Text.Anchor = TextAnchor.UpperLeft;
            base.DoBottomButtons(rect, null, null, null, false);
        }

        public void CreateSelector(Rect rect, float curLine, string skillName, ref bool useSkill, ref int minLevel) {
            Rect rect2 = new Rect(26f, curLine, 100f, 30f);
            Rect rect3 = new Rect(136f, curLine, 40f, 30f);
            Rect rect4 = new Rect(116f, curLine, 10f, 20f);
            Rect rect5 = new Rect(186f, curLine, 10f, 20f);
            Widgets.Label(rect2, skillName);
            Widgets.Checkbox(2f, curLine, ref useSkill, 24f, false);
            Widgets.Label(rect3, Convert.ToString(minLevel));
            checked {
                if (Widgets.ButtonImage(rect4, TexUI.ArrowTexLeft)) {
                    if (minLevel > 0) {
                        minLevel--;
                        SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click);
                    }
                    else {
                        SoundStarter.PlayOneShotOnCamera(SoundDefOf.ClickReject);
                    }
                }
                if (Widgets.ButtonImage(rect5, TexUI.ArrowTexRight)) {
                    if (minLevel < 20) {
                        minLevel++;
                        SoundStarter.PlayOneShotOnCamera(SoundDefOf.Click);
                    }
                    else {
                        SoundStarter.PlayOneShotOnCamera(SoundDefOf.ClickReject);
                    }
                }
            }
        }

        private void PreDrawPawnRow(Rect rect, Pawn p, bool colony = false, int pawnIndex = -1) {
            if (Mouse.IsOver(rect)) {
                GUI.DrawTexture(rect, TexUI.HighlightTex);
            }
            Rect rect2 = new Rect(rect.x, rect.y, 145f, rect.height);
            if (p != null && p.health.summaryHealth.SummaryHealthPercent < 0.99f) {
                Rect rect3 = new Rect(rect2);
                rect3.xMin = rect3.xMin - 4f;
                rect3.yMin = rect3.yMin + 4f;
                rect3.yMax = rect3.yMax - 6f;
                Texture2D overlayHealthTex = GenMapUI.OverlayHealthTex;
                Widgets.FillableBar(rect3, p.health.summaryHealth.SummaryHealthPercent, overlayHealthTex, BaseContent.ClearTex, false);
            }
            if (Mouse.IsOver(rect2)) {
                GUI.DrawTexture(rect2, TexUI.HighlightTex);
            }
            string text;
            if (colony) {
                text = "Colony";
            }
            else if (p != null && !p.RaceProps.Humanlike && p.Name != null && !p.Name.Numerical) {
                text = GenText.CapitalizeFirst(p.Name.ToStringShort) + ", " + p.KindLabel;
            }
            else if (p != null) {
                text = p.LabelCap;
            }
            else {
                text = "Unknown";
            }
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;
            Text.WordWrap = false;
            Rect rect4 = rect2;
            rect4.xMin = rect4.xMin + 3f;
            Widgets.Label(rect4, text);
            Text.WordWrap = true;
            if (p != null && Widgets.ButtonInvisible(rect2, false)) {
                ConfigureStartingPawns_Controller.SelectPawnOnPage(originalPage, p);
                this.Close(true);
            }
        }

        protected void DrawPawnRow(Rect rect, Pawn p, bool colony = false, int pawnIndex = -1) {
            Rect rect2 = new Rect(146f, rect.y + 3f, 50f, 25f);
            Rect rect3 = rect2;
            rect3.x = rect3.x - rect2.width;
            List<ConfigurationLine> config;
            if (colony) {
                config = VerifyStart.Instance.Config;
                if (Widgets.ButtonText(rect2, "Roll", true, false, true)) {
                    Find.WindowStack.Add(new Dialog_rolling(this));
                    this.iterations = 0;
                    this.RollColonists(-1);
                }
            }
            else {
                if (VerifyStart.Instance.ColonistConfig.ElementAtOrDefault(pawnIndex) == null) {
                    VerifyStart.Instance.ColonistConfig.Insert(pawnIndex, new ColonistConfiguration());
                }
                if (Widgets.ButtonText(rect2, VerifyStart.Instance.ColonistConfig[pawnIndex].isUsing.ToString(), true, true, true)) {
                    VerifyStart.Instance.ColonistConfig[pawnIndex].isUsing = this.NextValue(VerifyStart.Instance.ColonistConfig[pawnIndex].isUsing);
                }
                string text = string.Concat(new string[]
                {
                    "Colony - Use the colony reroll stats.",
                    Environment.NewLine,
                    "Locked - Do not reroll this colonist but count their stats.",
                    Environment.NewLine,
                    "Ignored = Do not reroll this colonist and ignore their stats"
                });
                TooltipHandler.TipRegion(rect2, text);
                enumIsUsing isUsing = VerifyStart.Instance.ColonistConfig[pawnIndex].isUsing;
                if (isUsing != enumIsUsing.Colony) {
                    config = VerifyStart.Instance.Config;
                }
                else {
                    config = VerifyStart.Instance.Config;
                }
            }
            int num = 0;
            checked {
                using (List<ConfigurationLine>.Enumerator enumerator = config.GetEnumerator()) {
                    while (enumerator.MoveNext()) {
                        ConfigurationLine skill = enumerator.Current;
                        if (DefDatabase<SkillDef>.AllDefsListForReading.FindIndex((SkillDef x) => (int)x.index == skill.skillId) >= 0) {
                            num++;
                            Rect rect4 = unchecked(new Rect(this.workColumnSpacing * (float)num + 126f, rect.y + 3f, 25f, 25f));
                            Text.Anchor = TextAnchor.MiddleCenter;
                            if (colony) {
                                if (!skill.useSkill) {
                                    Widgets.DrawTextureFitted(rect4, Widgets.CheckboxOffTex, 1f);
                                }
                                Widgets.Label(rect4, skill.minLevel.ToString());
                                Widgets.DrawBox(rect4, 1);
                                VerifyStartWarning verifyStartWarning;
                                try {
                                    verifyStartWarning = VerifyStart.Instance.GetWarnings().Find((VerifyStartWarning x) => x.skillId == skill.skillId);
                                }
                                catch (Exception ex) {
                                    verifyStartWarning = null;
                                    Log.Error("Couldn't find the warning in GetWarnings." + Environment.NewLine + ex.ToString());
                                }
                                if (Mouse.IsOver(rect4)) {
                                    if (verifyStartWarning == null || verifyStartWarning.passed) {
                                        Widgets.DrawBoxSolid(rect4, new Color(0.5f, 1f, 0.5f, 0.6f));
                                    }
                                    else {
                                        Widgets.DrawBoxSolid(rect4, new Color(1f, 0.5f, 0.5f, 0.6f));
                                    }
                                }
                                else if (verifyStartWarning == null || verifyStartWarning.passed) {
                                    Widgets.DrawBoxSolid(rect4, new Color(0.5f, 1f, 0.5f, 0.3f));
                                }
                                else {
                                    Widgets.DrawBoxSolid(rect4, new Color(1f, 0.5f, 0.5f, 0.3f));
                                }
                                if (Widgets.ButtonInvisible(rect4, true)) {
                                    if (Input.GetMouseButtonUp(2)) {
                                        skill.useSkill = !skill.useSkill;
                                    }
                                    else if (Input.GetMouseButtonUp(0) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))) {
                                        skill.useSkill = !skill.useSkill;
                                    }
                                    else if (Input.GetMouseButtonUp(1)) {
                                        if (skill.minLevel > 0) {
                                            skill.minLevel--;
                                        }
                                    }
                                    else if (Input.GetMouseButtonUp(0)) {
                                        if (skill.minLevel < 20) {
                                            skill.minLevel++;
                                        }
                                    }
                                }
                                string text2 = string.Concat(new string[]
                                {
                                    "Left Click to increase.",
                                    Environment.NewLine,
                                    "Right click to decrease.",
                                    Environment.NewLine,
                                    "Middle click or Shift click to enable/disable."
                                });
                                TooltipHandler.TipRegion(rect4, text2);
                                Text.Anchor = TextAnchor.UpperLeft;
                            }
                            else {
                                SkillRecord skill2 = Find.GameInitData.startingPawns[pawnIndex].skills.GetSkill(DefDatabase<SkillDef>.AllDefsListForReading.Find((SkillDef x) => (int)x.index == skill.skillId));
                                if (skill2.TotallyDisabled) {
                                    Widgets.Label(rect4, "-");
                                }
                                else {
                                    Widgets.Label(rect4, skill2.Level.ToString());
                                }
                            }
                        }
                    }
                }
            }
        }

        private void PostDrawPawnRow(Rect rect, Pawn p, bool colony = false, int pawnIndex = -1) {
            if (p != null && p.Downed) {
                GUI.color = new Color(1f, 0f, 0f, 0.5f);
                Widgets.DrawLineHorizontal(rect.x, rect.center.y, rect.width);
                GUI.color = Color.white;
            }
        }

        private enumIsUsing NextValue(enumIsUsing currentValue) {
            enumIsUsing result;
            switch (currentValue) {
                case enumIsUsing.Colony:
                    result = enumIsUsing.Locked;
                    break;

                case enumIsUsing.Locked:
                    result = enumIsUsing.Ignored;
                    break;

                default:
                    result = enumIsUsing.Colony;
                    break;
            }
            return result;
        }

        public void RollColonists(int index = -1) {
            checked {
                if (index <= -1) {
                    this.searching = true;
                    bool[] array = new bool[Find.GameInitData.startingPawns.Count];
                    for (int i = 0; i < array.Count<bool>(); i++) {
                        array[i] = false;
                    }
                    if (this.searching) {
                        this.iterations++;
                        for (int i = 0; i < Find.GameInitData.startingPawns.Count; i++) {
                            enumIsUsing enumIsUsing;
                            try {
                                enumIsUsing = VerifyStart.Instance.ColonistConfig[i].isUsing;
                            }
                            catch {
                                enumIsUsing = enumIsUsing.Locked;
                                Log.Error("Tried to get ColonistConfig at index " + i);
                            }
                            switch (enumIsUsing) {
                                case enumIsUsing.Locked:
                                    array[i] = true;
                                    break;

                                case enumIsUsing.Ignored:
                                    array[i] = true;
                                    break;

                                default:
                                    if (!array[i]) {
                                        this.RollColonist(i);
                                    }
                                    break;
                            }
                        }
                        if (VerifyStart.Instance.CheckColonists()) {
                            for (int j = 0; j < Find.GameInitData.startingPawns.Count; j++) {
                                enumIsUsing enumIsUsing;
                                try {
                                    enumIsUsing = VerifyStart.Instance.ColonistConfig[j].isUsing;
                                }
                                catch {
                                    enumIsUsing = enumIsUsing.Colony;
                                }
                                if (enumIsUsing == enumIsUsing.Colony) {
                                    array[j] = true;
                                }
                            }
                        }
                        bool flag = true;
                        bool[] array2 = array;
                        unchecked {
                            for (int k = 0; k < array2.Length; k++) {
                                if (!array2[k]) {
                                    flag = false;
                                }
                            }
                            if (flag) {
                                this.searching = false;
                            }
                        }
                    }
                }
            }
        }

        public void RollColonist(int index) {
            try {
                Pawn pawn = Find.GameInitData.startingPawns[index];
                pawn = StartingPawnUtility.RandomizeInPlace(pawn);
            }
            catch {
                Log.Message("index is " + index);
                Log.Message("Find.GameInitData.startingPawns count is " + Find.GameInitData.startingPawns.Count);
            }
            ConfigureStartingPawns_Controller.SelectPawnOnPage(this.originalPage, Find.GameInitData.startingPawns[0]);
        }
    }
}