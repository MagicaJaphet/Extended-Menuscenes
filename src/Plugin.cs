using BepInEx;
using BepInEx.Logging;
using MagicaHookingLibrary.Helpers;
using System.Security.Permissions;
using System.Runtime.CompilerServices;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using MagicaHookingLibrary;
using System.IO;
using UnityEngine;
using RWCustom;
using System;

// Allows access to private members
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618

namespace ExtendedMenuscenes;

[BepInDependency("magica.hookinglibrary", BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin("magica.extendedmenuscenes", "Extended Menuscenes", "1.0.0")]
sealed class Plugin : PluginTemplate
{
	public static new ManualLogSource Logger;

	public override void OnEnable()
	{
		base.OnEnable();
		Logger = base.Logger;
	}

    public override void PreModsInit(RainWorld self)
    {
        HookHelpers.ApplyHooks(HookHelpers.HookType.Pre, Logger);
    }

    public override void OnModsInit(RainWorld self)
	{
		HookHelpers.ApplyHooks(HookHelpers.HookType.On, Logger);
		Assets.Initialize();
	}

    public override void PostModsInit(RainWorld self)
    {
        HookHelpers.ApplyHooks(HookHelpers.HookType.Post, Logger);
    }
}

/// <summary>
/// Code and shaders provided by Haizlbliek.
/// </summary>
public static class Assets
{
	public static readonly string[] shaders = ["ColSceneBlur", "ColSceneBlurLightEdges", "ColSceneLighten", "ColSceneMultiply", "ColSceneOverlay", "ColSceneSoftLight"];

	public static void Initialize()
	{
		string bundlePath = AssetManager.ResolveFilePath(Path.Combine("assetbundles", "colscene"));
		if (!File.Exists(bundlePath)) return;

		AssetBundle assetBundle = null;

		try
		{
			assetBundle = AssetBundle.LoadFromFile(bundlePath);
			foreach (string shader in shaders)
			{
				Custom.rainWorld.Shaders[shader] = FShader.CreateShader(shader, assetBundle.LoadAsset<Shader>("Assets/Shaders/colscene/" + shader + ".shader"));
				Plugin.Logger.LogInfo($"Registered shader {shader}!");
			}
		}
		catch (Exception ex)
		{
			Plugin.Logger.LogError(ex);
		}
		finally
		{
			assetBundle?.Unload(false);
		}
	}

	public static void Cleanup()
	{
		foreach (string shader in shaders)
		{
			Custom.rainWorld.Shaders.Remove(shader);
		}
	}
}