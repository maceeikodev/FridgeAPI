using MSCLoader;
using UnityEngine;
using System.Linq;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

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
        public override string Description => "Allows other mods to add another fridges and allows customization of spoiling rates." +
            " Check the github page for documentation: https://github.com/maceeikodev/FridgeAPI"; //Short description of your mod

        public override byte[] Icon => Properties.Resources.fridgeicon;

        SettingsSlider slider_globalSpoilingRate, slider_globalSpoilingRateFridge, slider_globalSpoilingRateFreezer;

        public override void ModSetup()
        {
            SetupFunction(Setup.PreLoad, Mod_PreLoad);
        }

        public override void ModSettings()
        {
            slider_globalSpoilingRate = Settings.AddSlider(this, "default_spoil_rate", "Global spoiling rate outside of fridges", 0f, 0.1f,
                .034f, () => { Fridge.GLOBAL_SpoilingRate = slider_globalSpoilingRate.GetValue(); }, 4);
            slider_globalSpoilingRateFridge = Settings.AddSlider(this, "fridge_spoil_rate", "Global spoiling rate inside of fridges", 0f, 0.002f,
                .0005f, () => { Fridge.GLOBAL_SpoilingRateFridge = slider_globalSpoilingRateFridge.GetValue(); }, 4);
            slider_globalSpoilingRateFreezer = Settings.AddSlider(this, "freezer_spoil_rate", "Global spoiling rate inside of freezers", 0f, 0.001f,
                .0001f, () => { Fridge.GLOBAL_SpoilingRateFreezer = slider_globalSpoilingRateFreezer.GetValue(); }, 4);
        }

        private void Mod_PreLoad()
        {
            // Make vanilla fridge point
            var vanillaFridge = GameObject.Find("YARD").transform.Find("Building/KITCHEN/Fridge");
            var fridgePoint = new GameObject("FridgeAPI Vanilla Fridge");
            fridgePoint.transform.SetParent(vanillaFridge, false);
            fridgePoint.transform.localPosition = new Vector3(.3f, -.1f, .4f);
            fridgePoint.transform.localScale = new Vector3(.6f, .6f, 1f);
            fridgePoint.AddComponent<BoxCollider>().isTrigger = true;
            var vanillaFridgeBehaviour = fridgePoint.AddComponent<Fridge>();
            vanillaFridgeBehaviour.fridgeType = FridgeType.Fridge;

            // Fix vanilla fridge toggling
            var fridgePointFSM = vanillaFridge.Find("FridgePoint").GetPlayMaker("Chilling");
            var fridgeOwner = new FsmOwnerDefault
            {
                OwnerOption = OwnerDefaultOption.SpecifyGameObject,
                GameObject = fridgePoint
            };
            var fridgeOnState = fridgePointFSM.FsmStates[2];
            fridgeOnState.Actions[0] = new HutongGames.PlayMaker.Actions.EnableBehaviour
            {
                gameObject = fridgeOwner,
                component = vanillaFridgeBehaviour,
                enable = true,
                resetOnExit = false
            };
            var fridgeOffState = fridgePointFSM.FsmStates[3];
            fridgeOffState.Actions[0] = new HutongGames.PlayMaker.Actions.EnableBehaviour
            {
                gameObject = fridgeOwner,
                component = vanillaFridgeBehaviour,
                enable = false,
                resetOnExit = false
            };

            // Edit food prefabs
            // Spawner has to be found through an FSM because it's disabled in PreLoad
            var setupGame = GameObject.Find("Systems/Setup Game").GetComponent<PlayMakerFSM>();
            var spawner = ((ActivateGameObject)setupGame.FsmStates[0].Actions[2]).gameObject.GameObject.Value.transform.Find("CreateItems");
            var fsms = spawner.GetComponents<PlayMakerFSM>();
            foreach (var fsm in fsms)
            {
                // If this item doesn't have condition, skip
                if (fsm.FsmVariables.FloatVariables.FirstOrDefault(f => f.Name == "Condition") == null) continue;

                // Get the prefab
                var prefab = fsm.GetVariable<FsmGameObject>("CreatePrefab").Value;
                prefab.AddComponent<SpoilableFood>();
            }
        }
    }
}
