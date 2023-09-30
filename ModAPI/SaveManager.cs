using System;
using System.Collections.Generic;
using System.Linq;
using TanjentOGG;

using TommoJProductions.ModApi.Attachable;

using UnityEngine;

namespace TommoJProductions.ModApi
{
    /// <summary>
    /// Represents save and load logic.
    /// </summary>
    public class SaveManager
    {
        // Written, 26.08.2023

        /// <summary>
        /// The position Identifier (Tag)
        /// </summary>
        public static readonly string positionTag = "position";
        /// <summary>
        /// The rotation Identiifer (Tag)
        /// </summary>
        public static readonly string rotationTag = "rotation";
        /// <summary>
        /// The installed Identiifer (Tag)
        /// </summary>
        public static readonly string installedTag = "installed";
        /// <summary>
        /// The install point id Identiifer (Tag)
        /// </summary>
        public static readonly string installPointIDTag = "installPointID";
        /// <summary>
        /// The bolts Identiifer (Tag)
        /// </summary>
        public static readonly string boltTag = "bolts";
        /// <summary>
        /// The none (null) Identiifer (Tag)
        /// </summary>
        public static readonly string noneTag = "None";

        private string _saveFile;        
        private string _identifier;

        /// <summary>
        /// Represents the save file name.
        /// </summary>
        public string saveFile => _saveFile;
        /// <summary>
        /// Represents the pre identifier.
        /// </summary>
        public string identifier => _identifier;


        /// <summary>
        /// initializes new instance and assigns <see cref="_saveFile"/> to <paramref name="saveFile"/>
        /// </summary>
        /// <param name="saveFile"></param>
        public SaveManager(string saveFile)
        {
        }
        /// <summary>
        /// initializes new instance and asigns <see cref="_saveFile"/> to ModApiParts.txt
        /// </summary>
        public SaveManager()
        {
            _saveFile = "ModApiParts.txt";
            _identifier = _saveFile + "?tag=";
        }

