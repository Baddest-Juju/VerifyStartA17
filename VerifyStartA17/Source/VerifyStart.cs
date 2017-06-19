using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using VerifyStartA17.Patches;
using Verse;

namespace VerifyStartA17 {

    public class VerifyStart : Mod {
        public bool bypass = false;
        private HarmonyInstance HarmonyInst = HarmonyInstance.Create("com.scuba156.RimWorld.VerifyStart");
        protected List<ColonistConfiguration> colonistConfig;
        protected List<ConfigurationLine> config;
        protected List<VerifyStartWarning> warnings = new List<VerifyStartWarning>();

        public VerifyStart(ModContentPack modContentPack) : base(modContentPack) {
            Instance = this;

            HarmonyInst.PatchAll(Assembly.GetExecutingAssembly());
        }

        public List<ColonistConfiguration> ColonistConfig {
            get {
                if (this.colonistConfig == null) {
                    this.colonistConfig = new List<ColonistConfiguration>
                    {
                        new ColonistConfiguration(),
                        new ColonistConfiguration(),
                        new ColonistConfiguration()
                    };
                }
                return this.colonistConfig;
            }
            set {
                this.colonistConfig = value;
            }
        }

        public List<ConfigurationLine> Config {
            get {
                if (this.config == null) {
                    this.config = new List<ConfigurationLine>();
                    foreach (SkillDef current in DefDatabase<SkillDef>.AllDefsListForReading) {
                        ConfigurationLine configurationLine = new ConfigurationLine() {
                            skillName = current.label,
                            skillId = (int)current.index
                        };
                        this.config.Add(configurationLine);
                    }
                }
                return this.config;
            }
            set {
                this.config = value;
            }
        }

        internal static HarmonyInstance Harmony { get { return Instance.HarmonyInst; } }

        internal static VerifyStart Instance { get; private set; }

        public bool CheckColonists() {
            List<SkillCheckRecord> list = new List<SkillCheckRecord>();
            int num = 0;
            checked {
                foreach (Pawn current in Find.GameInitData.startingPawns) {
                    using (List<SkillRecord>.Enumerator enumerator2 = current.skills.skills.GetEnumerator()) {
                        while (enumerator2.MoveNext()) {
                            SkillRecord skill = enumerator2.Current;
                            SkillCheckRecord skillCheckRecord;
                            try {
                                skillCheckRecord = list.Find((SkillCheckRecord x) => x.skillId.Equals((int)skill.def.index));
                            }
                            catch {
                                skillCheckRecord = null;
                            }
                            if (skillCheckRecord != null) {
                                if (skillCheckRecord.highestSkill < skill.Level && !skill.TotallyDisabled && (this.ColonistConfig[num].isUsing == enumIsUsing.Colony || this.ColonistConfig[num].isUsing == enumIsUsing.Locked)) {
                                    skillCheckRecord.highestSkill = skill.Level;
                                    skillCheckRecord.highestPawn = current;
                                }
                            }
                            else {
                                skillCheckRecord = new SkillCheckRecord() {
                                    skillName = skill.def.skillLabel,
                                    skillId = (int)skill.def.index
                                };
                                if (!skill.TotallyDisabled && (this.ColonistConfig[num].isUsing == enumIsUsing.Colony || this.ColonistConfig[num].isUsing == enumIsUsing.Locked)) {
                                    skillCheckRecord.highestSkill = skill.Level;
                                    skillCheckRecord.highestPawn = current;
                                }
                                else {
                                    skillCheckRecord.highestSkill = 0;
                                    skillCheckRecord.highestPawn = null;
                                }
                                list.Add(skillCheckRecord);
                            }
                        }
                    }
                    num++;
                }
                bool result = true;
                this.warnings.Clear();
                using (List<SkillCheckRecord>.Enumerator enumerator3 = list.GetEnumerator()) {
                    while (enumerator3.MoveNext()) {
                        SkillCheckRecord skill = enumerator3.Current;
                        ConfigurationLine configurationLine = this.config.Find((ConfigurationLine x) => x.skillId == skill.skillId);
                        bool useSkill = configurationLine.useSkill;
                        int minLevel = configurationLine.minLevel;
                        int highestSkill = skill.highestSkill;
                        string highestPawnName = null;
                        Pawn highestPawn = skill.highestPawn;
                        if (highestPawn != null) {
                            highestPawnName = highestPawn.NameStringShort;
                        }
                        if (useSkill && highestSkill < minLevel) {
                            this.warnings.Add(new VerifyStartWarning(skill.skillName, skill.skillId, highestSkill, minLevel, false, highestPawnName, highestPawn));
                            result = false;
                        }
                        else {
                            this.warnings.Add(new VerifyStartWarning(skill.skillName, skill.skillId, highestSkill, minLevel, true, highestPawnName, highestPawn));
                        }
                    }
                }
                if (this.bypass) {
                    result = true;
                }
                return result;
            }
        }

