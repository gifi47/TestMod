using com.mindblocks.i18n;
using HarmonyLib;
using System.Text;

namespace TestMod
{
    [HarmonyPatch(typeof(InvGameItem), "get_itemDesc")]  // Targets the getter
    public class InvGameItemPatch
    {
        static bool Prefix(InvGameItem __instance, ref string __result)  // __result stores the return value
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Length = 0;
            stringBuilder.Append(T.Items(__instance.name, new object[0]));
            bool flag = __instance.isRYBLight;
            if (__instance.baseItem.stats.Count > 0 || __instance.baseItem.effects.Count > 0 || flag)
            {
                stringBuilder.Append("\n\n[FFFF00]");
                stringBuilder.Append(T.ItemEffects("Effects", new object[0]));
                stringBuilder.Append("[-]\n");
            }
            foreach (InvStat invStat in __instance.baseItem.stats)
            {
                if (invStat.id != InvStat.Identifier.BlueLight && invStat.id != InvStat.Identifier.GreenLight && invStat.id != InvStat.Identifier.RedLight)
                {
                    stringBuilder.Append("\n");
                    stringBuilder.Append((invStat.amount < 0f) ? "[FF0000]" : "[00FF00]");
                    stringBuilder.Append(T.ItemEffects(invStat.id.ToString(), new object[]
                    {
                        (invStat.modifier != InvStat.Modifier.Added) ? Helper.FormatPercent(invStat.amount, 0, true) : Helper.FormatValue(invStat.amount, 2, true)
                    }));
                    stringBuilder.Append("[-]");
                }
                else
                {
                    flag = true;
                }
            }
            foreach (InvEffect invEffect in __instance.baseItem.effects)
            {
                stringBuilder.Append("\n");
                if (invEffect.id == InvEffect.Identifier.Energy)
                {
                    stringBuilder.Append(T.ItemEffects("Energy", new object[]
                    {
                        invEffect.amount.ToString()
                    }));
                }
                else if (invEffect.id == (InvEffect.Identifier)((byte)126))
                {
                    stringBuilder.Append(T.ItemEffects("Experience", new object[]
                    {
                        invEffect.amount.ToString()
                    }));
                }
                else if (invEffect.id == InvEffect.Identifier.SlowTime)
                {
                    stringBuilder.Append(T.ItemEffects("SlowTime", new object[]
                    {
                        invEffect.duration.ToString()
                    }));
                }
                else
                {
                    stringBuilder.Append((invEffect.amount <= 0f) ? "[FF0000]" : "[00FF00]");
                    if (invEffect.id == InvEffect.Identifier.HealingMagic || invEffect.id == InvEffect.Identifier.HealthMagic || invEffect.id == InvEffect.Identifier.HealthRegenMagic || invEffect.id == InvEffect.Identifier.ManaMagic || invEffect.id == InvEffect.Identifier.ManaRegenMagic || invEffect.id == InvEffect.Identifier.BreathMagic || invEffect.id == InvEffect.Identifier.BreathRegenMagic)
                    {
                        stringBuilder.Append(T.ItemEffects((invEffect.amount <= 0f) ? "Damages" : "Restores", new object[0]));
                        stringBuilder.Append(" ");
                        stringBuilder.Append(T.ItemEffects(invEffect.id.ToString(), new object[]
                        {
                            (invEffect.modifier != InvEffect.Modifier.Added) ? Helper.FormatPercent(invEffect.amount, 0, false) : Helper.FormatValue(invEffect.amount, 0, false)
                        }));
                        if (invEffect.id == InvEffect.Identifier.HealthRegenMagic || invEffect.id == InvEffect.Identifier.ManaRegenMagic || invEffect.id == InvEffect.Identifier.BreathRegenMagic)
                        {
                            stringBuilder.Append(" ");
                            stringBuilder.Append(T.ItemEffects("Tick", new object[0]));
                        }
                    }
                    else if (invEffect.id == InvEffect.Identifier.HungerMagic)
                    {
                        stringBuilder.Append(T.ItemEffects(invEffect.id.ToString(), new object[]
                        {
                            (invEffect.modifier != InvEffect.Modifier.Added) ? Helper.FormatPercent(invEffect.amount, 0, false) : Helper.FormatValue(invEffect.amount, 0, false)
                        }));
                    }
                    else
                    {
                        stringBuilder.Append(T.ItemEffects(invEffect.id.ToString(), new object[]
                        {
                            (invEffect.modifier != InvEffect.Modifier.Added) ? Helper.FormatPercent(invEffect.amount, 0, true) : Helper.FormatValue(invEffect.amount, 0, true)
                        }));
                    }
                    if (invEffect.duration > 0f)
                    {
                        stringBuilder.Append(" ");
                        if (invEffect.id != InvEffect.Identifier.Explode && invEffect.id != InvEffect.Identifier.RadialDamage)
                        {
                            stringBuilder.Append(T.ItemEffects("Duration", new object[]
                            {
                                Helper.FormatValue(invEffect.duration, 0, false)
                            }));
                        }
                        else
                        {
                            stringBuilder.Append(T.ItemEffects("Radius", new object[]
                            {
                                Helper.FormatValue(invEffect.duration, 0, false)
                            }));
                        }
                    }
                    stringBuilder.Append("[-]");
                }
            }
            if (flag)
            {
                stringBuilder.Append("\n[00FF00]");
                stringBuilder.Append(T.ItemEffects("Light", new object[0]));
                stringBuilder.Append("[-]");
            }
            __result = stringBuilder.ToString();
            return false;
        }
    }
}
