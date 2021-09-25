using MSCLoader;

namespace TommoJProductions.ModApi.Interface
{
    /// <summary>
    /// Represents the modapi interface. Loads needed assets and other things.
    /// </summary>
    public class ModApiInterface : Mod
    {
        // Written, 09.10.2021

        #region Constraints

#if DEBUG
        /// <summary>
        /// Represents complie-time debug configuration is true.
        /// </summary>
        public const bool IS_DEBUG_CONFIG = true;
#else
        /// <summary>
        /// Represents complie-time debug configuration is false.
        /// </summary>
        public const bool IS_DEBUG_CONFIG = false;
#endif

#if x64
        /// <summary>
        /// Represents complie-time x64 configuration is true.
        /// </summary>
        public const bool IS_X64 = true;
#else
        /// <summary>
        /// Represents complie-time x64 configuration is false.
        /// </summary>
        public const bool IS_x64 = false;
#endif

        #endregion

        #region MSCLOADER OVERRIDE PROPERTIES

        /// <summary>
        /// Reps mod ID
        /// </summary>
        public override string ID => "ModAPI";
        /// <summary>
        /// Reps mod name
        /// </summary>
        public override string Name => "Mod API";
        /// <summary>
        /// Reps modapi version
        /// </summary>
        public override string Version => "v0.1.1";
        /// <summary>
        /// Reps mod author.
        /// </summary>
        public override string Author => "tommojphillips";
        /// <summary>
        /// loads in menu
        /// </summary>
        public override bool LoadInMenu => true;
        /// <summary>
        /// second pass on
        /// </summary>
        /// 
        public override bool SecondPass => true;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initialies a new instance of <see cref="ModApiInterface"/>.
        /// </summary>
        public ModApiInterface()
        {
            // Written, 09.10.2021

            preLoad();
        }

        #endregion
        
        #region MSCLOADER OVERRIDE METHODS

        /// <summary>
        /// Reps onMenuLoad.
        /// </summary>
        public override void OnMenuLoad()
        {
            //load();
        }
        /// <summary>
        /// Reps onLoad.
        /// </summary>
        public override void OnLoad()
        {
            load();
        }
        /// <summary>
        /// Reps onSecondPassLoad().
        /// </summary>
        public override void SecondPassOnLoad()
        {
            postLoad();
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Reps pre-load function
        /// </summary>
        public void preLoad()
        {
            // Written, 09.10.2021

            ConsoleCommand.Add(new ConsoleCommands());
        }
        /// <summary>
        /// Reps load function
        /// </summary>
        public void load()
        {
            // Written, 09.10.2021

            ModClient.parts.Clear();
        }
        /// <summary>
        /// Reps post-load function
        /// </summary>
        public void postLoad()
        {
            // Written, 09.10.2021

        }
        /// <summary>
        /// Reps modapi print-to-console function
        /// </summary>
        public static void print(string format, params object[] args) 
        {
            // Written, 09.10.2021

            ModConsole.Log(string.Format("<color=grey>[ModAPI] - "+ format +"</color>", args));
        }

        #endregion
    }
}