        public List<VerifyStartWarning> GetWarnings() {
            this.CheckColonists();
            return this.warnings;
        }

        public void LoadSettings() {
            string filepath = GenFilePaths.ModsConfigFilePath.Replace("ModsConfig.xml", "VerifyStart.xml");
            if (File.Exists(filepath)) {
                ConfigurationLine line = new ConfigurationLine();
                foreach (XElement element in XElement.Load(filepath).Elements()) {
                    XElement skill = element;
                    bool flag = false;
                    string localName = skill.Name.LocalName;
                    switch (localName) {
                        case "Shooting":
                        case "Melee":
                        case "Social":
                        case "Animals":
                        case "Medicine":
                        case "Cooking":
                        case "Construction":
                        case "Growing":
                        case "Mining":
                        case "Artistic":
                        case "Crafting":
                        case "Research":
                            line = Instance.Config.Find(x => x.skillName == skill.Name.LocalName);
                            if (line == null) {
                                flag = true;
                                line = new ConfigurationLine();
                            }
                            line.skillName = skill.Name.LocalName;
                            line.useSkill = Convert.ToBoolean(skill.Attribute("Use").Value);
                            line.minLevel = int.Parse(skill.Value);
                            if (flag) {
                                VerifyStart.Instance.Config.Add(line);
                                break;
                            }
                            break;

                        case "skill":
                            line = null;
                            if (skill.Attribute("id") == null) {
                                line = Instance.Config.Find(x => x.skillName == skill.Attribute("name").Value);
                            }
                            else {
                                line = Instance.Config.Find(x => x.skillId == int.Parse(skill.Attribute("id").Value));
                            }
                            if (line == null) {
                                flag = true;
                                line = new ConfigurationLine();
                            }
                            line.skillName = skill.Attribute("name").Value;
                            if (skill.Attribute("id") == null) {
                                SkillDef skillDef;
                                try {
                                    skillDef = DefDatabase<SkillDef>.AllDefsListForReading.Find(x => x.skillLabel == line.skillName);
                                }
                                catch (NullReferenceException ex) {
                                    Log.Error("Loading " + line.skillName + " from config file had no index.  Lookup of skill name in DefDatabase did not return an index either." + Environment.NewLine + ex.Message);
                                    skillDef = null;
                                }
                                line.skillId = skillDef == null ? -1 : skillDef.index;
                            }
                            else
                                line.skillId = int.Parse(skill.Attribute("id").Value);
                            line.useSkill = Convert.ToBoolean(skill.Attribute("use").Value);
                            line.minLevel = int.Parse(skill.Value);
                            if (flag) {
                                Instance.Config.Add(line);
                                break;
                            }
                            break;

                        default:
                            Log.Warning("Reading Verify Start settings found unknown setting name: " + localName);
                            break;
                    }
                }
            }
            else {
                foreach (ConfigurationLine configurationLine in this.Config) {
                    configurationLine.minLevel = 6;
                    configurationLine.useSkill = false;
                }
            }
        }

        public void SaveSettings() {
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings() {
                Indent = true,
                IndentChars = "\t"
            };
            string text = GenFilePaths.ModsConfigFilePath;
            text = text.Replace("ModsConfig.xml", "VerifyStart.xml");
            XmlWriter xmlWriter = XmlWriter.Create(text, xmlWriterSettings);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteComment("This file is generated by the Rimworld Mod Verify Start.");
            xmlWriter.WriteStartElement("VerifyStartSettings");
            foreach (ConfigurationLine current in this.Config) {
                xmlWriter.WriteStartElement("skill");
                xmlWriter.WriteAttributeString("name", current.skillName);
                xmlWriter.WriteAttributeString("id", current.skillId.ToString());
                xmlWriter.WriteAttributeString("use", current.useSkill.ToString());
                xmlWriter.WriteString(current.minLevel.ToString());
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            xmlWriter.Close();
        }

        public List<VerifyStartWarning> ShowWarnings() {
            return this.warnings;
        }
    }
}