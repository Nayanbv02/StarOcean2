using System;
using System.Collections.Generic;
using System.Linq;

namespace StarOcean2
{
    class Info
    {
        private static Info? mThis;
        public List<NameValueInfo> Character { get; private set; } = new List<NameValueInfo>();
        public List<NameValueInfo> Item { get; private set; } = new List<NameValueInfo>();
        public List<NameValueInfo> Talent { get; private set; } = new List<NameValueInfo>();
        public List<NameValueInfo> Skill { get; private set; } = new List<NameValueInfo>();
        public List<NameValueInfo> SpecialtySkills { get; private set; } = new List<NameValueInfo>();
        public Dictionary<int, string> SkillById { get; private set; } = new Dictionary<int, string>();


        private static readonly int[] CombatOrder = {
            0,  // Guardbreak
            4,  // Feint
            5,  // Power Burst
            8,  // Qigong
            7,  // Body Control
            9,  // Sidestep
            11, // Godspeed
            6,  // Hasten Speech
            14, // Trance
            15, // Concentration
            17, // Interrupt
            2,  // Backstab
            23, // Breaker
            24  // Accumulation Aid
        };

        public Dictionary<int, string> SpecialtySkillsById { get; private set; } = new Dictionary<int, string>();

        private static readonly int[] SpecialtyOrder = {
            10,   // Determination
            13,   // Biology
            14,   // Mental Science
            26,   // Aesthetic Design
            15,   // Knife
            21,   // Technology
            4,   // Mineralogy
            22,   // Faeriology
            11,   // Resilience
            8,   // Penmanship
            20,  // Smithing
            6,  // Eye for Detail
            1,  // Music Knowledge
            2,  // Performance
            12,  // Danger Radar
            28,  // Poker Face
            5,  // Herbology
            24,  // Piety
            23,  // ESP
            25,  // Purity
            3,  // Item Knowledge
            17,  // Keen Eye
            9,  // Effort
            16,  // Recipe
            0,  // Sketching
            7,  // Aesthetics
            18,  // Whistling
            19,  // Animal Training
            27,  // Courage
            29,  // Imitation
            30,  // Machinery
            31   // Operation
        };




        private static readonly HashSet<int> CombatSet = new HashSet<int>(CombatOrder);

        private Info() { }

        public static Info Instance()
        {
            if (mThis == null)
            {
                mThis = new Info();
                mThis.Init();
            }
            return mThis;
        }

        public NameValueInfo? Search<Type>(List<Type> list, uint id)
            where Type : NameValueInfo, new()
        {
            int min = 0;
            int max = list.Count;
            for (; min < max;)
            {
                int mid = (min + max) / 2;
                if (list[mid].Value == id) return list[mid];
                else if (list[mid].Value > id) max = mid;
                else min = mid + 1;
            }
            return null;
        }

        private void Init()
        {
            AppendList("info\\char.txt", Character);
            AppendList("info\\item.txt", Item);
            AppendList("info\\talent.txt", Talent);
            AppendList("info\\combat_skills.txt", Skill);
            AppendList("info\\specialty_skills.txt", SpecialtySkills);

            SkillById = Skill
                .GroupBy(s => (int)s.Value)
                .ToDictionary(g => (int)g.Key, g => g.First().Name);

            SpecialtySkillsById = SpecialtySkills
                .GroupBy(s => (int)s.Value)
                .ToDictionary(g => (int)g.Key, g => g.First().Name);
        }


        private void AppendList<Type>(String filename, List<Type> items) where Type : NameValueInfo, new()
        {
            if (!System.IO.File.Exists(filename)) return;

            String[] lines = System.IO.File.ReadAllLines(filename);

            foreach (String line in lines)
            {
                if (line.Length < 3) continue;
                if (line[0] == '#') continue;
                String[] values = line.Split('\t');
                if (values.Length < 2) continue;
                if (String.IsNullOrEmpty(values[0])) continue;
                if (String.IsNullOrEmpty(values[1])) continue;

                Type type = new Type();
                if (type.Line(values))
                {
                    items.Add(type);
                }
            }

            items.Sort();
        }

        public bool IsCombatSkill(int id) => CombatSet.Contains(id);

        public IEnumerable<(string Name, int Id)> GetCombatSkillsOrdered()
        {
            return CombatOrder.Where(id => SkillById.ContainsKey(id))
                              .Select(id => (SkillById[id], id))
                              .OrderBy(skill => Array.IndexOf(CombatOrder, skill.Item2));
        }

        public IEnumerable<(string Name, int Id)> GetSpecialtySkillsOrdered()
            => SpecialtyOrder.Where(id => SpecialtySkillsById.ContainsKey(id))
                             .Select(id => (SpecialtySkillsById[id], id));

    }
}
