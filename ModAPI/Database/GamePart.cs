using System;
using UnityEngine;
using HutongGames.PlayMaker;

namespace TommoJProductions.ModApi.Database.GameParts
{
    public class GamePart
    {
        public FsmBool bolted { get; set; }
        public FsmBool installed { get; set; }
        public FsmBool damaged { get; set; }
        public FsmGameObject thisPart { get; set; }
        public FsmGameObject newMesh { get; set; }
        public FsmGameObject damagedMesh { get; set; }
        public FsmGameObject trigger { get; set; }

        public GamePart(PlayMakerFSM data)
        {
            if (!data)
                throw new NullReferenceException("[GamePart Constructor] param, 'data' cannot be null.]");

            bolted = data.FsmVariables.GetFsmBool("Bolted");
            installed = data.FsmVariables.GetFsmBool("Installed");
            damaged = data.FsmVariables.GetFsmBool("Damaged");
            thisPart = data.FsmVariables.GetFsmGameObject("ThisPart");
            newMesh = data.FsmVariables.GetFsmGameObject("New");
            damagedMesh = data.FsmVariables.GetFsmGameObject("Damaged");
            trigger = data.FsmVariables.GetFsmGameObject("Trigger");
        }


        public static implicit operator GameObject(GamePart gp) => gp.thisPart.Value;
    }
    public class GamePartTime : GamePart
    {
        public FsmFloat time { get; set; }

        public GamePartTime(PlayMakerFSM data) : base(data)
        {
            time = data.FsmVariables.GetFsmFloat("Time");
        }
    }
    public class GamePartWear : GamePartTime
    {
        public FsmFloat wear { get; set; }

        public GamePartWear(PlayMakerFSM data) : base(data)
        {
            wear = data.FsmVariables.GetFsmFloat("Wear");
        }
    }

    public class Block : GamePart
    {
        public FsmBool inHoist { get; set; }

        public Block(PlayMakerFSM data) : base(data)
        {
            inHoist = data.FsmVariables.GetFsmBool("InHoist");
        }
    }
    public class OilPan : GamePart
    {
        public FsmFloat oilLevel { get; set; }
        public FsmFloat oilContamination { get; set; }
        public FsmFloat oilGrade { get; set; }

        public OilPan(PlayMakerFSM data) : base(data)
        {
            oilLevel = data.FsmVariables.GetFsmFloat("Oil");
            oilContamination = data.FsmVariables.GetFsmFloat("OilContamination");
            oilGrade = data.FsmVariables.GetFsmFloat("OilGrade");
        }
    }
    public class RockerShaft : GamePart
    {
        public FsmFloat maxExhaust { get; set; }
        public FsmFloat maxIntake { get; set; }
        public FsmFloat minExhaust { get; set; }
        public FsmFloat minIntake { get; set; }
        public FsmFloat settingExhaustMax { get; set; }
        public FsmFloat settingExhaustMin { get; set; }
        public FsmFloat settingIntakeMax { get; set; }
        public FsmFloat settingIntakeMin { get; set; }
        public FsmFloat cyl1Ex { get; set; }
        public FsmFloat cyl1In { get; set; }
        public FsmFloat cyl2Ex { get; set; }
        public FsmFloat cyl2In { get; set; }
        public FsmFloat cyl3Ex { get; set; }
        public FsmFloat cyl3In { get; set; }
        public FsmFloat cyl4Ex { get; set; }
        public FsmFloat cyl4In { get; set; }

        public RockerShaft(PlayMakerFSM data) : base(data)
        {
            maxExhaust = data.FsmVariables.GetFsmFloat("MaxExhaust");
            maxIntake = data.FsmVariables.GetFsmFloat("MaxIntake");
            minExhaust = data.FsmVariables.GetFsmFloat("MinExhaust");
            minIntake = data.FsmVariables.GetFsmFloat("MinIntake");
        }
    }
    public class Carburator : GamePart
    {
        public FsmFloat dirt { get; set; }
        public FsmFloat idleAdjust { get; set; }
        public FsmFloat adjustMax { get; set; }
        public FsmFloat adjustMin { get; set; }

        public Carburator(PlayMakerFSM data) : base(data)
        {
            dirt = data.FsmVariables.GetFsmFloat("Dirt");
            idleAdjust = data.FsmVariables.GetFsmFloat("IdleAdjust");
            adjustMax = data.FsmVariables.GetFsmFloat("Max");
            adjustMin = data.FsmVariables.GetFsmFloat("Min");
        }
    }
    public class Distributor : GamePart
    {
        public FsmFloat maxAngle { get; set; }
        public FsmFloat sparkAngle { get; set; }

        public Distributor(PlayMakerFSM data) : base(data)
        {
            maxAngle = data.FsmVariables.GetFsmFloat("MaxAngle");
            sparkAngle = data.FsmVariables.GetFsmFloat("SparkAngle");
        }
    }
    public class CamshaftGear : GamePart
    {
        public FsmFloat angle { get; set; }
        public FsmInt _int { get; set; }

        public CamshaftGear(PlayMakerFSM data) : base(data)
        {
            angle = data.FsmVariables.GetFsmFloat("Angle");
            _int = data.FsmVariables.GetFsmInt("Int");
        }
    }
}
