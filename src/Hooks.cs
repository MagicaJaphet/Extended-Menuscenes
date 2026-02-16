using MagicaHookingLibrary.Interfaces;
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

namespace ExtendedMenuscenes;
internal class Hooks : IOwnHooks
{
	public void PreApply()
	{
	}

	public void OnApply()
	{
		On.Menu.SlugcatSelectMenu.SlugcatPage.GrafUpdate += SlugcatPage_GrafUpdate;
		On.Menu.MenuDepthIllustration.GrafUpdate += MenuDepthIllustration_GrafUpdate;
	}

	private void SlugcatPage_GrafUpdate(On.Menu.SlugcatSelectMenu.SlugcatPage.orig_GrafUpdate orig, SlugcatSelectMenu.SlugcatPage self, float timeStacker)
	{
		orig(self, timeStacker);

		foreach (var image in self.slugcatImage.depthIllustrations)
		{
			HandleColorProcessing(image);
		}
	}

	private void MenuDepthIllustration_GrafUpdate(On.Menu.MenuDepthIllustration.orig_GrafUpdate orig, Menu.MenuDepthIllustration self, float timeStacker)
	{
		orig(self, timeStacker);

		if (self.menu is not SlugcatSelectMenu)
			HandleColorProcessing(self);
	}

	private void HandleColorProcessing(MenuDepthIllustration self)
	{
		if (ColorImages.colorImagesCWT.TryGetValue(self, out var colorImage))
		{
			var slugcat = self.menu is SlugcatSelectMenu select ? select.slugcatPages[select.slugcatPageIndex].slugcatNumber : Custom.rainWorld.progression.miscProgressionData.currentlySelectedSinglePlayerSlugcat;
			var _multiplyColor = colorImage.ImageColor;
			if ((colorImage.Slugcat == null || slugcat == colorImage.Slugcat) && colorImage.ColorSlot is int slot && PlayerGraphics.DefaultBodyPartColorHex(slugcat).Count > slot)
			{
				if (ModManager.CoopAvailable)
				{
					_multiplyColor = PlayerGraphics.JollyColor(0, slot);
				}
				else if (Custom.rainWorld.progression.miscProgressionData.colorsEnabled[slugcat.value] && Custom.rainWorld.progression.miscProgressionData.colorChoices.TryGetValue(slugcat.value, out var colorDict))
				{
					if (colorDict != null && colorDict.Count > slot)
					{
						var array = colorDict[slot].Split(',');
						var hsl = new Vector3(float.Parse(array[0], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[1], NumberStyles.Any, CultureInfo.InvariantCulture), float.Parse(array[2], NumberStyles.Any, CultureInfo.InvariantCulture));

						_multiplyColor = Custom.HSL2RGB(hsl[0], hsl[1], hsl[2]);
					}
				}
				else
				{
					_multiplyColor = Custom.hexToColor(PlayerGraphics.DefaultBodyPartColorHex(slugcat)[slot]);
				}
				colorImage.ImageColor = _multiplyColor;
			}
			if (colorImage.Opacity < 1f)
			{
				_multiplyColor = Color.Lerp(colorImage.ImageColor, Color.white, 1f - colorImage.Opacity);
			}
			if (!colorImage.ColorFSprite && self.menu is SlugcatSelectMenu select2 && (select2.scroll != 0f || colorImage.Slugcat != slugcat))
			{
				_multiplyColor = Color.Lerp(Color.black, _multiplyColor, self.sprite.alpha);
			}
			if (colorImage.ColorFSprite)
			{
				self.sprite.color = _multiplyColor;
			}
			else
			{
				self.sprite.alpha = MudUtils.PackColor(_multiplyColor);
			}
		}
	}

	public void PostApply()
	{
	}
}