        /// <summary>
        /// Loads a value using the <paramref name="part"/>s PartID combined with the <paramref name="id"/>. Creates an ES2Reader
        /// </summary>
        /// <typeparam name="T">The save type to load.</typeparam>
        /// <param name="part">The part to use its PartID.</param>
        /// <param name="id">The id of the value to load</param>
        /// <returns>If value id is found returns the loaded value otherwise returns null.</returns>
        public T loadValue<T>(Part part, string id) => loadValue<T>(part.partID + id);
        /// <summary>
        /// Loads a value using the <paramref name="part"/>s PartID combined with the <paramref name="id"/>. Using an already created reader.
        /// </summary>
        /// <typeparam name="T">The save type to load.</typeparam>
        /// <param name="part">the part to load a value from.</param>
        /// <param name="id">The id of the </param>
        /// <param name="reader">The reader to use.</param>
        /// <returns>If value id is found returns the loaded value otherwise returns null.</returns>
        public T loadValue<T>(Part part, string id, ES2Reader reader) => loadValue<T>(part.partID + id, reader);
        /// <summary>
        /// Saves a value using the <paramref name="part"/>s PartID combined with the <paramref name="id"/>. creates a writer.
        /// </summary>
        /// <typeparam name="T">The save type to Save.</typeparam>
        /// <param name="part">The part to use its PartID.</param>
        /// <param name="id">The id of the value to save</param>
        /// <param name="value">The value to save.</param>
        public void saveValue<T>(Part part, string id, T value) => saveValue(part.partID + id, value);
        /// <summary>
        /// Saves a value using the <paramref name="part"/>s PartID combined with the <paramref name="id"/>. Using an already created writer.
        /// </summary>
        /// <typeparam name="T">The save type to Save.</typeparam>
        /// <param name="part">The part to use its PartID.</param>
        /// <param name="id">The id of the value to save</param>
        /// <param name="writer">The writer to use.</param>
        /// <param name="value">The value to save.</param>
        public void saveValue<T>(Part part, string id, T value, ES2Writer writer) => saveValue(part.partID + id, value, writer);
        /// <summary>
        /// Saves a value. using an already created ES2Writer
        /// </summary>
        /// <param name="id">The Identifier to save as</param>
        /// <param name="value">the value to save.</param>
        /// <param name="writer">The ES2 Writer to use.</param>
        public void saveValue<T>(string id, T value, ES2Writer writer)
        {
            // Written, 26.08.2023

            writer.Write<T>(value, id);
        }
        /// <summary>
        /// Saves a value. Creates an ES2Writer
        /// </summary>
        /// <param name="id">The Identifier to save as</param>
        /// <param name="value">the value to save.</param>
        public void saveValue<T>(string id, T value)
        {
            // Written, 26.08.2023

            using (ES2Writer writer = ES2Writer.Create(_saveFile))
            {
                writer.Write<T>(value, id);
                writer.Save();
            }
        }
        /// <summary>
        /// loads a value. Creates an ES2Reader
        /// </summary>
        /// <param name="id">The Identifier to save as</param>
        public T loadValue<T>(string id)
        {
            // Written, 26.08.2023

            using (ES2Reader reader = ES2Reader.Create(_saveFile))
            {
                return readOrDefault<T>(id, reader);
            }
        }
        /// <summary>
        /// loads a value. using an already created ES2Reader
        /// </summary>
        /// <param name="id">The Identifier to save as</param>
        /// <param name="reader">The ES2 Reader to use.</param>
        public T loadValue<T>(string id, ES2Reader reader)
        {
            // Written, 26.08.2023

            return readOrDefault<T>(id, reader);
        }
        /// <summary>
        /// loads a value. using <see cref="ES2Reader"/>
        /// </summary>
        /// <param name="id">The Identifier to save as</param>
        /// <param name="reader">The ES2 Reader to use.</param>
        /// <param name="value">The loaded value.</param>
        /// <returns><see langword="true"/> if <paramref name="id"/> exists.</returns>
        public bool loadValue<T>(string id, out T value, ES2Reader reader)
        {
            // Written, 26.08.2023

            if (ES2.Exists(_identifier + id))
            {
                value = reader.Read<T>(id);
                return true;
            }
            value = default;
            return false;
        }


        /// <summary>
        /// Loads Part save info. if does not exist. returns <see langword="null"/>.
        /// </summary>
        /// <param name="id">The identifier of the part save info to get.</param>
        /// <returns>found part save info or <see langword="null"/>.</returns>
        public PartSaveInfo loadPart(string id)
        {
            // Written, 03.09.2023

            if (ES2.Exists(_identifier + id + installedTag))
            {
                using (ES2Reader reader = ES2Reader.Create(_saveFile))
                {
                    PartSaveInfo info = new PartSaveInfo();
                    info.position = readOrDefault<Vector3>(id + positionTag, reader);
                    info.rotation = readOrDefault<Vector3>(id + rotationTag, reader);
                    info.installed = readOrDefault<bool>(id + installedTag, reader);
                    info.installPointId = readOrDefault<string>(id + installPointIDTag, reader);

                    if (info.installPointId == noneTag)
                    {
                        info.installPointId = null;
                    }

                    if (info.installed)
                    {
                        info.boltTightness = loadBolts(id, reader);
                    }

                    return info;
                }
            }
            return null;
        }
        /// <summary>
        /// Checks if an ID exists. then reads the file + identifier. if ID doesnt exist, returns <see langword="default"/>(<typeparamref name="T"/>)
        /// </summary>
        /// <typeparam name="T">The type to read.</typeparam>
        /// <param name="id">The entire identifier (tag) without filename.</param>
        /// <param name="reader">The ES2 reader being used.</param>
        public T readOrDefault<T>(string id, ES2Reader reader)
        {
            // Written, 03.09.2023

            if (ES2.Exists(_identifier + id))
                return reader.Read<T>(id);
            return default;
        }
        /// <summary>
        /// Saves a <see cref="Part"/> using <see cref="ES2Writer"/>. Writes all data from <see cref="Part._loadedSaveInfo"/>
        /// </summary>
        /// <param name="part">The <see cref="Part"/> to save.</param>
        /// <param name="writer">The writer to save the <paramref name="part"/>.</param>
        internal void savePart(Part part, ES2Writer writer)
        {
            // Written, 03.09.2023

            PartSaveInfo info = part.getSaveInfo();
            string id = part.partID;

            if (part.partSettings.setPositionRotationOnInitialisePart)
            {
                writer.Write(info.position, id + positionTag);
                writer.Write(info.rotation, id + rotationTag);
            }
            else
            {
                if (ES2.Exists(_identifier + id + positionTag))
                    writer.Delete(id + positionTag);
                if (ES2.Exists(_identifier + id + rotationTag))
                    writer.Delete(id + rotationTag);
            }
            writer.Write(info.installed, id + installedTag);
            writer.Write(info.installed ? info.installPointId : noneTag, id + installPointIDTag);

            if (info.installed)
            {
                if (part.hasBolts)
                {
                    saveBolts(id, info.boltTightness, writer);
                }
            }
        }

