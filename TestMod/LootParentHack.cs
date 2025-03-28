using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestMod
{
    public class LootParentHack : MonoBehaviour
    {
        Loot loot;
        protected void OnEnable()
        {
            if (this.transform.childCount > 0)
            {
                GameObject child = this.transform.GetChild(0).gameObject;
                loot = child.GetComponent<Loot>();
                var spriteName = AccessTools.Field(typeof(UISprite), "mSpriteName");
                spriteName.SetValue(loot.sprite, loot.name);
                loot.sprite.atlas = LoadModResource.IconAtlas;
                child.AddComponent<LootChildHack>();
                child.SetActive(true);
            }
        }

        public void SetItem(InvBaseItem item)
        {
            loot.SetItem(item);
        }

        // Token: 0x060008BA RID: 2234 RVA: 0x00044B89 File Offset: 0x00042F89
        protected void SetData(byte data)
        {
            loot.SendMessage("SetData", data);
        }

        // Token: 0x060008BB RID: 2235 RVA: 0x00044B92 File Offset: 0x00042F92
        protected void SetCount(int count)
        {
            loot.SendMessage("SetCount", count);
            
        }

        public static GameObject Make(GameObject preafab, string newName)
        {
            GameObject newParent = new GameObject(newName);
            GameObject newChild = Instantiate(preafab);
            newChild.name = newName;
            newChild.SetActive(false);
            newParent.AddComponent<LootParentHack>().StartCoroutine(SetParent(newChild, newParent));
            return newParent;
        }

        protected static System.Collections.IEnumerator SetParent(GameObject newChild, GameObject newParent)
        {
            yield return null;
            newChild.transform.SetParent(newParent.transform, false);
        }
    }
}
