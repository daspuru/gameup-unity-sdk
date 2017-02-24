GameUp Unity SDK
================

The Unity SDK for the Heroic Labs (formerly GameUp) service.

[SDK Guide](https://saas.heroiclabs.com/docs/guide/unity/) | [SDK Reference](http://gameup-io.github.io/gameup-unity-sdk/annotated.html)

---

Heroic Labs is AWS for game developers. Easily add social, multiplayer, and competitive features to any kind of game. The platform handles all server infrastructure required to power games built for desktop, mobile, browser, or web. The goal is to help game studios build beautiful social and multiplayer games which work at massive scale.

For a full list of the API have a look at the [features](https://saas.heroiclabs.com/features).

### Install

To install the DLL in your project go into the Unity editor menu and select "Assets -> Import New Asset" and select the "bin/Release/GameUpSDK.dll" file. This will place the DLL in your Unity project's Assets folder.

Once the SDK is installed create a new GameObject in your game and attach a new C# script component to it. Then add the following import:

```csharp
using GameUp;
```

Copy and paste this code in your script, for example, in the `Start` method:

```csharp
Client.ApiKey = "1fb234d5678948199cb858ab0905f657";

Client.Ping ((status, reason) => {
  Debug.LogFormat ("Could not connect to API got '{0}' with '{1}'.", status, reason);
});
```

The API key shown above __must be replaced__ with one of your own from the [Developer Dashboard](https://dashboard.heroiclabs.com/). Run your game. A request will be sent to the Game API which will verify your API key is valid and the service is reachable.

### SDK Guide

You can find the full guide for the Unity SDK [online](https://saas.heroiclabs.com/docs/guide/unity/).

### Contribute

To develop on the codebase you'll need:

* [Unity](https://unity3d.com/get-unity) The Unity Editor and MonoDevelop.

#### Setup

1. Open the `GameUpSDK.sln` in MonoDevelop. On the menu bar, `Build` -> `Rebuild All`.
2. Open Unity Editor and choose to open an existing project - choose `GameUpTestScene` folder.
3. Once open, in the project gutter, double click on `GameUpTestScene.unity`. This should open our test scene with a cube in the middle of the scene.
4. Click on the `Console` tab. Click on the `Play` button in the middle-top of the editor.

All contributions to the documentation and the codebase are very welcome and feel free to open issues on the tracker wherever the documentation needs enhancements.

Pull requests are always welcome! `:)`
