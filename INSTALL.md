# Install Realm

## Required Software

### Unity (6000.0.25)

1. Download [Unity](https://unity.com/releases/editor/whats-new/6000.0.25#installs) for your operating system
2. Select Andorid and/or iOS Build Support depending on which platform you are using
3. Complete the remaining installation steps
4. [Setup Unity](https://code.visualstudio.com/docs/other/unity) to work with Visual Studio Code

### Visual Studio Code (1.98.2)

1. Download [Visual Studio Code](https://code.visualstudio.com/updates/v1_98)
2. Complete the installation steps

## Build & Install

1. Clone repo
```bash
git clone https://github.com/russellrd/realm.git
```
2. Open _realm/src/RealmClient_ in Unity
3. Set API keys in Edit->Project Settings
   1. ARCore Extentions
   2. ArcGIS Maps SDK 
4. Select desired build platform (File->Build Profiles)
5. Connect mobile device to computer and enable development mode
6. Select "Build And Run"

## Run Tests

1. Open _realm/src/RealmClient_ in Unity
2. Open Window->General->Test Runner
3. Select the _PlayMode_ tab
4. Dobule click on test to be run in tree