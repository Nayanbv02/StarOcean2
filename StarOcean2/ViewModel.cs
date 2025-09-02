using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace StarOcean2
{
	internal class ViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		public Info Info { get; private set; } = Info.Instance();
		public Basic Basic { get; private set; } = new Basic();
		public ObservableCollection<Character> Characters { get; private set; } = new ObservableCollection<Character>();
		public ObservableCollection<Number> Party { get; private set; } = new ObservableCollection<Number>();
		public ObservableCollection<Item> Items { get; private set; } = new ObservableCollection<Item>();

		public CommandAction OpenFileCommand { get; private set; }
		public CommandAction SaveFileCommand { get; private set; }
		public CommandAction AllItemCountCommand { get; private set; }
		public CommandAction AllItemClearCommand { get; private set; }

		public uint ItemCount { get; set; } = 10;

		public ViewModel()
		{
			OpenFileCommand = new CommandAction(OpenFile);
			SaveFileCommand = new CommandAction(SaveFile);
			AllItemCountCommand = new CommandAction(AllItemCount);
			AllItemClearCommand = new CommandAction(AllItemClear);
		}

		private void Load()
		{
			Characters.Clear();
			for (uint i = 0; i < 13; i++)
			{
				var ch = new Character(0x4FC + i * 1164, Info.Instance().Character[(int)i + 1].Name);
				Characters.Add(ch);
			}


			Party.Clear();
			for (uint i = 0; i < 8; i++)
			{
				Party.Add(new Number(0x57CD4 + i * 4, 4, 0, 13));
			}


			Items.Clear();
			for (uint i = 0; i < 0x37E0; i++)
			{
				var item = new Item(0x3FD4 + i * 24);
				if (item.ID == 0) continue;
				Items.Add(item);
			}
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Basic)));
		}

		private void OpenFile(Object? obj)
		{
			var dlg = new OpenFileDialog();
			dlg.Filter = "savedata*|savedata*";
			if (dlg.ShowDialog() == false) return;

			SaveData.Instance().Open(dlg.FileName);
			Load();
		}

		private void SaveFile(Object? obj)
		{
			SaveData.Instance().Save();
		}

        private void AllItemCount(Object? obj)
        {
            uint slot = 0;
            foreach (var item in Info.Item)
            {
                if (item.Name.Contains("???"))
                    continue;

                uint id = item.Value;
                uint address = 0x3FD4 + slot * 24;

                SaveData.Instance().WriteNumber(address + 2, 2, id);
                SaveData.Instance().WriteNumber(address + 4, 1, ItemCount);
                SaveData.Instance().WriteNumber(address + 16, 1, 1);
                SaveData.Instance().WriteNumber(address + 17, 1, 1);

                slot++;
            }
            Load();
        }

        private void AllItemClear(Object? obj)
		{
			for (uint i = 0; i < 0x37E0; i++)
			{
				uint address = 0x3FD4 + i * 24;
				SaveData.Instance().WriteNumber(address + 2, 2, 0);
				SaveData.Instance().WriteNumber(address + 4, 1, 0);
				SaveData.Instance().WriteNumber(address + 16, 1, 0);
				SaveData.Instance().WriteNumber(address + 17, 1, 0);
			}
			Load();
		}

        public void ApplyItemChoice(uint id)
        {
            if (id == 0) return;

            uint address = 0x3FD4 + id * 24;
            SaveData.Instance().WriteNumber(address + 2, 2, id);
            SaveData.Instance().WriteNumber(address + 4, 1, 1);
            SaveData.Instance().WriteNumber(address + 16, 1, 1);
            SaveData.Instance().WriteNumber(address + 17, 1, 1);

            Load();
        }

    }
}
