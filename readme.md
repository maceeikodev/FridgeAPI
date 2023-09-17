# What is FridgeAPI

FridgeAPI is a mod for My Summer Car allowing to change default food spoiling rates and most importantly, allowing other mods to create unlimited amount of fridges easily.

## How to install FridgeAPI

FridgeAPI is NOT a Reference, it is a mod, so it is installed like any other mod. Download the DLL file and put it in the mods folder.

# How to use FridgeAPI as a developer

If you are NOT making FridgeAPI as a requirement (not recommended), remember to check if FridgeAPI is present before running any code referencing FridgeAPI.
You can do so using the following code snippet:
```cs
...

if (ModLoader.IsModPresent("FridgeAPI"))
{
    load_fridgeAPI();
}

...

private void load_fridgeAPI()
{
    // Reference FridgeAPI here
}
```

It is important that you reference FridgeAPI in a different method because otherwise the mod will crash if FridgeAPI isn't present.

## How to make custom spoilable food compatible with more fridges

Making your custom spoilable food or drinks compatible with more fridges can be done easily.

The following example makes ExampleFood compatible with FridgeAPI:
```cs
var exampleFood = GameObject.Find("Example");
var spoilableFood = exampleFood.AddComponent<SpoilableFood>();
// It's important that you set this field to false if you are not using a clone of vanilla food FSM
spoilableFood.isDrivenByPlaymaker = false;
// If you are using custom food monobehaviour, then link it to this condition field
// This condition field is lowered automatically.
spoilableFood.condition = 100f;
// If the condition goes below zero SpoilableFood component is destroyed and this event is called
spoilableFood.FoodSpoiled += ExampleFood_Spoiled;
```

## How to add new fridge point

Adding custom fridge point is not any harder. The preferred way to do it is through the inspector, but it can be done through code as well.

### Example of creating fridge point in code:
```cs
// Create a new game object
var fridgePoint = new GameObject("Freezer");
// Parent it under the fridge
fridgePoint.transform.SetParent(fridge, false);
fridgePoint.transform.localPosition = new Vector3(.3f, -.516f, .1f);
fridgePoint.transform.localScale = new Vector3(.6f, .9f, .6f);
// Give it a trigger collider
fridgePoint.AddComponent<BoxCollider>().isTrigger = true;
// Add the component
var fridgeBehaviour = fridgePoint.AddComponent<Fridge>();
// Set the type
fridgeBehaviour.fridgeType = FridgeType.Freezer;
// Use this field to set custom spoiling rate when setting fridge type to FridgeType.Custom
// fridgeBehaviour.customSpoilingRate = 0.0002f;
```


### Example of creating fridge point in the editor:

![Creating the area of the freezer](https://cdn.discordapp.com/attachments/802882025325330482/1152983340736069662/image.png)

Create a collider of your choice, mark it trigger and set correct size and position.

Then add Fridge component and set the desired fridge type and custom spoiling rate if you use custom fridge type.

Your inspector should look something like this:

![Inspector of freezer area](https://cdn.discordapp.com/attachments/802882025325330482/1152984253534056458/image.png)

There are 3 fridge types you can pick from. They are as following:
```cs
public enum FridgeType
{
    Fridge, // Uses global fridge spoiling rate
    Freezer, // Uses global freezer spoiling rate
    Custom // Uses custom spoiling rate specified in the component
}
```

The mentioned global spoiling rates are static fields in Fridge class:
```cs
/// <summary>
/// Determines the global spoiling rate for all food items that don't use custom spoiling rate
/// </summary>
public static float GLOBAL_SpoilingRate = 0.034f, GLOBAL_SpoilingRateFridge = 0.0005f, GLOBAL_SpoilingRateFreezer = 0.0001f;
```
They can be edited, but doing so will affect all food items and all fridges in game. They are editable in the mod settings. 
