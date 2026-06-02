using System.Reflection;
using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using RainCap.Utils;
using SPT.Reflection.Patching;

namespace RainCap.Patches
{
    internal class RainScreenDropsInitPatch : ModulePatch
    {
        public static Player LocalPlayer => Singleton<GameWorld>.Instance?.MainPlayer;
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(RainScreenDrops), nameof(RainScreenDrops.Init));
        }

        [PatchPostfix]
        static void Postfix(
            GClass986 ___gclass986_0
            )
        {
            var headwearId = LocalPlayer?.Equipment?.GetSlot(EquipmentSlot.Headwear)?.ContainedItem?.Name?.Substring(0, 24);

            if (EquipmentIDsUtils.capIds.Contains(headwearId))
            {
                ___gclass986_0.ChangeGlassState(true);
            }
            else
            {
                ___gclass986_0.ChangeGlassState(false);
            }
        }
    }
}
