# ModAPI
 A Moding API for My Summer Car. Allows a mod developer easily implement an attachable part that can be installed to the satsuma (or anything really) with little effort. 
 - Many settings for parts.
 - Many parts can be installed to many install points.
 - A part can be installed to many install points.
 - Many parts can be installed to one install point.
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
 - Added [Matrix Layer Collision GUI](https://github.com/tommojphillips/ModAPI/wiki/Matrix-Layer-Collision-UI) for viewing what layer collides with what. 
 
 ## Installation
 1. Download the latest version of ModAPI
 2. Unpack the folder
 3. Copy "Release/ModApi.dll" to "Mods/References" folder.

 [Modders] 
 Get XML comments on ModAPI members.
 - Copy "ModAPI.xml" to "Mods/References" folder.
 
 ## Examples

- See [Jerry Holder Mod](https://github.com/tommojphillips/JerryCanHolderMod) to have a look at how to implement the new ModAPI v0.2.* structure. its a lot easier and straight forward.

- See [Secure Spare Tire Mod](https://github.com/tommojphillips/SecureSpareTire) to see how ModAPI v0.2.* is implemented in this mod!

- See [Secure Car Jack Mod](https://github.com/tommojphillips/SecureCarJack) is a mod that uses ModAPI v0.2.* gives a good representation on how to change/add logic to the assemble/disasemmble logic. I wanted Secure Car Jack to not allow the player to install/secure the jack unless it is folded. so i had to add logic to display an interaction tip to let the player know.  

## Modding

- Mod API [Documentation](https://github.com/tommojphillips/ModAPI/wiki)

## Debugging with ModAPI

- See [Debugging with an API Referenced by your mod](https://github.com/piotrulos/MSCModLoader/wiki/Debugging-with-an-API-referenced-by-your-mod).
- Make sure to Copy The debug version of ModApi.dll to "Mods/Refernces".

#### [Obsolete] everything below is outdated infomation and won't exactly work from v0.1.4.1 and on-wards.

- To see this API in action, take a look at the demo i have created with a truck engine that attaches to the roof of satsuma, see "<https://github.com/tommojphillips/AttachObjectDemo>". - A walk-through of how to create your own truck engine install in version v0.1.1.4, see "<https://github.com/tommojphillips/ModAPI/wiki/Getting-started-in-v0.1.1.4-alpha>".  

- Cruise Control Panel for stock steering wheel created by Nitro and myself. see "https://github.com/tommojphillips/SatsumaCruiseControl"
