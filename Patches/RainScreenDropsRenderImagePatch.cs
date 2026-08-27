using System;
using System.Linq;
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
            bool capsPreventRain = RainCap.capsPreventRain.Value;
            bool onlyFaceShields = RainCap.onlyFaceShields.Value;

            var headwearItem = LocalPlayer?.Equipment?.GetSlot(EquipmentSlot.Headwear)?.ContainedItem;
            var headwearId = headwearItem?.Name?.Substring(0, 24);
            var faceCoverItem = LocalPlayer?.Equipment?.GetSlot(EquipmentSlot.FaceCover)?.ContainedItem;
            var faceCoverId = faceCoverItem?.Name?.Substring(0, 24);
            var eyewearItem = LocalPlayer?.Equipment?.GetSlot(EquipmentSlot.Eyewear).ContainedItem;

            if (capsPreventRain && !onlyFaceShields)
            {
                try
                {
                    if (EquipmentIdsUtils.capIds.Contains(headwearId))
                    {
                        Graphics.Blit(source, destination);
                        return false;               
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    RainCap.LogSource.LogError($"RainCap: Error whilst disabling rain drop rendering for caps - {ex.Message}\n{ex.StackTrace}");
                    return true;
                }
            }
            else if (onlyFaceShields)
            {
                try
                {
                    if (headwearItem != null && !EquipmentIdsUtils.capIds.Contains(headwearId) && !EquipmentIdsUtils.faceMasksIds.Contains(headwearId))
                    {
                        CompoundItem item = headwearItem as CompoundItem;
                        FaceShieldComponent faceShield;
                        item.Slots.OfType<Slot>().First().ContainedItem.TryGetItemComponent<FaceShieldComponent>(out faceShield);
                        if (faceShield != null && faceShield.Mask != FaceShieldComponent.EMask.Narrow && faceShield.Togglable.On)
                        {
                            return true;
                        }
                        else
                        {
                            Graphics.Blit(source, destination);
                            return false;
                        }
                    }
                    else if (faceCoverItem != null && !EquipmentIdsUtils.faceCoversIds.Contains(faceCoverId))
                    {
                        FaceShieldComponent faceShield;
                        faceCoverItem.TryGetItemComponent<FaceShieldComponent>(out faceShield);
                        if (faceShield != null)
                        {
                            return true;
                        }
                        else
                        {
                            Graphics.Blit(source, destination);
                            return false;
                        }
                    }
                    else if (EquipmentIdsUtils.faceMasksIds.Contains(headwearId) || EquipmentIdsUtils.faceCoversIds.Contains(faceCoverId) || eyewearItem != null)
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
                    RainCap.LogSource.LogError($"RainCap: Error whilst disabling rain drop rendering for face shields - {ex.Message}\n{ex.StackTrace}");
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
