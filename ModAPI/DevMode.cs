using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TommoJProductions.ModApi.Attachable;
using TommoJProductions.ModApi.Attachable.CallBacks;
using UnityEngine;
using static TommoJProductions.ModApi.ModClient;
using static UnityEngine.GUILayout;

namespace TommoJProductions.ModApi
{
    public class DevMode : MonoBehaviour
    {
        internal static Part inspectionPart;
        private bool inspectionPartSet = false;
        private Vector3[] inspectionPartPosition;
        private Vector3[] inspectionPartEuler;
        private bool inspectingBolt = false;
        private bool inspectionBoltSet = false;
        internal static Bolt inspectionBolt;
        internal static Vector3 inspectionPosition;
        internal static Vector3 inspectionEuler;
        internal static float inspectionOffset;

        private int top = 75;
        private int margin = 20;
        private int height => Screen.height - top;
        private int width = 500;
        private int left => Screen.width - width - margin;
        private Vector2 scroll;
        private ScrollViewScope scrollViewScope;
        private Color c1 = new Color32(0, 178, 230, 176);
        private Color c2 = new Color32(0, 62, 230, 153);
        private GUIStyle style = new GUIStyle();

        void Start() 
        {
            StartCoroutine(devModeFunc());
        }
        void OnGUI() 
        {
            // Written, 03.07.2022

            if (inspectionPart)
            {
                Transform t;
                int tl;
                
                tl = inspectionPart.triggers.Length;

                if (!inspectionPartSet)
                {
                    inspectionPartPosition = new Vector3[tl];
                    inspectionPartEuler = new Vector3[tl];
                    for (int i = 0; i < tl; i++)
                    {
                        t = inspectionPart.triggers[i].triggerGameObject.transform;
                        inspectionPartPosition[i] = t.localPosition;
                        inspectionPartEuler[i] = t.localEulerAngles;
                    }
                    inspectionPartSet = true;
                }
                drawToolStatsGui();
                drawPartGui();
                if (inspectionPart.hasBolts)
                {
                    if (inspectingBolt)
                    {
                        if (!inspectionBoltSet)
                        {
                            if (inspectionBolt == null)
                                inspectionBolt = inspectionPart.bolts[0];
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
                                style.border = new RectOffset(0, 0, 0, 0);
                                style.alignment = TextAnchor.MiddleCenter;
                                style.fontSize = 14;
                                for (int i = 0; i < inspectionPart.bolts.Length; i++)
                                {
                                    bool menuCheck = inspectionPart.bolts[i] == inspectionBolt;
                                    style.normal.textColor = menuCheck ? c1 : Color.white;

                                    if (Button(inspectionPart.bolts[i].boltModel.name, style))
                                    {
                                        inspectionBolt = inspectionPart.bolts[i];
                                        inspectionBoltSet = false;
                                    }
                                }
                            }
                            drawBoltGui();
                        }
                    }
                }
            }
            else if (inspectionPartSet)
            {
                inspectionPartSet = false;
            }
        } 

