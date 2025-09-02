# Star Ocean The Second Story R - Save Editor

![DL Count](https://img.shields.io/github/downloads/Nayanbv02/StarOcean2/total.svg)


A save editor for **Star Ocean The Second Story R** (Nintendo Switch).  
This tool allows you to easily modify characters and inventory values in your save files.

This repository is an **updated fork** of a project that had been discontinued since December, 2023 (latest release).  
The goal is to **keep it alive, add new features, and fix bugs**, always under the **GNU GPL v3.0 license**.

[Game website](https://www.square-enix-games.com/en_US/games/star-ocean-second-story-r)


---


## Features

- Edit **Money (FOL)**.  
- Modify character attributes:  
  - **Level (Lv)** and **Experience (EXP)**.  
  - **SP** (Skill Points) and **BP** (Battle Points).  
  - **Base/Current HP and MP**.  
  - Stats: ATK, DEF, HIT, AVD, INT, LUC, STM, CRT, GUTS.  
- Manage your **Party members**.  
- Edit **Talents** and **Skills**.  
- Modify **Inventory items** (add, clear, change amounts).  
- Simple interface with tabbed navigation.  


---


## Usage

- **Make a backup of your save file first.**
- Open the editor and select File → Open.
- Edit the values you want to change.
- Save your changes with File → Save.
- Copy the modified save back to your console.

    Saves format: 
        savedata00 <----- **Autosave**
        **Every save slot:**
        savedata01
        savedata02
        savedata03...


---


## Compatibility

Supported versions: Nintendo Switch game.
Tested with save files from EU, 2025. Game version 1.0.1 (v196608) + 5 DLC

If you encounter issues with a specific version, please open a GitHub issue.


---


## 📥 Installation

1. Clone the repository or download the ZIP:  
   ```bash
   git clone https://github.com/Nayanbv02/StarOcean2.git
   ```

2. [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) + [Visual Studio 2022](https://visualstudio.microsoft.com/vs/).

3. Build the project (Build → Build Solution).

4. Run the editor from Visual Studio or from the generated binary *including the "info" folder*.


---


## Credits
- Original project: [turtle-insect](https://github.com/turtle-insect/StarOcean2)
- Fork and current maintenance: [Nayanbv02](https://github.com/Nayanbv02/StarOcean2)
- Documentation: [SkillerCMP](https://github.com/SkillerCMP) - [Reference spreadsheet](https://docs.google.com/spreadsheets/d/1FBkLqn542IIYeFAg3fF1c6W6-pi6ympMFjsJGogquqI)
