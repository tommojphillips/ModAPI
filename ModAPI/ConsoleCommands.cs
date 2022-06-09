using MSCLoader;
using System.Collections;
using TommoJProductions.ModApi.Attachable;
using UnityEngine;

namespace TommoJProductions.ModApi
{

    /// <summary>
    /// Represents all console commands for modapi.
    /// </summary>
    public class ConsoleCommands : ConsoleCommand
    {
        // Written, 10.09.2021

        #region Properties

        /// <summary>
        /// Represents the name of the base command.
        /// </summary>
        public override string Name => "modapi";
        /// <summary>
        /// Represents the base help message.
        /// </summary>
        public override string Help => "use \"modapi help\" to see command list!";
        /// <summary>
        /// Represents the general help list.
        /// </summary>
        const string helpListGeneral = "<color=yellow>modapi help</color> - shows this list\n" +
            "<color=yellow>modapi help debug</color> - shows debug help list\n " +
            "<color=yellow>modapi version</color> - reports modapi version\n";
        /// <summary>
        /// Represents the debug help list.
        /// </summary>
        const string helpListDebug = "<color=yellow>modapi help debug</color> - shows this list\n" +
            "<color=yellow>modapi debug editpartinstalltransform</color> - use \"modapi help debug editpartinstalltransform\"\n" +
            "<color=yellow>modapi debug teleportpart</color> - use \"modapi help debug teleportpart\"\n" +
            "<color=yellow>modapi debug listparts</color> - Lists all parts loaded in game\n" +
            "<color=yellow></color> - \n" +
            "<color=yellow></color> - \n";
        /// <summary>
        /// Represents the teleport part help list.
        /// </summary>
        const string helpListTeleportPart = "<color=yellow>modapi help debug teleportpart</color> - shows this list\n" +
            "usage: <color=yellow>modapi debug teleportpart</color> [partName] - Teleports the part to the player.\n" +
            "<color=yellow></color> - \n" +
            "<color=yellow></color> - \n";
        /// <summary>
        /// Represents the edit install transform help list.
        /// </summary>
        const string helpListEditInstallTransformPart = "<color=yellow>modapi help debug editpartinstalltransform</color> - for development purposes.. Creates new part and attaches said part to its' install point, allows you then move/rotate part to get correct install position and rotation.\n" +
            "<color=yellow>modapi debug editpartinstalltransform -r</color> - Get part by raycast.\n" +
            "<color=yellow>modapi debug editpartinstalltransform -n [partName]</color> - Get part by name.\n" +
            "<color=yellow>modapi debug editpartinstalltransform start</color> - Starts inspecting selected part.\n" +
            "<color=yellow>modapi debug editpartinstalltransform stop</color> - Stops inspecting selected part.\n" +
            "<color=yellow>modapi debug editpartinstalltransform print</color> - Prints inspecting part transform info.\n" +
            "<color=yellow></color> - \n";

        private Part _inspectingPart = null;
        private bool _startedInspectingPart = false;
        #endregion

        #region mscloader override methods

