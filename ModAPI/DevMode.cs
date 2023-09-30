using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using MSCLoader;

using TommoJProductions.ModApi.Attachable;
using TommoJProductions.ModApi.Database;
using TommoJProductions.ModApi.Database.GameParts;
using UnityEngine;

using static TommoJProductions.ModApi.Attachable.Part;
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
        private bool _inspectionPartSet = false;
        private Vector3[] _inspectionPartPivotPosition;
        private Vector3[] _inspectionPartPivotEuler;
        private Vector3[] _inspectionPartTriggerPosition;
        private Vector3[] _inspectionPartTriggerEuler;
        private float[] _inspectionPartTriggerRadius;
        private float _tightnessThresholdPercent;
        private bool _inspectingBolt = false;
        private bool _inspectionBoltSet = false;

        internal static Bolt inspectionBolt;
        internal static Vector3 inspectionPosition;
        internal static Vector3 inspectionEuler;
        internal static Vector3 inspectionPosDir;
        internal static Vector3 inspectionRotDir;
        internal static float inspectionPosStep;
        internal static float inspectionRotStep;
        private BoltWithNut inspectionBoltWithNut;
        internal static float inspectionOffset;

        internal static bool listGameParts = false;
        internal static bool listModApiParts = false;
        internal static bool showLog = false;

        private readonly int _top = 5;
        private readonly int _margin = 5;
        private int _height => Screen.height - (_top  * 2);
        private readonly int _width = 500;
        private int _left => Screen.width - _width - _margin;
        private Vector2 _triggerScroll;
        private Vector2 _gamePartsScroll;
        private Vector2 _logScroll;
        private ScrollViewScope _scrollViewScope;
        private Color _c1 = new Color32(0, 180, 230, 176);
        private Color _c2 = new Color32(0, 180, 120, 176);
        private readonly GUIStyle _style = new GUIStyle();
        private readonly GUIStyle _borderStyle = new GUIStyle();
        private Texture2D _defaultBackground;
        private Texture2D _primaryItemBackground;
        private Texture2D _secondaryItemBackground;

        private GamePart[] _allGameParts;
        private GamePart[] _listedGameParts;
        private Part[] _listedModApiParts;

        private bool _showWearOnlyParts = false;
        private bool _showBoltOnlyParts = false;
        private string _searchString = string.Empty;

        private bool _toggleBoltDetectionStats;
        private bool _toggleLoaderInfo;
        private bool _showDevGui = true;

        private Bolt[] _selectedBolts;
        private bool _showPartBolts;
        private Trigger _selectedBoltTrigger;
        private Trigger[] _inspectionPartTriggers;
        private bool _inspectionTriggersHasBolts;
        private bool _showAllBolts;
        private bool _inspectTriggers;
        private BoltManager _boltManager;
        private PartManager _partManager;

        void Start()
        {
            _defaultBackground = createTextureFromColor(1, 1, new Color32(125, 125, 125, 255));
            _primaryItemBackground = createTextureFromColor(1, 1, new Color32(75, 140, 200, 255));
            _secondaryItemBackground = createTextureFromColor(1, 1, new Color32(24, 120, 130, 255));

            _allGameParts = (Database.Database.databaseMotor.getProperties().Select(p => (GamePart)p.GetValue(Database.Database.databaseMotor, null))).ToArray();

            _style.border = new RectOffset(1, 1, top: 1, 2);
            _style.alignment = TextAnchor.MiddleCenter;
            _style.fontSize = 14;

            _borderStyle.border = new RectOffset(5, 5, 5, 5);

            _boltManager = getBoltManager;
            _partManager = getPartManager;
        }
        void OnGUI() 
        {
            // Written, 03.07.2022

            GUI.skin.box.normal.background = _defaultBackground;

            if (devMode)
            {
                drawDevGui();
            }

            if (_inspectionPart)
            {
                if (!_inspectionPartSet)
                {
                    Trigger t;
                    int tl;
                    _inspectionPartTriggers = Trigger.loadedTriggers.Where(_t => _t.callback.triggerData == _inspectionPart.triggerData).ToArray();
                    _inspectionTriggersHasBolts = _inspectionPartTriggers.Any(_t => _t.hasBolts);
                    _tightnessThresholdPercent = _inspectionPart.partSettings.tightnessThreshold * 100;

                    tl = _inspectionPartTriggers.Length;
                    _inspectionPartPivotPosition = new Vector3[tl];
                    _inspectionPartPivotEuler = new Vector3[tl];
                    _inspectionPartTriggerPosition = new Vector3[tl];
                    _inspectionPartTriggerEuler = new Vector3[tl];
                    _inspectionPartTriggerRadius = new float[tl];

                    for (int i = 0; i < tl; i++)
                    {
                        t = _inspectionPartTriggers[i];

                        if (!t.settings.useTriggerTransformData)
                        {
                            _inspectionPartPivotPosition[i] = t.partPivot.transform.localPosition;
                            _inspectionPartPivotEuler[i] = t.partPivot.transform.localEulerAngles;
                        }
                        _inspectionPartTriggerPosition[i] = t.gameObject.transform.localPosition;
                        _inspectionPartTriggerEuler[i] = t.gameObject.transform.localEulerAngles;
                        _inspectionPartTriggerRadius[i] = t.collider.radius;
                    }

                    _selectedBoltTrigger = _inspectionPartTriggers[0];
                    if (_showPartBolts)
                    {
                        _selectedBolts = _inspectionPart.bolts;
                    }
                    else if (_showAllBolts)
                    {
                        getAllBolts();
                    }
                    else
                    {
                        updateSelectedBolts(_selectedBoltTrigger);
                    }

                    _inspectionPartSet = true;
                }
                drawPartGui();
                if (_inspectionPart.hasBolts || _inspectionTriggersHasBolts)
                {
                    if (_inspectingBolt)
                    {
                        if (!_inspectionBoltSet)
                        {
                            if (inspectionBolt == null)
                            {
                                if (_inspectionPart.hasBolts)
                                {
                                    inspectionBolt = _inspectionPart.bolts[0];
                                }
                                else if (_selectedBoltTrigger.hasBolts)
                                {
                                    inspectionBolt = _inspectionPartTriggers[0].bolts[0];
                                }
                            }
                            inspectionPosition = inspectionBolt.startPosition;
                            inspectionEuler = inspectionBolt.startEulerAngles;
                            inspectionPosDir = inspectionBolt.settings.posDirection;
                            inspectionRotDir = inspectionBolt.settings.rotDirection;
                            inspectionPosStep = inspectionBolt.settings.posStep;
                            inspectionRotStep = inspectionBolt.settings.rotStep;

                            inspectionBoltWithNut = inspectionBolt as BoltWithNut;

                            if (inspectionBoltWithNut != null)
                            {
                                inspectionOffset = inspectionBoltWithNut.boltWithNutSettings.offset;
                            }

                            _inspectionBoltSet = true;
                        }
                        using (AreaScope area = new AreaScope(new Rect(_left - _width - _margin, _top, _width, _height)))
                        {
                            using (new HorizontalScope("box"))
                            {
                                if (_showPartBolts)
                                {
                                    _style.normal.textColor = _c1;
                                    GUI.skin.box.normal.background = _primaryItemBackground;
                                }
                                else
                                {
                                    _style.normal.textColor = Color.white;
                                    GUI.skin.box.normal.background = _secondaryItemBackground;
                                }
                                if (Button("Part bolts", _style))
                                {
                                    _showPartBolts = !_showPartBolts;

                                    if (_showPartBolts)
                                    {
                                        _showAllBolts = false;
                                        _selectedBolts = _inspectionPart.bolts;
                                    }
                                    else
                                    {
                                        updateSelectedBolts(_selectedBoltTrigger);
                                        setInspectionBolt(_selectedBolts[0]);
                                    }
                                }
                                if (_showAllBolts)
                                {
                                    _style.normal.textColor = _c1;
                                    GUI.skin.box.normal.background = _primaryItemBackground;
                                }
                                else
                                {
                                    _style.normal.textColor = Color.white;
                                    GUI.skin.box.normal.background = _secondaryItemBackground;
                                }
                                if (Button("All bolts", _style))
                                {
                                    _showAllBolts = !_showAllBolts;

                                    if (_showAllBolts)
                                    {
                                        _showPartBolts = false;
                                        getAllBolts();
                                    }
                                    else
                                    {
                                        updateSelectedBolts(_selectedBoltTrigger);
                                        setInspectionBolt(_selectedBolts[0]);
                                    }
                                }

                                if (!_showPartBolts && !_showAllBolts)
                                {
                                    for (int i = 0; i < _inspectionPartTriggers.Length; i++)
                                    {
                                        if (_inspectionPartTriggers[i] == _selectedBoltTrigger)
                                        {
                                            _style.normal.textColor = _c1;
                                            GUI.skin.box.normal.background = _primaryItemBackground;
                                        }
                                        else
                                        {
                                            _style.normal.textColor = Color.white;
                                            GUI.skin.box.normal.background = _secondaryItemBackground;
                                        }
                                        if (Button(_inspectionPartTriggers[i].triggerID, _style))
                                        {
                                            if (_inspectionPartTriggers[i].triggerID == _selectedBoltTrigger.triggerID)
                                                continue;

                                            _selectedBoltTrigger = _inspectionPartTriggers[i];
                                            updateSelectedBolts(_inspectionPartTriggers[i]);
                                            setInspectionBolt(_selectedBolts[0]);
                                        }
                                    }
                                }
                                else
                                {
                                    if (_inspectionTriggersHasBolts)
                                    {
                                        if (Button("Triggers ->"))
                                        {
                                            _showAllBolts = false;
                                            _showPartBolts = false;
                                        }
                                    }
                                }
                                GUI.skin.box.normal.background = _defaultBackground;
                            }

                            if (_selectedBolts != null && _selectedBolts.Length > 0)
                            {
                                using (new HorizontalScope("box"))
                                {
                                    for (int i = 0; i < _selectedBolts.Length; i++)
                                    {
                                        if (_selectedBolts[i].model == _boltManager.bolt.Value)
                                        {
                                            _style.normal.textColor = _c2;
                                        }                                        
                                        else if (_selectedBolts[i] == inspectionBolt)
                                        {
                                            _style.normal.textColor = _c1;
                                        }
                                        else
                                        {
                                            _style.normal.textColor = Color.white;
                                        }

                                        if (Button(_selectedBolts[i].model.name, _style))
                                        {
                                            setInspectionBolt(_selectedBolts[i]);
                                        }
                                    }
                                }
                                drawBoltGui();
                            }
                            else
                            {
                                using (new HorizontalScope("box"))
                                    Label("No Bolts");
                            }
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

        private void getAllBolts()
        {
            // Written, 25.08.2023

            List<Bolt> bolts = new List<Bolt>();
            if (_inspectionPart.hasBolts)
                bolts.AddRange(_selectedBoltTrigger.bolts);

            for (int i = 0; i < _inspectionPartTriggers.Length; i++)
            {
                if (_inspectionPartTriggers[i].hasBolts)
                    bolts.AddRange(_inspectionPartTriggers[i].bolts);
            }
            _selectedBolts = bolts.ToArray();
        }
        private void updateSelectedBolts(Trigger trigger)
        {
            // Written, 19.08.2023

            _selectedBolts = trigger.bolts;
        }
        internal void setInspectionPart(Part part)
        {
            _inspectionPart = part;
            _inspectionPartSet = false;
            setInspectionBolt(null);
        }
        internal void setInspectionBolt(Bolt bolt)
        {
            inspectionBolt = bolt;
            _inspectionBoltSet = false;
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
            using (AreaScope area = new AreaScope(new Rect(_margin, _top, _width, _height)))
            {
                if (_showDevGui)
                {
                    using (new VerticalScope("box", Width(275)))
                    {
                        using (new HorizontalScope("box", Width(275)))
                        {

                            drawProperty($"ModAPI v{ModApi.VersionInfo.version} BUILD {ModApi.VersionInfo.build} | DEV MODE |");
                            if (Button("-", new GUIStyle() { alignment = TextAnchor.MiddleCenter, }))
                            {
                                _showDevGui = false;
                            }
                            Space(5);
                            if (Button("X", new GUIStyle() { alignment = TextAnchor.MiddleCenter, }))
                            {
                                devMode = false;
                            }
                        }

                        drawProperty($"- Use <b>Ctrl+P</b> while looking at a part to inspect it");
                        drawToolStatsGui();

                        if (Button((listModApiParts ? "Hide" : "List") + " Mod Api Parts"))
                        {
                            listModApiParts = !listModApiParts;
                            listGameParts = false;
                        }
                        if (Button((listGameParts ? "Hide" : "List") + " Game Parts"))
                        {
                            listGameParts = !listGameParts;
                            listModApiParts = false;
                        }
                        if (Button((showLog ? "Hide" : "Show") + " log"))
                        {
                            showLog = !showLog;
                        }
                        if (_inspectionPart)
                        {
                            if (Button("Close part inspection"))
                            {
                                setInspectionPart(null);
                            }
                        }
                        else
                        {
                            if (Button((_inspectTriggers ? "Close" : "Open") + " Trigger inspection"))
                            {
                                _inspectTriggers = !_inspectTriggers;
                            }
                        }

                        Space(5);
                        drawModApiLoaderInfo();
                        drawBoltDetectionStats();
                    }
                    using (new VerticalScope("box"))
                    {
                        drawGamePartsList();
                        drawModApiList();
                    }
                }
                else
                {
                    using (new VerticalScope("box", Width(25)))
                    {
                        if (Button("+", new GUIStyle() { alignment = TextAnchor.MiddleCenter, }, ExpandWidth(false)))
                        {
                            _showDevGui = true;
                        }
                    }
                }
            }

            if (showLog)
            {
                using (AreaScope area = new AreaScope(new Rect((_margin * 2) + _width, _top, _width * 2, _height)))
                {
                    drawLog();
                }
            }
        }

        private void drawBoltDetectionStats()
        {
            if (Button(_toggleBoltDetectionStats ? "Hide" : "Show" + " Bolt detection stats"))
            {
                _toggleBoltDetectionStats = !_toggleBoltDetectionStats;
            }
            if (_toggleBoltDetectionStats)
            {
                drawProperty("Detected bolt (toolmode)", _boltManager.bolt.Value?.ToString() ?? "null");
                drawProperty("current callback", _boltManager.currentCallback?.ToString() ?? "null");
                if (_boltManager.currentCallback)
                {
                    drawProperty("bolt check", _boltManager.currentCallback.boltCheck);
                }
            }
        }

        private void drawModApiLoaderInfo()
        {
            if (Button((_toggleLoaderInfo ? "Hide" : "Show") + " loader info"))
            {
                _toggleLoaderInfo = !_toggleLoaderInfo;
            }
            if (_toggleLoaderInfo)
            {
                drawProperty("ModApi:", modapiGo?.ToString() ?? "null");
                drawProperty("picked object:", _partManager.pickedObject?.ToString() ?? "null");
                drawProperty("picked part SET:", _partManager.pickedPartSet);
                drawProperty("inherenty picked part SET:", _partManager.inherentlyPickedPartsSet);
            }
        }

        private void drawModApiList()
        {
            void updateParts()
            {
                _listedModApiParts = loadedParts.ToArray();

                if (!string.IsNullOrEmpty(_searchString))
                {
                    _listedModApiParts = loadedParts.Where(p => p.partID.ToLower().Contains(_searchString.ToLower())).ToArray();
                }

                if (_showBoltOnlyParts)
                {
                    _listedModApiParts = loadedParts.Where(p => p.hasBolts || Trigger.triggerDictionary[p.triggerData].Any(t => t.Value.hasBolts)).ToArray();
                }
            }

            if (listModApiParts)
            {
                using (new HorizontalScope())
                {
                    if (drawPropertyEdit("Search", ref _searchString))
                    {
                        updateParts();
                    }
                    if (drawPropertyBool("Show only boltable modapi-parts", ref _showBoltOnlyParts))
                    {
                        updateParts();
                    }
                    if (_listedGameParts == null)
                    {
                        updateParts();
                    }
                }
                using (_scrollViewScope = new ScrollViewScope(_gamePartsScroll, false, false))
                {
                    _gamePartsScroll = _scrollViewScope.scrollPosition;
                    _scrollViewScope.handleScrollWheel = true;
                    
                    for (int i = 0; i < _listedModApiParts.Length; i++)
                    {
                        if (_listedModApiParts[i].isPlayerLookingAtByPickUp())
                        {
                            _style.normal.textColor = _c2;
                        }
                        else
                        {
                            _style.normal.textColor = Color.white;
                        }
                        
                        if (i.isEven())
                        {
                            GUI.skin.box.normal.background = _primaryItemBackground;
                        }
                        else
                        {
                            GUI.skin.box.normal.background = _secondaryItemBackground;
                        }
                        using (new VerticalScope("box"))
                        {
                            Label(_listedModApiParts[i].partID, _style);

                            if (_inspectionPart != _listedModApiParts[i])
                            {
                                if (Button("Inspect"))
                                {
                                    setInspectionPart(_listedModApiParts[i]);
                                }
                            }
                            else
                            {
                                if (Button("Close"))
                                {
                                    setInspectionPart(null);
                                }
                            }
                        }
                    }
                    GUI.skin.box.normal.background = _defaultBackground;
                }
            }
        }

        private void drawGamePartsList()
        {
            if (listGameParts)
            {
                using (new HorizontalScope())
                {
                    drawPropertyEdit("Search", ref _searchString);
                    drawPropertyBool("Show only wear-able game-parts", ref _showWearOnlyParts);
                }


                _listedGameParts = _allGameParts;

                if (!string.IsNullOrEmpty(_searchString))
                {
                    _listedGameParts = _listedGameParts.Where(p => p.thisPart.Value.name.ToLower().Contains(_searchString)).ToArray();
                }

                if (_showWearOnlyParts)
                {
                    _listedGameParts = _allGameParts.Where(p => p is GamePartWear).ToArray();
                }
                using (_scrollViewScope = new ScrollViewScope(_gamePartsScroll, false, false))
                {
                    _gamePartsScroll = _scrollViewScope.scrollPosition;
                    _scrollViewScope.handleScrollWheel = true;
                    for (int i = 0; i < _listedGameParts.Length; i++)
                    {
                        if (i.isEven())
                        {
                            GUI.skin.box.normal.background = _primaryItemBackground;
                        }
                        else
                        {
                            GUI.skin.box.normal.background = _secondaryItemBackground;
                        }
                        using (new VerticalScope("box"))
                        {
                            drawGamePartInfo(_listedGameParts[i]);
                        }
                    }
                    GUI.skin.box.normal.background = _defaultBackground;
                }
            }
        }

        private void drawPartGui() 
        {
            using (AreaScope area = new AreaScope(new Rect(_left, _top, _width, _height - _top)))
            {
                using (new VerticalScope("box"))
                {
                    using (new HorizontalScope())
                    {
                        using (new VerticalScope("box", MaxWidth(_width / 2)))
                        {

                            drawProperty($"Name: {_inspectionPart.name}");
                            Space(1);
                            drawProperty("Routine", $"{(_inspectionPart.triggerRoutine == null ? "In" : "A")}ctive");
                            drawProperty("Position", _inspectionPart.transform.position);
                            drawProperty("Euler", _inspectionPart.transform.eulerAngles);
                            drawProperty("Installed", _inspectionPart.installed);
                            drawPartPickedUp();
                            drawProperty("In trigger", _inspectionPart.inTrigger);
                            if (!_inspectionPart.installed)
                            {
                                drawPropertyEnumVertical(ref _inspectionPart.partSettings.assembleType);
                            }
                            else
                            {
                                _inspectionPart.partSettings.assembleType.drawProperty();
                            }
                        }
                        using (new VerticalScope("box", MaxWidth(_width / 2)))
                        {
                            if (Button("X", new GUIStyle() { alignment = TextAnchor.MiddleRight, }))
                            {
                                setInspectionPart(null);
                            }

                            if (_inspectionPart.partSettings.assembleType == AssembleType.joint)
                            {
                                using (new VerticalScope("box"))
                                {
                                    drawProperty("Joint settings");
                                    if (_inspectionPart.hasBolts || _inspectionTriggersHasBolts)
                                    {
                                        drawPropertyBool("bolt tightness effects breakforce", ref _inspectionPart.partSettings.assemblyTypeJointSettings.boltTightnessEffectsBreakforce);
                                        if (_inspectionPart.partSettings.assemblyTypeJointSettings.boltTightnessEffectsBreakforce)
                                        {
                                            drawProperty($"Tightness Step: {_inspectionPart.tightnessStep} (Nm per bolt turn)");
                                            _inspectionPart.partSettings.assemblyTypeJointSettings.breakForceMin = drawPropertyEdit("Initial breakforce", _inspectionPart.partSettings.assemblyTypeJointSettings.breakForceMin);
                                        }
                                        if (drawPropertyEdit("tightness threshold (%)", ref _tightnessThresholdPercent))//_inspectionPart.partSettings.tightnessThreshold);
                                        {
                                            _inspectionPart.partSettings.tightnessThreshold = _tightnessThresholdPercent / 100;
                                        }
                                    }
                                    drawPropertyEdit("breakforce", ref _inspectionPart.partSettings.assemblyTypeJointSettings.breakForce);
                                }
                                
                            }
                            if (_inspectionPart.hasBolts || _inspectionTriggersHasBolts)
                            {
                                using (new VerticalScope("box"))
                                {
                                    drawProperty($"{(_inspectionPart.bolted ? "" : "Not")} Bolted");
                                    drawProperty($"Total tightness: {_inspectionPart.tightnessTotal} / {_inspectionPart.maxTightnessTotal} ({_inspectionPart.tightnessTotal / _inspectionPart.maxTightnessTotal * 100}%)");
                                    drawProperty($"Tightness threshold: {_inspectionPart.maxTightnessTotal * _inspectionPart.partSettings.tightnessThreshold} / {_inspectionPart.maxTightnessTotal} ({_inspectionPart.maxTightnessTotal * _inspectionPart.partSettings.tightnessThreshold / _inspectionPart.maxTightnessTotal * 100}%)");
                                    drawProperty("At what percent of total bolt tightness does the part disassemble logic disable.");
                                    Space(5);
                                    drawPropertyBool("Inspect Bolts", ref _inspectingBolt);
                                }
                            }
                            if (_inspectionPart.joint)
                            {
                                using (new VerticalScope("", _borderStyle))
                                {
                                    drawProperty("Joint <b>Active</b>");
                                    if (_inspectionPart.joint.breakForce == float.PositiveInfinity)
                                    {
                                        drawProperty($"Joint.breakforce: unbreakable (infinity)");
                                    }
                                    else
                                    {
                                        drawProperty($"Joint.breakforce: {_inspectionPart.joint.breakForce}Nm");
                                    }
                                    if (Button("Update joint breakforce"))
                                    {
                                        _inspectionPart.updateJointBreakForce();
                                    }
                                }
                            }
                        }
                    }
                    Space(1);
                    using (new HorizontalScope("box"))
                    {
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
                    using (_scrollViewScope = new ScrollViewScope(_triggerScroll, false, false))
                    {
                        _triggerScroll = _scrollViewScope.scrollPosition;
                        _scrollViewScope.handleScrollWheel = true;

                        
                        if (_inspectionPartTriggers != null && _inspectionPartTriggers.Length > 0)
                        {
                            for (int i = 0; i < _inspectionPartTriggers.Length; i++)
                            {
                                Trigger trigger = _inspectionPartTriggers[i];

                                if (i.isEven())
                                {
                                    GUI.skin.box.normal.background = _primaryItemBackground;
                                }
                                else
                                {
                                    GUI.skin.box.normal.background = _secondaryItemBackground;
                                }
                                using (new VerticalScope("box"))
                                {
                                    using (new HorizontalScope())
                                    {
                                        drawProperty(trigger.triggerID);

                                        if (Button("Teleport to trigger"))
                                        {
                                            getPlayer.teleport(trigger.gameObject.transform.position);
                                        }

                                        if (!trigger.callback.part)
                                        {
                                            if (!_inspectionPart.installed)
                                            {
                                                if (Button("Assemble"))
                                                {
                                                    _inspectionPart.assemble(trigger);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Label("(In Use)");
                                        }

                                        if (_inspectionPart.installed && _inspectionPart == _inspectionPartTriggers[i].callback.part)
                                        {
                                            if (Button("Disassemble"))
                                            {
                                                _inspectionPart.disassemble();
                                            }
                                        }
                                    }
                                    using (new HorizontalScope())
                                    {
                                        if (!_inspectionPartTriggers[i].settings.useTriggerTransformData)
                                        {
                                            using (new VerticalScope())
                                            {
                                                drawProperty("Pivot transform");
                                                drawPropertyVector3("Pivot position", ref _inspectionPartPivotPosition[i]);
                                                drawPropertyVector3("Pivot euler", ref _inspectionPartPivotEuler[i]);
                                            }
                                            Space(5);
                                        }
                                        using (new VerticalScope())
                                        {
                                            drawProperty("Trigger transform");
                                            drawPropertyVector3("Trigger position", ref _inspectionPartTriggerPosition[i]);
                                            drawPropertyVector3("Trigger euler", ref _inspectionPartTriggerEuler[i]);
                                            drawPropertyEdit("Trigger Radius", ref _inspectionPartTriggerRadius[i]);
                                        }
                                    }

                                    if (_inspectionPartTriggers[i].callback.part && _inspectionPartTriggers[i].callback.part.partSettings.assembleType == AssembleType.joint)
                                    {
                                        drawProperty("Change assemble type (temp) or uninstall part to apply trigger transform.");
                                        continue;
                                    }
                                    if (Button("apply"))
                                    {
                                        if (!_inspectionPartTriggers[i].settings.useTriggerTransformData)
                                        {
                                            _inspectionPartTriggers[i].partPivot.transform.localPosition = _inspectionPartPivotPosition[i];
                                            _inspectionPartTriggers[i].partPivot.transform.localEulerAngles = _inspectionPartPivotEuler[i];
                                        }
                                        else
                                        {
                                            _inspectionPartTriggers[i].partPivot.transform.localPosition = _inspectionPartTriggerPosition[i];
                                            _inspectionPartTriggers[i].partPivot.transform.localEulerAngles = _inspectionPartTriggerEuler[i];
                                        }
                                        _inspectionPartTriggers[i].gameObject.transform.localPosition = _inspectionPartTriggerPosition[i];
                                        _inspectionPartTriggers[i].gameObject.transform.localEulerAngles = _inspectionPartTriggerEuler[i];
                                        _inspectionPartTriggers[i].collider.radius = _inspectionPartTriggerRadius[i];
                                    }
                                }
                            }
                            GUI.skin.box.normal.background = _defaultBackground;
                        }
                        else
                        {
                            drawProperty("No triggers");
                        }
                    }
                }
            }
        }

        private void drawPartPickedUp() 
        {

            drawProperty($"{(_inspectionPart.inherentlyPickedUp ? "Inherently p" : "P")}icked up{_inspectionPart.pickedUp} ");

            if (_inspectionPart.pickedUp)
            {
                drawProperty("Picked up");
            }
            else if (_inspectionPart.inherentlyPickedUp)
            {
                drawProperty("Inherently picked up");
            }
            else
            {
                drawProperty("Not picked up");
            }

        }

        private void drawBoltGui()
        {
            using (new VerticalScope("box"))
            {
                using (new HorizontalScope("box"))
                {
                    using (new VerticalScope("box"))
                    {
                        string type;
                        if (inspectionBoltWithNut != null)
                        {
                            type = "BoltWithNut";
                        }
                        else
                        {
                            type = "Bolt";
                        }
                        drawProperty($"Bolt name: {inspectionBolt.callback.name} ({type})");
                        inspectionBolt.settings.type.drawProperty();
                        inspectionBolt.settings.size.drawProperty();
                        if (inspectionBoltWithNut != null)
                        {
                            drawProperty("Nut");
                            inspectionBoltWithNut.nut.settings.type.drawProperty();
                            inspectionBoltWithNut.nut.settings.size.drawProperty();
                        }
                    }
                    using (new VerticalScope("box"))
                    {
                        drawProperty($"Routine: {(inspectionBolt.routine == null ? "in" : "")}active");
                        drawProperty("Bolt tightness", inspectionBolt.tightness);
                        bool h = inspectionBolt.callback.highlighted;
                        if (Button((h ? "Stop h" : "H") + "ighlight"))
                        {
                            inspectionBolt.callback.highlight(!h);
                        }
                    }
                }
                Space(1);
                using (new VerticalScope("box"))
                {
                    drawPropertyVector3("start pos", ref inspectionPosition);
                    drawPropertyVector3("start rot", ref inspectionEuler);
                    drawPropertyVector3("pos dir", ref inspectionPosDir);
                    drawPropertyVector3("rot dir", ref inspectionRotDir);
                    drawPropertyEdit("pos step", ref inspectionPosStep);
                    drawPropertyEdit("rot step", ref inspectionRotStep);
                    if (inspectionBoltWithNut != null)
                    {
                        drawPropertyEdit("Nut offset", ref inspectionOffset);
                    }
                    if (Button("apply"))
                    {
                        inspectionBolt.startPosition = inspectionPosition;
                        inspectionBolt.startEulerAngles = inspectionEuler;
                        inspectionBolt.settings.posDirection = inspectionPosDir;
                        inspectionBolt.settings.rotDirection = inspectionRotDir;
                        inspectionBolt.settings.posStep = inspectionPosStep;
                        inspectionBolt.settings.rotStep = inspectionRotStep;

                        if (inspectionBoltWithNut != null)
                        {
                            inspectionBoltWithNut.boltWithNutSettings.offset = inspectionOffset;
                        }

                        inspectionBolt.updateModelPosition();
                    }
                }
            }
        }

        private void drawLog()
        {
            // Written, 06.05.2023

            using (new VerticalScope("box"))
            {
                using (_scrollViewScope = new ScrollViewScope(_logScroll, false, true))
                {
                    _logScroll = _scrollViewScope.scrollPosition;
                    _scrollViewScope.handleScrollWheel = true;

                    ModClient.log = TextArea(ModClient.log, ExpandWidth(true), ExpandHeight(true));
                }
            }
        }
    }
}
