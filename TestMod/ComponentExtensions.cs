using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TestMod
{
    public static class ComponentExtensions
    {
        public static void CopyFields<T>(T source, T target)
        {
            if (source == null || target == null)
            {
                Debug.LogError("Source or target is null!");
                return;
            }

            Type type = typeof(T);
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (FieldInfo field in type.GetFields(flags))
            {
                object value = field.GetValue(source);
                field.SetValue(target, value);
            }
        }

        public static void CopyFields<T1, T2>(T1 source, T2 target)
        {
            if (source == null || target == null)
            {
                Debug.LogError("Source or target is null!");
                return;
            }

            Type sourceType = typeof(T1);
            Type targetType = typeof(T2);

            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            foreach (FieldInfo sourceField in sourceType.GetFields(flags))
            {
                FieldInfo targetField = targetType.GetField(sourceField.Name, flags);

                if (targetField != null && targetField.FieldType == sourceField.FieldType)
                {
                    object value = sourceField.GetValue(source);
                    targetField.SetValue(target, value);
                }
            }
        }

        public static InvBaseItem SetDurability(this InvBaseItem invBaseItem, int durability)
        {
            invBaseItem.durability = durability;
            return invBaseItem;
        }

        public static InvBaseItem SetPrice(this InvBaseItem invBaseItem, int price)
        {
            invBaseItem.price = price;
            return invBaseItem;
        }

        public static InvBaseItem SetEffects(this InvBaseItem invBaseItem, System.Collections.Generic.List<InvEffect> effects)
        {
            invBaseItem.effects = effects;
            return invBaseItem;
        }
    }
}