        /// <summary>
        /// if <see cref="devModeBehaviour"/> is true, this enumerator will poll for (CTRL+P) raycating for parts. allows raycast for parts. and assigns inspection VAR with out behaviour or null.
        /// </summary>
        public IEnumerator devModeFunc()
        {
            ModClient.print("Dev mode started");
            
            while (devMode)
            {
                if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.P))
                {
                    inspectionPart = raycastForBehaviour<Part>();
                }
                yield return null;
            }
            ModClient.print("Dev mode ended");
        }
        
        private void drawToolStatsGui()
        {
            getToolWrenchSize_boltSize.drawProperty();
            drawProperty("tool size", getToolWrenchSize_float);
            drawProperty("bolting speed", getBoltingSpeed);
        }
        private void drawDevGui()
        {

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
                            drawProperty("Part Inspection", name);
                            Space(1);
                            drawProperty($"Trigger routine: {(inspectionPart.triggerRoutine == null ? "in" : "")}active");
                            drawProperty("Position", inspectionPart.transform.position);
                            drawProperty("Euler", inspectionPart.transform.eulerAngles);
                            drawProperty("Installed", inspectionPart.installed);
                            drawProperty($"Picked up: {inspectionPart.pickedUp} | {(inspectionPart.inherentlyPickedUp ? "inherently" : "")}");
                            drawProperty("InTrigger", inspectionPart.inTrigger);
                            drawProperty("Install point index", inspectionPart.installPointIndex);
                            inspectionPart.partSettings.drawProperty("assembleType");
                        }
                        using (new VerticalScope())
                        {
                            if (inspectionPart.partSettings.assembleType == Part.AssembleType.joint)
                            {
                                using (new VerticalScope("box"))
                                {
                                    drawProperty("Joint settings");
                                    if (inspectionPart.hasBolts)
                                    {
                                        drawPropertyBool("bolt tightness effects breakforce", ref inspectionPart.partSettings.assemblyTypeJointSettings.boltTightnessEffectsBreakforce);
                                        if (inspectionPart.partSettings.assemblyTypeJointSettings.boltTightnessEffectsBreakforce)
                                        {
                                            drawPropertyEdit("breakforce min", ref inspectionPart.partSettings.assemblyTypeJointSettings.breakForceMin);
                                        }
                                        drawPropertyEdit("tightness threshold", ref inspectionPart.partSettings.tightnessThreshold);
                                    }
                                    drawPropertyEdit("breakforce", ref inspectionPart.partSettings.assemblyTypeJointSettings.breakForce);
                                }
                            }
                            if (inspectionPart.joint)
                            {
                                using (new VerticalScope("box"))
                                {
                                    drawProperty("breakforce", inspectionPart.joint.breakForce);
                                    if (inspectionPart.hasBolts)
                                    {
                                        float bf = inspectionPart.partSettings.assemblyTypeJointSettings.breakForce;
                                        float thresholdObsolute = bf * inspectionPart.partSettings.tightnessThreshold;
                                        drawProperty($"\t- Threshold: {thresholdObsolute}Nm ({thresholdObsolute / bf * 100})");
                                        drawProperty("");
                                    }
                                }
                            }
                            if (inspectionPart.hasBolts)
                            {
                                using (new VerticalScope("box"))
                                {
                                    drawProperty($"{(inspectionPart.bolted ? "" : "Not")} Bolted");
                                    drawProperty($"Tightness: {inspectionPart.tightnessTotal} / {inspectionPart.maxTightnessTotal} ({inspectionPart.tightnessTotal / inspectionPart.maxTightnessTotal * 100})");
                                    drawProperty($"Tightness Step: {inspectionPart.tightnessStep}");
                                    drawPropertyBool("Inspect Bolts", ref inspectingBolt);
                                }
                            }
                        }
                    }
                    Space(1);
                    using (new HorizontalScope("box"))
                    {
                        if (inspectionPart.hasBolts && Button((inspectionPart.boltParent.activeInHierarchy ? "Deactivate" : "Activate") + " bolts"))
                        {
                            inspectionPart.boltParent.SetActive(!inspectionPart.boltParent.activeInHierarchy);
                        }
                        if (Button("Teleport to player"))
                        {
                            inspectionPart.teleport(true, ModClient.getPOV.transform.position);
                        }
                        if (Button("Teleport to part"))
                        {
                            ModClient.getPOV.transform.root.gameObject.teleport(gameObject.transform.position);
                        }
                    }
                    Space(1);
                    drawProperty("Triggers");
                    using (scrollViewScope = new ScrollViewScope(scroll, false, false))
                    {
                        scroll = scrollViewScope.scrollPosition;
                        scrollViewScope.handleScrollWheel = true;

                        for (int i = 0; i < inspectionPart.triggers.Length; i++)
                        {
                            using (new VerticalScope("box"))
                            {
                                using (new HorizontalScope())
                                {
                                    drawProperty(inspectionPart.triggers[i].triggerGameObject.name);
                                    if (Button("Teleport to trigger"))
                                    {
                                        ModClient.getPOV.transform.root.gameObject.teleport(inspectionPart.triggers[i].triggerGameObject.transform.position);
                                    }
                                }
                                drawPropertyVector3("position", ref inspectionPartPosition[i]);
                                drawPropertyVector3("euler", ref inspectionPartEuler[i]);

                                if (Button("apply"))
                                {
                                    inspectionPart.triggers[i].partPivot.transform.localPosition = inspectionPartPosition[i];
                                    inspectionPart.triggers[i].partPivot.transform.localEulerAngles = inspectionPartEuler[i];
                                }
                            }
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
                    drawProperty("Bolt Inspection", name);
                    Space(1);
                    drawProperty($"routine: {(inspectionBolt.boltRoutine == null ? "in" : "")}active");
                    inspectionBolt.boltSettings.drawProperty("boltType");
                    drawProperty("Bolt tightness", inspectionBolt.loadedSaveInfo.boltTightness);
                    inspectionBolt.boltSettings.drawProperty("boltSize");
                    if (inspectionBolt.boltSettings.addNut)
                    {
                        drawProperty("Nut tightness", inspectionBolt.loadedSaveInfo.addNutTightness);
                        inspectionBolt.boltSettings.addNutSettings.drawProperty("nutSize");
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
                        inspectionBolt.updateNutPosRot();
                    }
                }
                Space(1);
                using (new VerticalScope("box"))
                {
                    drawPropertyVector3("pos direction", ref inspectionBolt.boltSettings.posDirection);
                    drawPropertyVector3("rot direction", ref inspectionBolt.boltSettings.rotDirection);
                    drawPropertyEdit("pos step", ref inspectionBolt.boltSettings.posStep);
                    drawPropertyEdit("rot step", ref inspectionBolt.boltSettings.rotStep);
                    drawPropertyEdit("tightness step", ref inspectionBolt.boltSettings.tightnessStep);
                }
                Space(1);
                using (new HorizontalScope("box"))
                {
                    if (Button((inspectionPart.boltParent.activeInHierarchy ? "Deactivate" : "Activate") + " bolts"))
                        inspectionPart.boltParent.SetActive(!inspectionPart.boltParent.activeInHierarchy);

                    if (Button("Teleport to bolt"))
                    {
                        ModClient.getPOV.transform.root.gameObject.teleport(gameObject.transform.position);
                    }
                }
            }
        }
    }
}
