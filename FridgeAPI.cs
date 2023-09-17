using MSCLoader;
using UnityEngine;

namespace FridgeAPI
{
    public class FridgeAPI : Mod
    {
        public override string ID => "FridgeAPI"; //Your mod ID (unique)
        public override string Name => "Fridge API"; //You mod name
        public override string Author => "Maceeiko"; //Your Username

        readonly string version = "1.0";
#if DEBUG
        public override string Version => version + "-debug"; //Version
#else
        public override string Version => version; //Version
#endif
        public override string Description => "Allows other mods to add another fridges and allows customization of spoiling rates."; //Short description of your mod

        public override void ModSetup()
        {
            SetupFunction(Setup.OnLoad, Mod_OnLoad);
        }

        public override void ModSettings()
        {
            // All settings should be created here. 
            // DO NOT put anything else here that settings or keybinds
        }

        private void Mod_OnLoad()
        {
            // Called once, when mod is loading after game is fully loaded
        }
    }
}
