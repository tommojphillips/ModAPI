using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

using MSCLoader;

using TommoJProductions.ModApi.Attachable;
using TommoJProductions.ModApi.Attachable.CallBacks;
using TommoJProductions.ModApi.Database;
using TommoJProductions.ModApi.Database.GameParts;

using UnityEngine;
using static TommoJProductions.ModApi.ModClient;
using static UnityEngine.GUILayout;

namespace TommoJProductions.ModApi
{
    /// <summary>
    /// Represents debug behaviour for mod api.
    /// </summary>
    public class DevMode : MonoBehaviour
    {
        private static Part _inspectionPart;
        private bool inspectionPartSet = false;
        private Vector3[] inspectionPartPosition;
        private Vector3[] inspectionPartEuler;
        private bool inspectingBolt = false;
        private bool inspectionBoltSet = false;

        internal static Bolt inspectionBolt;
        internal static Vector3 inspectionPosition;
        internal static Vector3 inspectionEuler;
        internal static float inspectionOffset;

        internal static bool listGameParts = false;
        internal static bool listModApiParts = false;

        private readonly int top = 75;
        private readonly int margin = 20;
        private int height => Screen.height - top;
        private readonly int width = 500;
        private int left => Screen.width - width - margin;
        private Vector2 triggerScroll;
        private Vector2 gamePartsScroll;
        private ScrollViewScope scrollViewScope;
        private Color c1 = new Color32(0, 178, 230, 176);
        private readonly GUIStyle style = new GUIStyle();
        private Texture2D defaultBackground;
        private Texture2D primaryItemBackground;
        private Texture2D secondaryItemBackground;

        private GamePart[] allGameParts;
        private GamePart[] listedGameParts;
        private Part[] listedModApiParts;

        private bool showWearOnlyParts = false;
        private bool showBoltOnlyParts = false;
        private string searchString = string.Empty;

