using MSCLoader;
using System;
using System.Collections;
using TommoJProductions.ModApi.Attachable;
using UnityEngine;
using static TommoJProductions.ModApi.ModClient;

namespace TommoJProductions.ModApi
{

    /// <summary>
    /// Represents all console commands for modapi.
    /// </summary>
    public class ConsoleCommands : ConsoleCommand
    {
        // Written, 10.09.2021

        internal static Part _inspectingPart = null;
        internal static BoltCallback _inspectingBolt = null;

        #region Properties

        /// <summary>
        /// Represents the name of the base command.
        /// </summary>
        public override string Name => "modapi";
        /// <summary>
        /// Represents the base help message.
        /// </summary>
        public override string Help => "use \"modapi [?]/[help]\" to see command list!";
        /// <summary>
        /// Represents the general help list.
        /// </summary>
        const string HELP_LIST_GENERAL = "<color=yellow>modapi help</color> - shows this list\n" +
            "<color=yellow>modapi help [db]/[debug]</color> - shows debug help list\n " +
            "<color=yellow>modapi help [dv]/[developer]</color> - shows developer help list\n " +
            "<color=yellow>modapi [fv]/[fver]/[fileversion]</color> - reports modapi file version\n " +
            "<color=yellow>modapi [v]/[ver]/[version]</color> - reports modapi assembly version + Build number.\n";
        /// <summary>
        /// Represents the debug help list.
        /// </summary>
        const string HELP_LIST_DEBUG = "<color=yellow>modapi help [db]/[debug]</color> - shows this list\n" +
            "<color=yellow>modapi debug [tpp]/[teleportpart]</color> - use \"modapi help debug teleportpart\"\n" +
            "<color=yellow>modapi debug [lp]/[listparts]</color> - Lists all parts loaded in game - use \"modapi help debug listparts\"\n" +
            "<color=yellow>modapi debug [lb]/[listbolts]</color> - Lists all bolts loaded in game - use \"modapi help debug listbolts\"\n";
        /// <summary>
        /// Represents the dev help list.
        /// </summary>
        const string HELP_LIST_DEV = "<color=yellow>modapi help [dv]/[dev]/[developer]</color> - shows this list\n" +
            "<color=yellow>modapi dev [ep]/[editpart]</color> - use \"modapi help dev editpart\"\n" +
            "<color=yellow>modapi dev [eb]/[editbolt]</color> - use \"modapi help dev editbolt\"" +
            "<color=yellow>modapi dev [-t]/[toggle]</color> - toggles modapi dev mode. dev mode allows mod api users / dev to raycast for parts / bolts. shows a dev GUI. (CTRL+P)/(CTRL+B) while looking at ";
        /// <summary>
        /// Represents the teleport part help list.
        /// </summary>
        const string HELP_LIST_TELEPORT_PART = "<color=yellow>modapi help debug [tpp]/[tppart]/[teleportpart]</color> - shows this list\n" +
            "usage: <color=yellow>modapi debug teleportpart</color> [partName] - Teleports the part to the player.\n";
        /// <summary>
        /// Represents the teleport part help list.
        /// </summary>
        const string HELP_LIST_LIST_PARTS = "<color=yellow>modapi help debug [lp]/[listparts]</color> - shows this list\n" +
            "usage: <color=yellow>modapi debug listparts</color> - Lists all parts loaded in the game.\n";
        /// <summary>
        /// Represents the teleport part help list.
        /// </summary>
        const string HELP_LIST_LIST_BOLTS = "<color=yellow>modapi help debug [lb]/[listbolts]</color> - shows this list\n" +
            "usage: <color=yellow>modapi debug listbolts</color> - Lists all bolts loaded in the game.\n";
        /// <summary>
        /// Represents the edit part install transform help list.
        /// </summary>
        const string HELP_LIST_EDIT_PART = "<color=yellow>modapi help dev [ep]/[editpart]</color> - for development and design purposes..\n" +
            "Finds part to inspect. allows dev to move selected part and print part values." +
            "<color=yellow>modapi dev editpart -r</color> - Get part by raycast.\n" +
            "<color=yellow>modapi dev editpart -n [partName]</color> - Get part by name.\n" +
            "<color=yellow>modapi dev editpart -s</color> - Toggles inspection of selected part.\n" +
            "<color=yellow>modapi dev editpart start</color> - Starts inspecting selected part.\n" +
            "<color=yellow>modapi dev editpart stop</color> - Stops inspecting selected part.\n" +
            "<color=yellow>modapi dev editpart print</color> - Prints inspecting part transform info.\n";
        /// <summary>
        /// Represents the edit bolt transform help list.
        /// </summary>
        const string HELP_LIST_EDIT_BOLT = "<color=yellow>modapi help dev [eb]/[editbolt]</color> - for development and design purposes.. \n" +
            "Finds bolt to inspect. allows developer to move selected bolt and print bolt values." +
            "<color=yellow>modapi dev editbolt -r</color> - Get bolt by raycast.\n" +
            "<color=yellow>modapi dev editbolt -n [boltName]</color> - Get bolt by name.\n" +
            "<color=yellow>modapi dev editbolt -s</color> - Toggles inspection of selected bolt.\n" +
            "<color=yellow>modapi dev editbolt start</color> - Starts inspecting selected bolt.\n" +
            "<color=yellow>modapi dev editbolt stop</color> - Stops inspecting selected bolt.\n" +
            "<color=yellow>modapi dev editbolt print</color> - Prints inspecting bolt transform info.\n" +
            "<color=yellow></color> - \n";

