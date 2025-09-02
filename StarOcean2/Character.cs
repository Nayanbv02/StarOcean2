using System;
using System.Collections.ObjectModel;

namespace StarOcean2
{
	internal class Character
	{
		public ObservableCollection<BIT> Talents { get; private set; } = new ObservableCollection<BIT>();
		public ObservableCollection<Number> Skills { get; private set; } = new ObservableCollection<Number>();
        public ObservableCollection<Number> CombatSkills { get; } = new ObservableCollection<Number>();
        public ObservableCollection<Number> SpecialtySkills { get; } = new ObservableCollection<Number>();

        private readonly uint mAddress;

		public Character(uint address, String name)
		{
			mAddress = address;
			Name = name;
			 
			foreach (var talent in Info.Instance().Talent)
			{
				Talents.Add(new BIT(mAddress + 676 + talent.Value / 8, talent.Value % 8, talent.Name));
			}

            foreach (var (nameC, id) in Info.Instance().GetCombatSkillsOrdered())
            {
                uint off = mAddress + 420u + (uint)id * 4u;
                uint value = SaveData.Instance().ReadNumber(off, 4);
                CombatSkills.Add(new Number(off, 4, 0, 10) { Name = nameC, Value = value });
            }

            foreach (var (nameS, id) in Info.Instance().GetSpecialtySkillsOrdered())
            {
                uint off = mAddress + 160u + (uint)id * 4u;
                uint value = SaveData.Instance().ReadNumber(off, 4);
                SpecialtySkills.Add(new Number(off, 4, 0, 10) { Name = nameS, Value = value });
            }


        }

        public String Name { get; private set; }

		public uint Lv
		{
			get => SaveData.Instance().ReadNumber(mAddress, 1);
			set => Util.WriteNumber(mAddress, 1, value, 1, 255);
		}

		public uint Exp
		{
			get => SaveData.Instance().ReadNumber(mAddress + 4, 4);
			set => Util.WriteNumber(mAddress + 4, 4, value, 0, 99999999);
		}

        public uint SP
        {
            get => SaveData.Instance().ReadNumber(mAddress + 412, 4);
            set => Util.WriteNumber(mAddress + 412, 4, value, 0, 9999);
        }

        public uint BP
        {
            get => SaveData.Instance().ReadNumber(mAddress + 544, 4);
            set => Util.WriteNumber(mAddress + 544, 4, value, 0, 9999);
        }

        public uint BaseHP
		{
			get => SaveData.Instance().ReadNumber(mAddress + 16, 4);
			set => Util.WriteNumber(mAddress + 16, 4, value, 0, 9999);
		}

		public uint HP
		{
			get => SaveData.Instance().ReadNumber(mAddress + 20, 4);
			set => Util.WriteNumber(mAddress + 20, 4, value, 1, 9999);
		}

		public uint BaseMP
		{
			get => SaveData.Instance().ReadNumber(mAddress + 24, 4);
			set => Util.WriteNumber(mAddress + 24, 4, value, 0, 9999);
		}

		public uint MP
		{
			get => SaveData.Instance().ReadNumber(mAddress + 28, 4);
			set => Util.WriteNumber(mAddress + 28, 4, value, 0, 9999);
		}

		public uint ATK
		{
			get => SaveData.Instance().ReadNumber(mAddress + 32, 4);
			set => Util.WriteNumber(mAddress + 32, 4, value, 1, 9999);
		}

		public uint DEF
		{
			get => SaveData.Instance().ReadNumber(mAddress + 36, 4);
			set => Util.WriteNumber(mAddress + 36, 4, value, 1, 9999);
		}

        public uint INT
        {
            get => SaveData.Instance().ReadNumber(mAddress + 40, 4);
            set => Util.WriteNumber(mAddress + 40, 4, value, 1, 9999);
        }

        public uint HIT
        {
            get => SaveData.Instance().ReadNumber(mAddress + 44, 4);
            set => Util.WriteNumber(mAddress + 44, 4, value, 1, 9999);
        }

        public uint AVD
        {
            get => SaveData.Instance().ReadNumber(mAddress + 48, 4);
            set => Util.WriteNumber(mAddress + 48, 4, value, 1, 9999);
        }


        public uint GUTS
		{
			get => SaveData.Instance().ReadNumber(mAddress + 52, 4);
			set => Util.WriteNumber(mAddress + 52, 4, value, 1, 9999);
		}

		public uint CRT
		{
			get => SaveData.Instance().ReadNumber(mAddress + 56, 4);
			set => Util.WriteNumber(mAddress + 56, 4, value, 1, 9999);
		}

		public uint LUC
		{
			get => SaveData.Instance().ReadNumber(mAddress + 68, 4);
			set => Util.WriteNumber(mAddress + 68, 4, value, 1, 9999);
		}

		public uint STM
		{
			get => SaveData.Instance().ReadNumber(mAddress + 72, 4);
			set => Util.WriteNumber(mAddress + 72, 4, value, 1, 9999);
		}

	}
}
