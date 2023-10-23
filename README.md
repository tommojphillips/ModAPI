## ModAPI
* A Moding API for My Summer Car. Allows a mod developer easily implement an attachable part that can be installed to the satsuma (or anything really) with little effort.
* [Mod API Wiki Home](https://github.com/tommojphillips/ModAPI/wiki)
  
  ### Features
 - Lots of settings for part, trigger, bolt.
 - Many parts can be installed to many install points.
 - Parts can be installed in different ways, rigid_del, rigid_kinematic or joint.
 - Easy access to common modding items, i.e common playmaker variables.
 - Many extention methods
 - Cached object references
 - Autosave feature that saves data (Part, Bolts) just like My Summer Car. Open "ModAPISaveData.txt" in MSCEditor!
 - Modders can save their own data to this save file as well.
 - Bolts have been implemented.
 - Bolts can be linked to either the part itself or a trigger.
 - Bolt tightness can effect the breakforce of the linked part.
 - New bolt type, "BoltWithNut". same as a bolt, but has a nut placed on the oposite side of the bolt.
 - Packages have been implemented.
 - New Package Type: CardboardBox. Part/s can be packed inside a cardboard box. Press the "Use" button to open the package! Apon opening, the cardboardbox turns into a folding box and plays a sound. The same as the packages found at the shop when purchased parts.
 - New Package Type: Crate. Part/s can be packed inside a crate. The player will need to pry each corner of a panel on the crate to pry it off. Once a panel has been pryed off, the player will be able to retrieve the packed parts.
 - INGAME DEV GUI for configuring parts / triggers / bolts.
 
 ### Installation
 1. Download the latest version of [ModAPI](../../releases/latest)
 2. Unpack the folder
 3. Copy "Release/ModApi.dll" to "Mods/References" folder.
