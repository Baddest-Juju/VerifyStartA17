using Verse;

namespace VerifyStartA17 {

    public class VerifyStartWarning {
        public string skillName = null;

        public int skillId = -1;

        public int highestSkill = -1;

        public int minSkill = -1;

        public bool passed = false;

        public string highestPawnName;

        public Pawn highestPawn;

        public VerifyStartWarning(string skillName = null, int skillId = -1, int highestSkill = -1, int minSkill = -1, bool passed = false, string highestPawnName = null, Pawn highestPawn = null) {
            this.skillName = skillName;
            this.skillId = skillId;
            this.highestSkill = highestSkill;
            this.minSkill = minSkill;
            this.passed = passed;
            this.highestPawnName = highestPawnName;
            this.highestPawn = highestPawn;
        }
    }
}