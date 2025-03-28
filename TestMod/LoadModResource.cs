using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace TestMod
{
    public class LoadModResource : AutoLoadResource
    {
        public string modResourceName;

        public void InvokeOnResourceLoaded()
        {
            var eventField = typeof(AutoLoadResource).GetField("OnResourceLoaded", BindingFlags.Instance | BindingFlags.NonPublic);
            if (eventField != null)
            {
                var eventDelegate = eventField.GetValue(this) as Action;
                eventDelegate?.Invoke();
            }
        }

        protected void OnEnable()
        {
            var eventField = typeof(AutoLoadResource).GetField("_child", BindingFlags.Instance | BindingFlags.NonPublic);
            GameObject child = eventField.GetValue(this) as GameObject;
            if (child != null)
            {
                UnityEngine.Object.Destroy(child);
            }

            child = (UnityEngine.Object.Instantiate(LoadResource<GameObject>(modResourceName)));

            Vector3 pos = child.transform.localPosition;
            Quaternion rot = child.transform.localRotation;
            Vector3 scl = child.transform.localScale;
            child.transform.parent = base.transform;
            child.transform.localPosition = pos;
            child.transform.localRotation = rot;
            child.transform.localScale = scl;

            eventField.SetValue(this, child);

            InvokeOnResourceLoaded();
        }

        public static void AddResource<T>(T resource, string name) where T : UnityEngine.Object
        {
            if (!resources.ContainsKey(name))
            {
                resources.Add(name, resource);
            }
        }
        protected static string bundleName = "Mods/testmodresources";
        protected static Dictionary<string, UnityEngine.Object> resources = new Dictionary<string, UnityEngine.Object>();
        protected static AssetBundle assetBundle;
        protected static bool isBundleLoaded = false;

        public static T LoadResource<T>(string name) where T : UnityEngine.Object
        {
            if (!resources.ContainsKey(name))
            {
                //AssetBundle assetBundle = null;
                if (!isBundleLoaded)
                {
                    Melon<Test>.Logger.Msg("Loading AssetBundle at:");
                    string bundlePath = Application.dataPath + "/../" + bundleName;
                    Melon<Test>.Logger.Msg(bundlePath);
                    if (File.Exists(bundlePath))
                    {
                        Melon<Test>.Logger.Msg("File exists!");
                    }
                    Melon<Test>.Logger.Msg("Loading into array");
                    byte[] bundleData = System.IO.File.ReadAllBytes(bundlePath);
                    Melon<Test>.Logger.Msg("Lead from memory");
                    assetBundle = AssetBundle.LoadFromMemory(bundleData);
                    isBundleLoaded = true;
                }
                Melon<Test>.Logger.Msg($"Load res {name}");
                resources[name] = assetBundle.LoadAsset<T>(name);
            }
            Melon<Test>.Logger.Msg("Loaded res: " + resources[name].name);
            return resources[name] as T;
        }

        protected static bool isIconAtlasLoaded = false;
        protected static UIAtlas iconAtlas;
        public static UIAtlas IconAtlas
        {
            get
            {
                if (iconAtlas == null)
                {
                    Material originalMat = null;
                    try
                    {
                        originalMat = QuestManager.FindAtlas("Icons").spriteMaterial;
                    }
                    catch (System.NullReferenceException)
                    {
                        foreach (var atlas in (Resources.FindObjectsOfTypeAll(typeof(UIAtlas)) as UIAtlas[]))
                        {
                            if (atlas.name == "Icons")
                            {
                                originalMat = atlas.spriteMaterial;
                            }
                        }
                    }
                    Material newMat = new Material(originalMat);
                    newMat.mainTexture = LoadResource<Texture2D>("ModIcons");
                    iconAtlas = new GameObject("IconAtlasHolder").AddComponent<UIAtlas>();
                    iconAtlas.spriteMaterial = newMat;
                    iconAtlas.coordinates = UIAtlas.Coordinates.TexCoords;
                    iconAtlas.pixelSize = 1;
                    iconAtlas.spriteList.Add(
                        new UIAtlas.Sprite() 
                        { 
                            name = "CustomBow",
                            inner=GetIconRect(0, 0),
                            outer=GetIconRect(0, 0), 
                            paddingBottom=0,
                            paddingLeft=0,
                            paddingRight=0,
                            paddingTop=0,
                            rotated=false
                        }
                    );
                    iconAtlas.spriteList.Add(
                        new UIAtlas.Sprite()
                        {
                            name = "MirrorOfElves",
                            inner = GetIconRect(1, 0),
                            outer = GetIconRect(1, 0),
                            paddingBottom = 0,
                            paddingLeft = 0,
                            paddingRight = 0,
                            paddingTop = 0,
                            rotated = false
                        }
                    );
                    iconAtlas.spriteList.Add(
                        new UIAtlas.Sprite()
                        {
                            name = "Amogus",
                            inner = GetIconRect(2, 0),
                            outer = GetIconRect(2, 0),
                            paddingBottom = 0,
                            paddingLeft = 0,
                            paddingRight = 0,
                            paddingTop = 0,
                            rotated = false
                        }
                    );
                    iconAtlas.spriteList.Add(
                        new UIAtlas.Sprite()
                        {
                            name = "ExperiencePotion",
                            inner = GetIconRect(3, 0),
                            outer = GetIconRect(3, 0),
                            paddingBottom = 0,
                            paddingLeft = 0,
                            paddingRight = 0,
                            paddingTop = 0,
                            rotated = false
                        }
                    );
                    iconAtlas.spriteList.Add(
                        new UIAtlas.Sprite()
                        {
                            name = "RainbowRod",
                            inner = GetIconRect(4, 0),
                            outer = GetIconRect(4, 0),
                            paddingBottom = 0,
                            paddingLeft = 0,
                            paddingRight = 0,
                            paddingTop = 0,
                            rotated = false
                        }
                    );
                    iconAtlas.spriteList.Add(
                        new UIAtlas.Sprite()
                        {
                            name = "RainbowOre",
                            inner = GetIconRect(5, 0),
                            outer = GetIconRect(5, 0),
                            paddingBottom = 0,
                            paddingLeft = 0,
                            paddingRight = 0,
                            paddingTop = 0,
                            rotated = false
                        }
                    );
                    iconAtlas.spriteList.Add(
                        new UIAtlas.Sprite()
                        {
                            name = "SusSword",
                            inner = GetIconRect(6, 0),
                            outer = GetIconRect(6, 0),
                            paddingBottom = 0,
                            paddingLeft = 0,
                            paddingRight = 0,
                            paddingTop = 0,
                            rotated = false
                        }
                    );
                    //isIconAtlasLoaded = true;
                }
                return iconAtlas;
            }
        }

        protected static Rect GetIconRect(int posX, int posY, float wX=0.125f, float wY = 0.125f)
        {
            return new Rect(posX * wX, posY * wY, wX, wY);
        }
    }
}
