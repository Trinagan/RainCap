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
            RainScreenDropsPlacer ____dropsPlacer
            )
        {
            bool onlyFaceShields = RainCap.onlyFaceShields.Value;
            var headwearItem = LocalPlayer?.Equipment?.GetSlot(EquipmentSlot.Headwear)?.ContainedItem;
            var headwearId = headwearItem?.Name?.Substring(0, 24);

            if (EquipmentIdsUtils.capIds.Contains(headwearId) && !onlyFaceShields)
            {
                ____dropsPlacer.ChangeGlassState(true);
            }
            else
            {
                return;
            }
        }
    }
}