        void Start()
        {
            defaultBackground = createTextureFromColor(1, 1, new Color32(125, 125, 125, 255));
            primaryItemBackground = createTextureFromColor(1, 1, new Color32(75, 140, 200, 255));
            secondaryItemBackground = createTextureFromColor(1, 1, new Color32(24, 120, 130, 255));

            allGameParts = (Database.Database.databaseMotor.getProperties().Select(p => (GamePart)p.GetValue(Database.Database.databaseMotor, null))).ToArray();
        }
        void OnGUI() 
        {
            // Written, 03.07.2022

            GUI.skin.box.normal.background = defaultBackground;

            if (devMode)
            {
                drawDevGui();
            }

            if (_inspectionPart)
            {
                Transform t;
                int tl;
                
                tl = _inspectionPart.triggers.Length;

                if (!inspectionPartSet)
                {
                    inspectionPartPosition = new Vector3[tl];
                    inspectionPartEuler = new Vector3[tl];
                    for (int i = 0; i < tl; i++)
                    {
                        t = _inspectionPart.triggers[i].triggerGameObject.transform;
                        inspectionPartPosition[i] = t.localPosition;
                        inspectionPartEuler[i] = t.localEulerAngles;
                    }
                    inspectionPartSet = true;
                }
                drawPartGui();
                if (_inspectionPart.hasBolts)
                {
                    if (inspectingBolt)
                    {
                        if (!inspectionBoltSet)
                        {
                            if (inspectionBolt == null)
                                inspectionBolt = _inspectionPart.bolts[0];
                            inspectionPosition = inspectionBolt.startPosition;
                            inspectionEuler = inspectionBolt.startEulerAngles;
                            if (inspectionBolt.boltSettings.addNut)
                                inspectionOffset = inspectionBolt.boltSettings.addNutSettings.nutOffset;
                            inspectionBoltSet = true;
                        }
                        using (AreaScope area = new AreaScope(new Rect(left - width - margin, top, width, height)))
                        {
                            using (new HorizontalScope("box"))
                            { 
                                style.border = new RectOffset(1, 1, top: 1, 2);
                                style.alignment = TextAnchor.MiddleCenter;
                                style.fontSize = 14;
                                for (int i = 0; i < _inspectionPart.bolts.Length; i++)
                                {
                                    if (_inspectionPart.bolts[i] == inspectionBolt)
                                    {
                                        style.normal.textColor = c1;
                                        GUI.skin.box.normal.background = primaryItemBackground;
                                    }
                                    else
                                    {
                                        style.normal.textColor = Color.white;
                                        GUI.skin.box.normal.background = secondaryItemBackground;
                                    }

                                    if (Button(_inspectionPart.bolts[i].boltModel.name, style))
                                    {
                                        setInspectionBolt(_inspectionPart.bolts[i]);
                                    }
                                }
                                GUI.skin.box.normal.background = defaultBackground;
                            }
                            drawBoltGui();
                        }
                    }
                }
            }
        }
        void Update()
        {
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.P))
            {
                setInspectionPart(raycastForBehaviour<Part>());
            }
        }

        internal void setInspectionPart(Part part)
        {
            _inspectionPart = part;
            inspectionPartSet = false;
        }
        internal void setInspectionBolt(Bolt bolt)
        {
            inspectionBolt = bolt;
            inspectionBoltSet = false;
        }

        private void drawToolStatsGui()
        {
            getToolWrenchSize_boltSize.drawProperty();
            drawProperty("tool size", getToolWrenchSize_float);
            //drawProperty("spanner bolting speed", getSpannerBoltingSpeed);
            //drawProperty("rachet bolting speed", getRachetBoltingSpeed);
        }
        private void drawDevGui()
        {
            using (AreaScope area = new AreaScope(new Rect(margin, top, width, height)))
            {
                using (new VerticalScope("box", Width(275)))
                {
                    drawProperty($"ModAPI v{VersionInfo.version} | DEV MODE |\n- Use <b>Ctrl+P</b> while looking at a part to inspect it");
                    drawToolStatsGui();

                    if (Button("List Mod Api Parts"))
                    {
                        listModApiParts = !listModApiParts;
                        listGameParts = false;
                    }
                    if (Button("List Game Parts"))
                    {
                        listGameParts = !listGameParts;
                        listModApiParts = false;
                    }
                    drawModApiLoaderInfo();
                }

                using (new VerticalScope("box"))
                {
                    drawGamePartsList();
                    drawModApiList();
                }
            }
        }
        private void drawModApiLoaderInfo()
        {
            drawProperty("ModApi:", ModApiLoader.modapiGo?.ToString() ?? "null");
            drawProperty("picked object:", ModApiLoader.pickedObject?.ToString() ?? "null");
            drawProperty("picked part:", ModApiLoader.pickedPart?.ToString() ?? "null");
            drawProperty("currently detected bolt:", ModApiLoader.lookingAtCallback?.ToString() ?? "null");
            drawProperty("activateGameStateInjected:", ModApiLoader.activateGameStateInjected);
            drawProperty("activateGameState:", ModApiLoader.activateGameState?.ToString() ?? "null");
            drawProperty("loader inject action:", ModApiLoader.actionCallback?.ToString() ?? "null");
            drawProperty("picked part SET:", ModApiLoader.pickedPartSet);
            drawProperty("inherenty picked part SET:", ModApiLoader.inherentyPickedPartsSet);
        }

        private void drawModApiList()
        {
            if (listModApiParts)
            {
                using (new HorizontalScope())
                {
                    drawPropertyEdit("Search", ref searchString);
                    drawPropertyBool("Show only bolt modapi-parts", ref showBoltOnlyParts);
                }
                using (scrollViewScope = new ScrollViewScope(gamePartsScroll, false, false))
                {
                    gamePartsScroll = scrollViewScope.scrollPosition;
                    scrollViewScope.handleScrollWheel = true;

                    listedModApiParts = loadedParts.ToArray();

                    if (!string.IsNullOrEmpty(searchString))
                    {
                        listedModApiParts = loadedParts.Where(p => p.name.ToLower().Contains(searchString)).ToArray();
                    }

                    if (showBoltOnlyParts)
                    {
                        listedModApiParts = loadedParts.Where(p => p.hasBolts).ToArray();
                    }

                    for (int i = 0; i < listedModApiParts.Length; i++)
                    {
                        if (i.isEven())
                        {
                            GUI.skin.box.normal.background = primaryItemBackground;
                        }
                        else
                        {
                            GUI.skin.box.normal.background = secondaryItemBackground;
                        }
                        using (new VerticalScope("box"))
                        {
                            drawProperty(listedModApiParts[i].name);
                            if (Button("Inspect"))
                            {
                                setInspectionPart(listedModApiParts[i]);
                            }
                        }
                    }
                    GUI.skin.box.normal.background = defaultBackground;
                }
            }
        }

        private void drawGamePartsList()
        {
            if (listGameParts)
            {
                using (new HorizontalScope())
                {
                    drawPropertyEdit("Search", ref searchString);
                    drawPropertyBool("Show only wear-able game-parts", ref showWearOnlyParts);
                }


                listedGameParts = allGameParts;

                if (!string.IsNullOrEmpty(searchString))
                {
                    listedGameParts = listedGameParts.Where(p => p.thisPart.Value.name.ToLower().Contains(searchString)).ToArray();
                }

                if (showWearOnlyParts)
                {
                    listedGameParts = allGameParts.Where(p => p is GamePartWear).ToArray();
                }
                using (scrollViewScope = new ScrollViewScope(gamePartsScroll, false, false))
                {
                    gamePartsScroll = scrollViewScope.scrollPosition;
                    scrollViewScope.handleScrollWheel = true;
                    for (int i = 0; i < listedGameParts.Length; i++)
                    {
                        if (i.isEven())
                        {
                            GUI.skin.box.normal.background = primaryItemBackground;
                        }
                        else
                        {
                            GUI.skin.box.normal.background = secondaryItemBackground;
                        }
                        using (new VerticalScope("box"))
                        {
                            drawGamePartInfo(listedGameParts[i]);
                        }
                    }
                    GUI.skin.box.normal.background = defaultBackground;
                }
            }
        }

        private void drawPartGui() 
        {
            using (AreaScope area = new AreaScope(new Rect(left, top, width, height - top)))
            {
                using (new VerticalScope("box"))
                {
                    using (new HorizontalScope())
                    {
                        using (new VerticalScope("box"))
                        {
                            drawProperty("Part Inspection", _inspectionPart.name);
                            Space(1);
                            drawProperty($"Trigger routine: {(_inspectionPart.triggerRoutine == null ? "in" : "")}active");
                            drawProperty("Position", _inspectionPart.transform.position);
                            drawProperty("Euler", _inspectionPart.transform.eulerAngles);
                            drawProperty("Installed", _inspectionPart.installed);
                            drawProperty($"Picked up: {_inspectionPart.pickedUp} | {(_inspectionPart.inherentlyPickedUp ? "inherently" : "")}");
                            drawProperty("InTrigger", _inspectionPart.inTrigger);
                            drawProperty("Install point index", _inspectionPart.installPointIndex);
                            _inspectionPart.partSettings.assembleType.drawProperty();
                        }
                        using (new VerticalScope())
                        {
                            if (_inspectionPart.partSettings.assembleType == Part.AssembleType.joint)
                            {
                                using (new VerticalScope("box"))
                                {
                                    drawProperty("Joint settings");
                                    if (_inspectionPart.hasBolts)
                                    {
                                        drawPropertyBool("bolt tightness effects breakforce", ref _inspectionPart.partSettings.assemblyTypeJointSettings.boltTightnessEffectsBreakforce);
                                        if (_inspectionPart.partSettings.assemblyTypeJointSettings.boltTightnessEffectsBreakforce)
                                        {
                                            drawPropertyEdit("breakforce min", ref _inspectionPart.partSettings.assemblyTypeJointSettings.breakForceMin);
                                        }
                                        drawPropertyEdit("tightness threshold", ref _inspectionPart.partSettings.tightnessThreshold);
                                    }
                                    drawPropertyEdit("breakforce", ref _inspectionPart.partSettings.assemblyTypeJointSettings.breakForce);
                                }
                            }
                            if (_inspectionPart.joint)
                            {
                                using (new VerticalScope("box"))
                                {
                                    if (_inspectionPart.joint.breakForce == float.PositiveInfinity)
                                    {
                                        drawProperty($"breakforce: unbreakable (infinity)");
                                    }
                                    else
                                    {
                                        drawProperty($"breakforce: {_inspectionPart.joint.breakForce}Nm");
                                    }
                                    if (_inspectionPart.hasBolts)
                                    {
                                        float bf = _inspectionPart.partSettings.assemblyTypeJointSettings.breakForce;
                                        float thresholdObsolute = bf * _inspectionPart.partSettings.tightnessThreshold;
                                        drawProperty($"\t- Threshold: {thresholdObsolute}Nm ({thresholdObsolute / bf * 100})");
                                    }
                                    if (Button("Update joint breakforce"))
                                    {
                                        _inspectionPart.updateJointBreakForce();
                                    }
                                }
                            }
                            if (_inspectionPart.hasBolts)
                            {
                                using (new VerticalScope("box"))
                                {
                                    drawProperty($"{(_inspectionPart.bolted ? "" : "Not")} Bolted");
                                    drawProperty($"Tightness: {_inspectionPart.tightnessTotal} / {_inspectionPart.maxTightnessTotal} ({_inspectionPart.tightnessTotal / _inspectionPart.maxTightnessTotal * 100})");
                                    drawProperty($"Tightness Step: {_inspectionPart.tightnessStep}");
                                    drawPropertyBool("Inspect Bolts", ref inspectingBolt);
                                    using (new HorizontalScope("box"))
                                    {
                                        
                                        if (Button("Update tightness vars"))
                                        {
                                            _inspectionPart.setupBoltTightnessVariables();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Space(1);
                    using (new HorizontalScope("box"))
                    {
                        if (_inspectionPart.hasBolts && Button((_inspectionPart.boltParent.activeInHierarchy ? "Deactivate" : "Activate") + " bolts"))
                        {
                            _inspectionPart.boltParent.SetActive(!_inspectionPart.boltParent.activeInHierarchy);
                        }
                        if (_inspectionPart.installed)
                        {
                            if (_inspectionPart.tightnessTotal < _inspectionPart.maxTightnessTotal)
                            {
                                if (Button("Tighten bolts"))
                                {
                                    _inspectionPart.setMaxBoltTightness();
                                }
                            }
                            else
                            {
                                if (Button("Loosen bolts"))
                                {
                                    _inspectionPart.resetBoltTightness();
                                }
                            }
                            if (Button("Disassemble"))
                            {
                                _inspectionPart.disassemble();
                            }
                        }
                        if (Button("Teleport to player"))
                        {
                            _inspectionPart.teleport(true, getPOV.transform.position);
                        }
                        if (Button("Teleport to part"))
                        {
                            getPlayer.teleport(_inspectionPart.transform.position);
                        }
                    }
                    Space(1);
                    drawProperty("Triggers");
                    using (scrollViewScope = new ScrollViewScope(triggerScroll, false, false))
                    {
                        triggerScroll = scrollViewScope.scrollPosition;
                        scrollViewScope.handleScrollWheel = true;

                        if (_inspectionPart.triggers != null && _inspectionPart.triggers.Length > 0)
                        {
                            for (int i = 0; i < _inspectionPart.triggers.Length; i++)
                            {
                                if (i.isEven())
                                {
                                    GUI.skin.box.normal.background = primaryItemBackground;
                                }
                                else
                                {
                                    GUI.skin.box.normal.background = secondaryItemBackground;
                                }
                                using (new VerticalScope("box"))
                                {
                                    using (new HorizontalScope())
                                    {
                                        drawProperty(_inspectionPart.triggers[i].triggerName);
                                        if (Button("Teleport to trigger"))
                                        {
                                            getPlayer.teleport(_inspectionPart.triggers[i].triggerGameObject.transform.position);
                                        }
                                        if (!_inspectionPart.installed)
                                        {
                                            if (Button("Assemble"))
                                            {
                                                _inspectionPart.assemble(_inspectionPart.installPointColliders[i]);
                                            }
                                        }
                                    }
                                    drawPropertyVector3("position", ref inspectionPartPosition[i]);
                                    drawPropertyVector3("euler", ref inspectionPartEuler[i]);

                                    if (Button("apply"))
                                    {
                                        _inspectionPart.triggers[i].partPivot.transform.localPosition = inspectionPartPosition[i];
                                        _inspectionPart.triggers[i].partPivot.transform.localEulerAngles = inspectionPartEuler[i];
                                    }
                                }
                            }
                            GUI.skin.box.normal.background = defaultBackground;
                        }
                        else
                        {
                            drawProperty("No triggers");
                        }
                    }
                }
            }
        }
        private void drawBoltGui()
        {
            using (new VerticalScope("box"))
            {
                using (new VerticalScope("box"))
                {
                    drawProperty("Bolt Inspection", inspectionBolt.boltCallback.name);
                    Space(1);
                    drawProperty($"routine: {(inspectionBolt.boltRoutine == null ? "in" : "")}active");
                    inspectionBolt.boltSettings.boltType.drawProperty();
                    drawProperty("Bolt tightness", inspectionBolt.loadedSaveInfo.boltTightness);
                    inspectionBolt.boltSettings.boltSize.drawProperty();
                    if (inspectionBolt.boltSettings.addNut)
                    {
                        drawProperty("Nut tightness", inspectionBolt.loadedSaveInfo.addNutTightness);
                        (inspectionBolt.boltSettings.addNutSettings.nutSize ?? inspectionBolt.boltSettings.boltSize).drawProperty();
                    }
                }
                Space(1);
                using (new VerticalScope("box"))
                {
                    drawPropertyVector3("start position", ref inspectionPosition);
                    drawPropertyVector3("start euler", ref inspectionEuler);
                    if (inspectionBolt.boltSettings.addNut)
                        drawPropertyEdit("Nut offset", ref inspectionOffset);
                    if (Button("apply"))
                    {
                        inspectionBolt.startPosition = inspectionPosition;
                        inspectionBolt.startEulerAngles = inspectionEuler;
                        inspectionBolt.boltSettings.addNutSettings.nutOffset = inspectionOffset;
                        inspectionBolt.updateModelPosition();
                    }
                }
                Space(1);
                using (new VerticalScope("box"))
                {
                    drawPropertyVector3("pos direction", ref inspectionBolt.boltSettings.posDirection);
                    drawPropertyVector3("rot direction", ref inspectionBolt.boltSettings.rotDirection);
                    drawPropertyEdit("pos step", ref inspectionBolt.boltSettings.posStep);
                    drawPropertyEdit("rot step", ref inspectionBolt.boltSettings.rotStep);
                    //drawPropertyEdit("tightness step", ref inspectionBolt.boltSettings.tightnessStep);
                }
            }
        }
    }
}
