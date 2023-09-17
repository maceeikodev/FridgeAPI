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

It is important that you reference FridgeAPI in a different method because otherwise the mod will crash if FridgeAPI wasn't present.

## How to make custom spoilable food compatible with more fridges

Making your custom spoilable food or drinks compatible with more fridges can be done easily both in code and in the inspector.

The following example makes ExampleFood compatible with FridgeAPI:
```cs

```
