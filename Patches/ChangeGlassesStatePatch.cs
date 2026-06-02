using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using RainCap.Utils;
using SPT.Reflection.Patching;

namespace RainCap.Patches
{
    internal class ChangeGlassesStatePatch() : ModulePatch
    {
        public static Player LocalPlayer => Singleton<GameWorld>.Instance?.MainPlayer;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(RainScreenDrops), nameof(RainScreenDrops.ChangeGlassesState));
        }

        [PatchPostfix]
        static void Postfix(
            ref bool ___bool_0,
            ref GClass986 ___gclass986_0
            )
        {
            bool preventRain = RainCap.preventRain.Value;
            var headwearId = LocalPlayer?.Equipment?.GetSlot(EquipmentSlot.Headwear)?.ContainedItem?.Name?.Substring(0, 24);
            bool capOn;

            try
            {
                if (!preventRain)
                {
                    if (EquipmentIDsUtils.capIds.Contains(headwearId))
                    {
                        capOn = true;
                    }
                    else
                    {
                        capOn = false;
                    }
                    ___bool_0 = capOn;
                    ___gclass986_0?.ChangeGlassState(capOn);
                }
            }
            catch (Exception ex)
            {
                RainCap.LogSource.LogError($"RainCap: Error whilst making changes to the rain drops rendering - {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
