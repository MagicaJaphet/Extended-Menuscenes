using Menu;
using RWCustom;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Menu.MenuDepthIllustration;

namespace ExtendedMenuscenes;
public static class ColorImages
{
	internal static ConditionalWeakTable<MenuDepthIllustration, ColorDepthIllustration> colorImagesCWT = new();

	private static readonly HashSet<MenuShader> defaultDepthShaders = [
			MenuShader.LightEdges,
			MenuShader.Lighten,
			MenuShader.Multiply,
			MenuShader.Normal,
			MenuShader.Overlay,
			MenuShader.SoftLight
		];

	public static void ColorImage(this MenuDepthIllustration menuDepth, Color color, float opacity = 1f, int? slotIndex = null, SlugcatStats.Name name = null)
	{
		var cwt = colorImagesCWT.GetOrCreateValue(menuDepth);
		cwt.ImageColor = color;
		cwt.ColorSlot = slotIndex;
		cwt.Slugcat = name;
		cwt.Opacity = opacity;
		if (defaultDepthShaders.Contains(menuDepth.shader))
		{
			menuDepth.sprite.shader = Custom.rainWorld.Shaders[$"Col{menuDepth.sprite.shader.name}"];
		}
		else
		{
			cwt.ColorFSprite = true;
		}
	}

	public class ColorDepthIllustration()
	{
		public int? ColorSlot { get; internal set; }
		public Color ImageColor { get; internal set; }
		public SlugcatStats.Name Slugcat { get; internal set; }
		public bool ColorFSprite { get; internal set; }
		public float Opacity { get; internal set; }
	}
}
