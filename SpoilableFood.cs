using UnityEngine;
using MSCLoader;
using HutongGames.PlayMaker;
using System.Linq;

namespace FridgeAPI
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class SpoilableFood : MonoBehaviour
    {
        /// <summary>
        /// Determines whether the component should look for vanilla Playmaker food system and edit it
        /// </summary>
        public bool isDrivenByPlaymaker = true;
        /// <summary>
        /// Condition of the food item
        /// </summary>
        public float condition = 100f;

        /// <summary>
        /// Called when the condition drops below 0 and this component is destroyed
        /// </summary>
        public System.Action FoodSpoiled;

        Fridge currentFridge;
        /// <summary>
        /// The fridge the food is currently in (if it's disabled it's like there is no fridge, and if it's null the food is not in a fridge)
        /// </summary>
        public Fridge CurrentFridge => currentFridge;

        PlayMakerFSM foodFSM;
        FsmFloat conditionFSM;
        const string SpoiledEvent = "BAD"; // FSM event called when item is spoiled
        float spoilTime = 1f;

        void Start()
        {
            if (isDrivenByPlaymaker) editPlaymaker();
        }

        void editPlaymaker()
        {
            foodFSM = transform.GetPlayMaker("Use");
            if (foodFSM == null)
            {
                ModConsole.LogError($"The spoilable food {gameObject.name} does not have \"Use\" FSM");
                return;
            }

            // Set variable
            conditionFSM = foodFSM.GetVariable<FsmFloat>("Condition");
            if (conditionFSM == null)
            {
                ModConsole.LogError($"The spoilable food {gameObject.name} does not have \"Condition\" FSM variable");
                return;
            }
            condition = conditionFSM.Value;

            // Disable vanilla spoiling
            var waitPlayerState = foodFSM.FsmStates.FirstOrDefault(s => s.Name.ToLower() == "wait player" || s.Name.ToLower() == "wait player 2");
            if (waitPlayerState == null)
            {
                ModConsole.LogError($"The spoilable food {gameObject.name} does not have \"Wait player\" nor \"Wait player 2\" FSM state");
                return;
            }

            // The wait action triggers state that subtracts condition
            var spoilWaitAction = waitPlayerState.Actions.FirstOrDefault(a =>
            {
                var waitAction = a as HutongGames.PlayMaker.Actions.Wait;
                if (waitAction == null) return false;
                return waitAction.finishEvent.Name == "UPDATE";
            });
            if (spoilWaitAction == null)
            {
                ModConsole.LogError($"The spoilable food {gameObject.name} does not have suitable Wait FSM action");
                return;
            }
            spoilWaitAction.Enabled = false;

            // Add transition to state Bad when the item spoils
            var transitions = new FsmTransition[waitPlayerState.Transitions.Length + 1];
            waitPlayerState.Transitions.CopyTo(transitions, 0);
            transitions[transitions.Length - 1] = new FsmTransition
            {
                ToState = "Bad",
                FsmEvent = foodFSM.FsmEvents.First(e => e.Name == SpoiledEvent)
            };
            waitPlayerState.Transitions = transitions;
        }

        void OnTriggerEnter(Collider other)
        {
            Fridge fridge = null;
            if (!isFridgeTrigger(other, out fridge)) return;
            currentFridge = fridge;
        }

        void OnTriggerExit(Collider other)
        {
            if (!isFridgeTrigger(other, out _)) return;
            currentFridge = null;
        }

        bool isFridgeTrigger(Collider other, out Fridge fridge)
        {
            fridge = other.GetComponent<Fridge>();
            return fridge != null;
        }

        void Update()
        {
            if (condition < 0f)
            {
                // If item is spoiled we don't need this script anymore
                if (isDrivenByPlaymaker && foodFSM != null) foodFSM.SendEvent(SpoiledEvent);
                FoodSpoiled?.Invoke();
                Destroy(this);
                return;
            }

            // Create a 1 second timer
            if (spoilTime > 0f) spoilTime -= Time.deltaTime;
            else
            {
                spoilTime = 1f;

                // Subtract condition (fridge can be turned off if e.g. electricity is out or door is open)
                condition -= currentFridge == null || !currentFridge.enabled ? Fridge.GLOBAL_SpoilingRate : currentFridge.FridgeSpoilingRate;

                // Update playmaker
                if (isDrivenByPlaymaker && conditionFSM != null)
                {
                    conditionFSM.Value = condition;
                }
            }
        }
    }
}
