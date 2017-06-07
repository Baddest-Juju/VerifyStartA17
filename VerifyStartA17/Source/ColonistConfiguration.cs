using RimWorld;
using System.Collections.Generic;
using Verse;

namespace VerifyStartA17 {

    public class ColonistConfiguration {
        public List<ConfigurationLine> config = new List<ConfigurationLine>();

        public enumIsUsing isUsing = enumIsUsing.Colony;

        public ColonistConfiguration() {
            foreach (SkillDef current in DefDatabase<SkillDef>.AllDefsListForReading) {
                ConfigurationLine configurationLine = new ConfigurationLine() {
                    skillName = current.label,
                    skillId = (int)current.index
                };
                this.config.Add(configurationLine);
            }
        }
    }
}