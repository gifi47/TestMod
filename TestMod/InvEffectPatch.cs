using HarmonyLib;
using UnityEngine;

namespace TestMod
{
    [HarmonyPatch(typeof(InvEffect), "Apply")]
    public class InvEffectPatch
    {
        public static bool Prefix(GameObject user, GameObject target, InvBaseItem hitBy, InvEffect __instance)
        {
            if (__instance.id == (InvEffect.Identifier)((byte)126))
            {
                Experience component = target.GetComponent<Experience>();
                if (component != null)
                {
                    component.AddjustCurrentExperience((int)__instance.amount);
                    return false;
                } 
            }
            return true;
        }
    }
}
