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
            ref bool ____hasGlasses,
            ref RainScreenDropsPlacer ____dropsPlacer
            )
        {
            bool capsPreventRain = RainCap.capsPreventRain.Value;
            bool onlyFaceShields = RainCap.onlyFaceShields.Value;
            var headwearItem = LocalPlayer?.Equipment?.GetSlot(EquipmentSlot.Headwear)?.ContainedItem;
            var headwearId = headwearItem?.Name?.Substring(0, 24);
            bool capOn;

            try
            {
                if (!capsPreventRain)
                {
                    if (EquipmentIdsUtils.capIds.Contains(headwearId))
                    {
                        capOn = true;
                    }
                    else
                    {
                        capOn = false;
                    }
                    ____hasGlasses = capOn;
                    ____dropsPlacer?.ChangeGlassState(capOn);
                }
            }
            catch (Exception ex)
            {
                RainCap.LogSource.LogError($"RainCap: Error whilst making changes to the rain drops rendering for caps - {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
