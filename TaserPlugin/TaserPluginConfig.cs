using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rocket.API;

namespace TaserPlugin
{
    public class TaserPluginConfig : IRocketPluginConfiguration
    {
        public string TaserPerm;
        public uint Taser;
        public float TaseDuration;
        public bool Debug;


        public void LoadDefaults()
        {
            TaserPerm = "TaserPlugin.Tase";
            Taser = 51200;
            TaseDuration = 5;
            Debug = true;

        }
    }
}
