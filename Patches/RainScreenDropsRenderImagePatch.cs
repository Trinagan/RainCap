using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using RainCap.Utils;
using SPT.Reflection.Patching;
using UnityEngine;

namespace RainCap.Patches
{
    internal class RainScreenDropsRenderImagePatch : ModulePatch
    {
        public static Player LocalPlayer => Singleton<GameWorld>.Instance?.MainPlayer;
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(RainScreenDrops), nameof(RainScreenDrops.OnRenderImage));
        }

        [PatchPrefix]
        static bool Prefix(
            RenderTexture source,
            RenderTexture destination
            )
        {
            bool preventRain = RainCap.preventRain.Value;
            var headwearId = LocalPlayer?.Equipment?.GetSlot(EquipmentSlot.Headwear)?.ContainedItem?.Name?.Substring(0, 24);

            if (preventRain)
            {
                try
                {
                    if (!EquipmentIDsUtils.capIds.Contains(headwearId))
                    {
                        return true;
                    }
                    else
                    {
                        Graphics.Blit(source, destination);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    RainCap.LogSource.LogError($"RainCap: Error whilst disabling rain drop rendering - {ex.Message}\n{ex.StackTrace}");
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
    }
}