        private bool _startedInspectingPart = false;
        private bool _startedInspectingBolt = false;

        #endregion

        /// <summary>
        /// invokes the <see cref="ConsoleCommand.Run(string[])"/> method.
        /// </summary>
        /// <param name="args"></param>
        public void invokeRun(params string[] args)
        {
            Run(args);
        }

        #region mscloader override methods

        /// <summary>
        /// Represents the main entry for the command.
        /// </summary>
        /// <param name="args">arguments passed to command</param>
        public override void Run(string[] args)
        {
            // Written, 10.09.2021

            if (args.Length == 0)
                print(Help);
            else if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "help":
                    case "?":
                    case "":
                        if (args.Length > 1)
                        {
                            switch (args[1])
                            {
                                case "debug":
                                case "db":
                                    if (args.Length > 2)
                                    {
                                        switch (args[2])
                                        {
                                            case "lp":
                                            case "listparts":
                                                print(HELP_LIST_LIST_PARTS);
                                                break;
                                            case "lb":
                                            case "listbolts":
                                                print(HELP_LIST_LIST_BOLTS);
                                                break;
                                            case "tpp":
                                            case "tppart":
                                            case "teleportpart":
                                                print(HELP_LIST_TELEPORT_PART);
                                                break;
                                        }
                                    }
                                    else
                                        print(HELP_LIST_DEBUG);
                                    break;
                                case "dv":
                                case "dev":
                                case "developer":
                                    if (args.Length > 2) 
                                    {
                                        switch (args[2])
                                        {
                                            case "eb":
                                            case "editbolt":
                                                print(HELP_LIST_EDIT_BOLT);
                                                break;
                                            case "ep":
                                            case "editpart":
                                                print(HELP_LIST_EDIT_PART);
                                                break;

                                        }
                                    }
                                    else
                                        print(HELP_LIST_DEV);
                                    break;
                                default:
                                    print("unknown command");
                                    print(HELP_LIST_GENERAL);
                                    break;
                            }
                        }
                        else
                        {
                            print(HELP_LIST_GENERAL);
                        }
                        break;
                    case "assemblyversion":
                    case "version":
                    case "ver":
                    case "v":
                        print("Assembly Version:{0}\nplatform:{1}\nconfiguration:{2}\nLatest Release: {3}", VERSION, ModApi.VersionInfo.IS_64_BIT ? "x64" : "x86", ModApi.VersionInfo.IS_DEBUG_CONFIG ? "debug" : "release", ModApi.VersionInfo.lastestRelease);
                        break;
                    case "fileversion":
                    case "fullversion":
                    case "fver":
                    case "fv":
                        print("File Version:{0}\nplatform:{1}\nconfiguration:{2}\nLatest Release: {3}", VersionInfo.fullVersion, ModApi.VersionInfo.IS_64_BIT ? "x64" : "x86", ModApi.VersionInfo.IS_DEBUG_CONFIG ? "debug" : "release", ModApi.VersionInfo.lastestRelease);
                        break;
                    case "debug":
                    case "db":
                        if (args.Length > 1)
                        {
                            switch (args[1])
                            {
                                case "lp":
                                case "lparts":
                                case "listparts":
                                    print("Count {0}", loadedParts.Count);
                                    foreach (var item in loadedParts)
                                        print("{0} | {1}", item.gameObject.name, item.partID);
                                    break;
                                case "lb":
                                case "lbolts":
                                case "listbolts":
                                    print("Count {0}", loadedBolts.Count);
                                    foreach (var item in loadedBolts)
                                        print("{0} | {1}", item.callback.gameObject.name, item.boltID);
                                    break;
                                case "tpp":
                                case "teleportpart":
                                    if (args.Length >= 2)
                                    {
                                        Part part = findPartByName(args[2]);
                                        if (part)
                                        {
                                            if (args.Length == 2)
                                            {
                                                if (part.installed)
                                                {
                                                    print("cannot teleport part while it's installed... use \"-f\" to force an uninstall and teleport.");
                                                    return;
                                                }
                                                part.teleport(false, ModClient.getPOV.transform.position);
                                            }
                                            else if (args.Length == 3 && args[3] == "-f")
                                            {
                                                part.teleport(true, ModClient.getPOV.transform.position);
                                            }
                                            else
                                                print("args were incorrect. error.");
                                        }
                                        else
                                            print("Use \"modapi debug listparts\" to see all vaild parameters.");
                                    }
                                    else
                                        print("command, \"modapi debug teleportpart\" expects atleast 1 argument, the parts name or ID. Use \"modapi debug listparts\" to see all vaild parameters.");
                                    break;
                                default:
                                    print("unknown command");
                                    print(HELP_LIST_DEBUG);
                                    break;
                            }
                        }
                        else
                            print("command, \"modapi debug\" expects atleast 1 argument, a sub command. Use \"modapi help debug\" to see all vaild parameters.");
                        break;
                    case "dv":
                    case "dev":
                    case "developer":
                        if (args.Length > 1)
                        {
                            switch (args[1])
                            {
                                case "ep":
                                case "editpart":
                                    if (args.Length > 2)
                                    {
                                        switch (args[2])
                                        {
                                            case "-r":
                                                if (!_startedInspectingPart)
                                                {
                                                    if (args.Length == 3)
                                                    {
                                                        print("Edit part install transform by raycast (-r)");
                                                        Part _part = raycastForBehaviour<Part>(true);
                                                        if (_part)
                                                        {
                                                            print("found {0} by raycast!", _part.name);
                                                            _inspectingPart = _part;
                                                        }
                                                        else
                                                            print("Could not find a part by raycast :( reason: raycast hit returned true but no part behaviour was found.");
                                                    }
                                                    else
                                                        print("command, \"modapi dev editpart -r\" expects 0 arguments.");
                                                }
                                                else
                                                    print("Stop inspecting part prior to looking for another part.");
                                                break;
                                            case "-n":
                                                if (!_startedInspectingPart)
                                                {
                                                    if (args.Length == 4)
                                                    {
                                                        print("Edit part install transform by name (-n)");
                                                        Part _part = findPartByName(args[4]);
                                                        if (_part)
                                                        {
                                                            print("found {0} by name!", _part.name);
                                                            _inspectingPart = _part;
                                                        }
                                                        else
                                                            print("Use \"modapi debug listparts\" to see all vaild parameters.");
                                                    }
                                                    else
                                                        print("command, \"modapi dev editpart -n\" expects 1 argument, the parts name. Use \"modapi debug listparts\" to see all vaild parameters.");
                                                }
                                                else
                                                    print("Stop inspecting part prior to looking for another part.");
                                                break;
                                            case "-s":
                                                if (!_startedInspectingPart)
                                                    invokeRun("developer", "editpart", "start");
                                                else
                                                    invokeRun("developer", "editpart", "stop" );
                                                break;
                                            case "start":
                                                if (!_inspectingPart)
                                                {
                                                    print("Find part prior to starting.");
                                                    return;
                                                }
                                                if (_startedInspectingPart)
                                                {
                                                    print("Already inspecting part.");
                                                    return;
                                                }
                                                if (_startedInspectingBolt)
                                                {
                                                    print("Cannot inspect a part when you are already inspecting a bolt.");
                                                    return;
                                                }
                                                _startedInspectingPart = true;
                                                devModeBehaviour.setInspectionPart(_inspectingPart);
                                                print("Started inspecting {0}..", _inspectingPart.name);
                                                break;
                                            case "stop":
                                                if (!_inspectingPart)
                                                {
                                                    print("Find part prior to stopping.");
                                                    return;
                                                }
                                                if (!_startedInspectingPart)
                                                {
                                                    print("You need to start inspecting a part prior to stopping..");
                                                    return;
                                                }
                                                _startedInspectingPart = false;
                                                devModeBehaviour.setInspectionPart(null);
                                                print("Stopped inspecting {0}..", _inspectingPart.name);
                                                break;
                                            case "print":
                                                if (!_inspectingPart)
                                                {
                                                    print("Find part prior to printing transform info.");
                                                    return;
                                                }
                                                print("{0} Transform info:\nposition: {1}\nrotation: {2}", _inspectingPart.name, _inspectingPart.transform.localPosition, _inspectingPart.transform.localEulerAngles);
                                                break;
                                            default:
                                                print("unknown command");
                                                print(HELP_LIST_EDIT_PART);
                                                break;
                                        }
                                    }
                                    else
                                        print("command, \"modapi dev editpart\" expects atleast 1 argument, a sub command. Use \"modapi help dev editpart\" to see all vaild parameters.");
                                    break;
                                case "eb":
                                case "editbolt":
                                    if (args.Length > 2)
                                    {
                                        switch (args[2])
                                        {
                                            case "-r":
                                                if (!_startedInspectingBolt)
                                                {
                                                    if (args.Length == 3)
                                                    {
                                                        print("Edit bolt transform by raycast (-r)");
                                                            BoltCallback _bolt = raycastForBehaviour<BoltCallback>(true);
                                                            if (_bolt)
                                                            {
                                                                print("found {0} by raycast!", _bolt.name);
                                                                _inspectingBolt = _bolt;
                                                            }
                                                            else
                                                                print("Could not find a bolt by raycast :( reason: raycast hit returned true but no bolt behaviour was found.");
                                                    }
                                                    else
                                                        print("command, \"modapi dev editbolt -r\" expects 0 arguments.");
                                                }
                                                else
                                                    print("Stop inspecting bolt prior to looking for another bolt.");
                                                break;
                                            case "-n":
                                                if (!_startedInspectingBolt)
                                                {
                                                    if (args.Length == 4)
                                                    {
                                                        print("Edit bolt transform by id (-n)");
                                                        BoltCallback _bolt = findBoltByName(args[4]);
                                                        if (_bolt)
                                                        {
                                                            print("found {0} by name!", _bolt.name);
                                                            _inspectingBolt = _bolt;
                                                        }
                                                        else
                                                            print("Use \"modapi debug listbolts\" to see all vaild parameters.");
                                                    }
                                                    else
                                                        print("command, \"modapi developer editbolt -n\" expects 1 argument, the bolts id. Use \"modapi debug listbolts {PartName}\" to see all vaild parameters.");
                                                }
                                                else
                                                    print("Stop inspecting bolt prior to looking for another bolt.");
                                                break;
                                            case "-s":
                                                if (!_startedInspectingBolt)
                                                    invokeRun("developer", "editbolt", "start");
                                                else
                                                    invokeRun("developer", "editbolt", "stop");
                                                break;
                                            case "start":
                                                if (!_inspectingBolt)
                                                {
                                                    print("Find bolt prior to starting.");
                                                    return;
                                                }
                                                if (_startedInspectingBolt)
                                                {
                                                    print("Already inspecting bolt.");
                                                    return;
                                                }
                                                if (_startedInspectingPart)
                                                {
                                                    print("Cannot inspect a bolt when you are already inspecting a part.");
                                                    return;
                                                }
                                                _startedInspectingBolt = true;
                                                DevMode.inspectionBolt = _inspectingBolt.bolt;
                                                print("Started inspecting {0}..", _inspectingBolt.name);
                                                break;
                                            case "stop":
                                                if (!_inspectingBolt)
                                                {
                                                    print("Find bolt prior to stopping.");
                                                    return;
                                                }
                                                if (!_startedInspectingBolt)
                                                {
                                                    print("You need to start inspecting a bolt prior to stopping..");
                                                    return;
                                                }
                                                _startedInspectingBolt = false;
                                                DevMode.inspectionBolt = null;
                                                print("Stopped inspecting {0}..", _inspectingBolt.name);
                                                break;
                                            case "print":
                                                if (!_inspectingBolt)
                                                {
                                                    print("Find bolt prior to printing transform info.");
                                                    return;
                                                }
                                                print("{0} Transform info:\nposition: {1}\nrotation: {2}", _inspectingBolt.name, _inspectingBolt.transform.localPosition, _inspectingBolt.transform.localEulerAngles);
                                                break;
                                            default:
                                                print("unknown command");
                                                print(HELP_LIST_EDIT_BOLT);
                                                break;
                                        }
                                    }
                                    else
                                        print("command, \"modapi debug editbolt\" expects atleast 1 argument, a sub command. Use \"modapi help dev editbolt\" to see all vaild parameters.");
                                    break;
                                case "-t":
                                case "toggle":
                                    if (devModeBehaviour)
                                    {
                                        GameObject.Destroy(devModeBehaviour);
                                    }
                                    else
                                    {
                                        ModApiLoader.addDevMode();
                                    }
                                    devMode = !devMode;
                                    print($"Developer mode: {(devMode ? "ON" : "OFF")}");
                                    break;
                                case "listgameparts":
                                case "listgp":
                                case "lgp":
                                    DevMode.listGameParts = !DevMode.listGameParts;
                                    DevMode.listModApiParts = false;
                                    break;
                                case "listmodapiparts":
                                case "listmodparts":
                                case "listmap":
                                case "lmp":
                                    DevMode.listModApiParts = !DevMode.listModApiParts;
                                    DevMode.listGameParts = false;
                                    break;
                                default:
                                    print("unknown command");
                                    print(HELP_LIST_DEV);
                                    break;
                            }
                        }
                        else
                            print("command, \"modapi developer\" expects atleast 1 argument, a sub command. Use \"modapi help developer\" to see all vaild parameters.");
                        break;
                    default:
                        print("unknown command");
                        print(HELP_LIST_GENERAL);
                        break;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary> 
        /// Finds the part by name or ID. first attempts to find an exact match, if no match was found then attempts to find the part by seeing if the part name contains the phrase, (<paramref name="inPartName"/>)
        /// </summary>
        /// <param name="inPartName">The parts name to find</param>
        private Part findPartByName(string inPartName)
        {
            // Written, 10.09.2021

            Part part = loadedParts.Find(_part => _part.name == inPartName || _part.partID == inPartName);
            if (!part)
                part = loadedParts.Find(_part => _part.name.Contains(inPartName));
            if (part)
                print("Found part: {0}", part.name);
            else
                print("Could not find the part by name: {0}", inPartName);
            return part;
        }
        /// <summary> 
        /// Finds the bolt by name or ID. first attempts to find an exact match, if no match was found then attempts to find the bolt by seeing if the bolt name contains the phrase, (<paramref name="inBoltName"/>)
        /// </summary>
        /// <param name="inBoltName">The bolts name to find</param>
        private BoltCallback findBoltByName(string inBoltName)
        {
            // Written, 10.09.2021

            BoltCallback bolt = loadedBolts.Find(_b => _b.callback.name == inBoltName || _b.boltID == inBoltName).callback;
            if (!bolt)
                bolt = loadedBolts.Find(_b => _b.callback.name.Contains(inBoltName)).callback;
            if (bolt)
                print("Found bolt: {0}", bolt.name);
            else
                print("Could not find the bolt by name: {0}", inBoltName);
            return bolt;
        }

        private void print(string format, params object[] args) 
        {
            ModConsole.Print(string.Format(format, args));
        }

        #endregion
    }
}
