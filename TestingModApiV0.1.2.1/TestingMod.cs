using System.Linq;
using UnityEngine;
using MSCLoader;
using TommoJProductions.ModApi;

namespace TestingModApiV0._1._2._1
{
    public class TestingMod : Mod
    {
        public override string ID => "testingmodapi";

        public override string Version => "n/a";

        public override string Author => "tommojphillips";

        public override bool LoadInMenu => true;

        public override void OnMenuLoad()
        {
            
        }

        public override void OnLoad()
        {

        }
    }
}