        /// <summary>
        /// Represents the main entry for the command.
        /// </summary>
        /// <param name="args">arguments passed to command</param>
        public override void Run(string[] args)
        {
            // Written, 10.09.2021

            if (args.Length == 0)
                ModClient.print(Help);
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
                                            case "tpp":
                                            case "tppart":
                                            case "teleportpart":
                                                ModClient.print(helpListTeleportPart);
                                                break;
                                            case "epit":
                                            case "editpart":
                                            case "editpartinstalltransform":
                                                ModClient.print(helpListEditInstallTransformPart);
                                                break;
                                        }
                                    }
                                    else
                                        ModClient.print(helpListDebug);
                                    break;
                                default:
                                    ModClient.print("unknown command");
                                    ModClient.print(helpListGeneral);
                                    break;
                            }
                        }
                        else
                        {
                            ModClient.print(helpListGeneral);
                        }
                        break;
                    case "version":
                    case "ver":
                    case "v":
                        ModClient.print("version:{0}\nplatform:{1}\nconfiguration:{2}", ModClient.version, ModClient.IS_X64 ? "x64" : "x86", ModClient.IS_DEBUG_CONFIG ? "debug" : "release");
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
                                    if (ModClient.parts.Count > 0)
                                        foreach (var item in ModClient.parts)
                                            ModClient.print("{0}", item.gameObject.name);
                                    else
                                        ModClient.print("No parts found :(");
                                    break;
                                case "epit":
                                case "editpart":
                                case "editpartinstalltransform":
                                    if (args.Length > 2)
                                    {
                                        switch (args[2])
                                        {
                                            case "-r":
                                                if (!_startedInspectingPart)
                                                {
                                                    if (args.Length == 3)
                                                    {
                                                        ModClient.print("Edit part install transform by raycast (-r)");
                                                        RaycastHit hitInfo;
                                                        bool hasHit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                                                        if (hasHit)
                                                        {
                                                            Part _part = hitInfo.collider.GetComponent<Part>();
                                                            if (_part)
                                                            {
                                                                ModClient.print("found {0} by raycast!", _part.name);
                                                                _inspectingPart = _part;
                                                            }
                                                            else
                                                                ModClient.print("Could not find a part by raycast :( reason: raycast hit returned true but no part behaviour was found.");
                                                        }
                                                        else
                                                            ModClient.print("Could not find a part by raycast :( reason: raycast hit returned false.");
                                                    }
                                                    else
                                                        ModClient.print("command, \"modapi debug editpartinstalltransform -r\" expects 0 arguments.");
                                                }
                                                else
                                                    ModClient.print("Stop inspecting part prior to looking for another part.");
                                                break;
                                            case "-n":
                                                if (!_startedInspectingPart)
                                                {
                                                    if (args.Length == 4)
                                                    {
                                                        ModClient.print("Edit part install transform by name (-n)");
                                                        Part _part = findPartByName(args[3]);
                                                        if (_part)
                                                        {
                                                            ModClient.print("found {0} by name!", _part.name);
                                                            _inspectingPart = _part;
                                                        }
                                                        else
                                                            ModClient.print("Use \"modapi listparts\" to see all vaild parameters.");
                                                    }
                                                    else
                                                        ModClient.print("command, \"modapi debug editpartinstalltransform -n\" expects 1 argument, the parts name. Use \"modapi listparts\" to see all vaild parameters.");
                                                }
                                                else
                                                    ModClient.print("Stop inspecting part prior to looking for another part.");
                                                break;
                                            case "start":
                                                if (!_inspectingPart)
                                                {
                                                    ModClient.print("Find part prior to starting.");
                                                    return;
                                                }
                                                if (_startedInspectingPart)
                                                {
                                                    ModClient.print("Already inspecting part.");
                                                    return;
                                                }
                                                _startedInspectingPart = true;
                                                _inspectingPart.StartCoroutine(movePartCoroutine());
                                                ModClient.print("Started inspecting {0}..", _inspectingPart.name);
                                                break;
                                            case "stop":
                                                if (!_inspectingPart)
                                                {
                                                    ModClient.print("Find part prior to stopping.");
                                                    return;
                                                }
                                                if (!_startedInspectingPart)
                                                {
                                                    ModClient.print("You need to start inspecting a part prior to stopping..");
                                                    return;
                                                }
                                                _startedInspectingPart = false;
                                                ModClient.print("Stopped inspecting {0}..", _inspectingPart.name);
                                                break;
                                            case "print":
                                                if (!_inspectingPart)
                                                {
                                                    ModClient.print("Find part prior to printing transform info.");
                                                    return;
                                                }
                                                ModClient.print("{0} Transform info:\nposition: {1}\nrotation: {2}", _inspectingPart.name, _inspectingPart.transform.localPosition, _inspectingPart.transform.localEulerAngles);
                                                break;
                                            default:
                                                ModClient.print("unknown command");
                                                ModClient.print(helpListEditInstallTransformPart);
                                                break;
                                        }
                                    }
                                    else
                                        ModClient.print("command, \"modapi debug editpartinstalltransform\" expects atleast 1 argument, a sub command. Use \"modapi help debug editpartinstalltransform\" to see all vaild parameters.");
                                    break;
                                case "tpp":
                                case "tppart":
                                case "teleportpart":
                                    if (args.Length >= 3)
                                    {
                                        Part part = findPartByName(args[2]);
                                        if (part)
                                        {
                                            if (part.installed && args.Length == 3 && args[3] != "-f")
                                                ModClient.print("cannot teleport part while it's installed... use \"-f\" to force an uninstall and teleport.");
                                            else
                                            {
                                                if (part.installed && args.Length == 3 && args[3] == "-f")
                                                    part.disassemble();
                                                Rigidbody rb = part.GetComponent<Rigidbody>();
                                                if (rb)
                                                    if (!rb.isKinematic)
                                                        rb = null;
                                                    else
                                                        rb.isKinematic = true;
                                                part.transform.position = Camera.main.transform.position;
                                                if (rb)
                                                    rb.isKinematic = false;
                                            }
                                        }
                                        else
                                            ModClient.print("Use \"modapi listparts\" to see all vaild parameters.");
                                    }
                                    else
                                        ModClient.print("command, \"modapi debug teleportpart\" expects atleast 1 argument, the parts name. Use \"modapi listparts\" to see all vaild parameters.");
                                    break;
                                default:
                                    ModClient.print("unknown command");
                                    ModClient.print(helpListDebug);
                                    break;
                            }
                        }
                        else
                            ModClient.print("command, \"modapi debug\" expects atleast 1 argument, a sub command. Use \"modapi help debug\" to see all vaild parameters.");
                        break;
                    default:
                        ModClient.print("unknown command");
                        ModClient.print(helpListGeneral);
                        break;
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Finds the part by name.
        /// </summary>
        /// <param name="inPartName">The parts name to find</param>
        private Part findPartByName(string inPartName)
        {
            // Written, 10.09.2021

            Part part = ModClient.parts.Find(_part => _part.name == inPartName);
            if (!part)
            {
                ModClient.print("Could not find the part by name.");
            }
            else
            {
                ModClient.print("Found part: {0}", part.name);
            }
            return part;
        }

        private IEnumerator movePartCoroutine()
        {
            while (_startedInspectingPart)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
        #endregion
    }
}