        /// <summary>
        /// Saves a <see cref="Trigger"/> using <see cref="ES2Writer"/>. Writes all data from <see cref="Trigger.bolts"/>
        /// </summary>
        /// <param name="trigger">The <see cref="Trigger"/> to save.</param>
        /// <param name="writer">The writer to save the <paramref name="trigger"/>.</param>
        internal void saveTrigger(Trigger trigger, ES2Writer writer)
        {
            // Written, 11.09.2023

            if (trigger.hasBolts)
            {
                TriggerSaveInfo info = trigger.getSaveInfo();

                saveBolts(trigger.triggerID, info.boltTightness, writer);
            }
        }
        /// <summary>
        /// Loads bolts from es2 using the Tag <paramref name="id"/>. Creates an ES2Reader. 
        /// </summary>
        /// <param name="id">The id of the bolts to load.</param>
        /// <returns>If found returns loaded bolts, Otherwise returns <see langword="null"/>.</returns>
        internal int[] loadBolts(string id)
        {
            using (ES2Reader reader = ES2Reader.Create(_saveFile))
            {
                return loadBolts(id, reader);
            }
        }
        /// <summary>
        /// Loads bolts from es2 using the Tag <paramref name="id"/>. Uses <paramref name="reader"/> to read data. 
        /// </summary>
        /// <param name="id">The id of the bolts to load.</param>
        /// <param name="reader">The ES2Reader to use.</param>
        internal int[] loadBolts(string id, ES2Reader reader) 
        {
            // Written, 11.09.2023

            if (ES2.Exists(_identifier + id + boltTag))
            {
                List<string> bolts = reader.ReadList<string>(id + boltTag);
                int[] tightness = new int[bolts.Count];
                int current;
                for (int i = 0; i < bolts.Count; i++)
                {
                    if (Int32.TryParse(bolts[i], out current))
                    {
                        tightness[i] = current;
                        continue;
                    }
                    tightness[i] = 0;
                }
                return tightness;
            }
            return null;
        }

        private void init(string saveFile)
        {
            _saveFile = saveFile;
            _identifier = saveFile + "?tag=";
        }

        internal void saveBolts(string id, int[] boltTightness)
        {
            using (ES2Writer writer = ES2Writer.Create(_saveFile))
            {
                saveBolts(id, boltTightness, writer);
            }
        }
        internal void saveBolts(string id, int[] boltTightness, ES2Writer writer)
        {
            // Written, 11.09.2023

            List<string> bolts = new List<string>();
            for (int i = 0; i < boltTightness.Length; i++)
            {
                bolts.Add(boltTightness[i].ToString());
            }
            writer.Write(bolts, id + boltTag);
        }
    }
}
