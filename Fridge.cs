using UnityEngine;
using MSCLoader;
using HutongGames.PlayMaker;

namespace FridgeAPI
{
    [RequireComponent(typeof(Collider))]
    public class Fridge : MonoBehaviour
    {
        /// <summary>
        /// Determines the global spoiling rate for all food items that don't use custom spoiling rate
        /// </summary>
        public static float GLOBAL_SpoilingRate = 0.034f, GLOBAL_SpoilingRateFridge = 0.0005f, GLOBAL_SpoilingRateFreezer = 0.0001f;

        /// <summary>
        /// Spoiling rate to use when fridge type is set to custom
        /// </summary>
        [Header("This property is only used when the fridge type \"Custom\" is selected")]
        public float customSpoilingRate = 0f;

        /// <summary>
        /// Determines which spoiling rate to use
        /// </summary>
        public FridgeType fridgeType = FridgeType.Fridge;

        /// <summary>
        /// Returns the correct spoiling rate
        /// </summary>
        public float FridgeSpoilingRate
        {
            get
            {
                switch (fridgeType)
                {
                    case FridgeType.Fridge:
                        return GLOBAL_SpoilingRateFridge;
                    case FridgeType.Freezer:
                        return GLOBAL_SpoilingRateFreezer;
                    default:
                        return customSpoilingRate;
                }
            }
        }
    }

    public enum FridgeType
    {
        Fridge, Freezer, Custom
    }
}
