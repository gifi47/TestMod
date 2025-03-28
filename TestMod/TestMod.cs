using HarmonyLib;
using MelonLoader;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TestMod
{
    public class Test : MelonMod
    {

        private Vector3 cameraLocalPos;
        InventoryCollector inventoryCollector;
        private float speed = 1f;
        private GameObject lootPrefab;
        GameObject canvasObject;

        /// <summary>
        /// Canvas is created but not interactable!
        /// </summary>
        private void CreateCanvas()
        {
            int layerUI = LayerMask.NameToLayer("UI");

            if (GameObject.FindObjectOfType<EventSystem>() == null)
            {

                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                eventSystem.layer = layerUI;
                LoggerInstance.Msg("+Event System Added!");
            }

            canvasObject = new GameObject("DebugCanvas");
            canvasObject.tag = "UI";
            canvasObject.layer = layerUI;
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.sortingOrder = 100;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Add CanvasScaler for UI scaling
            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Add a Panel to hold the debug text
            GameObject panelObject = new GameObject("DebugPanel");
            panelObject.tag = "UI";
            panelObject.layer = layerUI;
            panelObject.transform.SetParent(canvasObject.transform);
            RectTransform panelRect = panelObject.AddComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(400, 100);
            panelRect.anchoredPosition = new Vector2(0, 300);

            Image panelImage = panelObject.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.5f); // Semi-transparent black

            // Create the Text UI element
            GameObject textObject = new GameObject("DebugText");
            textObject.tag = "UI";
            textObject.layer = layerUI;
            textObject.transform.SetParent(panelObject.transform);
            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(400, 100);
            textRect.anchoredPosition = Vector2.zero;

            var debugText = textObject.AddComponent<Text>();
            debugText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            debugText.text = "Debug Info: ";
            debugText.fontSize = 24;
            debugText.alignment = TextAnchor.MiddleCenter;
            debugText.color = Color.white;

            // Create an Input Field
            GameObject inputFieldObject = new GameObject("InputField");
            inputFieldObject.tag = "UI";
            inputFieldObject.layer = layerUI;
            inputFieldObject.transform.SetParent(panelObject.transform);
            RectTransform inputFieldRect = inputFieldObject.AddComponent<RectTransform>();
            inputFieldRect.sizeDelta = new Vector2(300, 50);
            inputFieldRect.anchoredPosition = new Vector2(0, 25);

            Image inputFieldBg = inputFieldObject.AddComponent<Image>();
            inputFieldBg.color = Color.white; // Background color
            inputFieldBg.raycastTarget = true;
            var inputField = inputFieldObject.AddComponent<InputField>();

            // Create InputField Text
            GameObject inputTextObject = new GameObject("InputText");
            inputTextObject.tag = "UI";
            inputTextObject.layer = layerUI;
            inputTextObject.transform.SetParent(inputFieldObject.transform);
            RectTransform inputTextRect = inputTextObject.AddComponent<RectTransform>();
            inputTextRect.sizeDelta = new Vector2(280, 40);
            inputTextRect.anchoredPosition = Vector2.zero;

            Text inputText = inputTextObject.AddComponent<Text>();
            inputText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            inputText.text = "";
            inputText.fontSize = 24;
            inputText.alignment = TextAnchor.MiddleLeft;
            inputText.color = Color.black;

            inputField.textComponent = inputText;

            // Create a Button
            GameObject btnObject = new GameObject("Button");
            btnObject.tag = "UI";
            btnObject.layer = layerUI;
            btnObject.transform.SetParent(panelObject.transform);
            RectTransform btnRect = btnObject.AddComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(300, 50);
            btnRect.anchoredPosition = new Vector2(0, -25);

            Image btnImage = btnObject.AddComponent<Image>();
            btnImage.color = Color.gray;
            btnImage.raycastTarget = true;

            Button btn = btnObject.AddComponent<Button>();
            btn.interactable = true;
            btn.targetGraphic = btnImage;
            btn.onClick.AddListener(() =>
            {
                if (inventoryCollector != null)
                {
                    inventoryCollector.Collect(inputField.text, 0, 100);
                    MelonLogger.Msg($"Collected item: {inputField.text}");
                }
                else
                {
                    MelonLogger.Error("inventoryCollector is null!");
                }
            });

            // Create Button Text
            GameObject btnTextObject = new GameObject("ButtonText");
            btnTextObject.tag = "UI";
            btnTextObject.layer = layerUI;
            btnTextObject.transform.SetParent(btnObject.transform);
            RectTransform btnTextRect = btnTextObject.AddComponent<RectTransform>();
            btnTextRect.sizeDelta = new Vector2(280, 40);
            btnTextRect.anchoredPosition = Vector2.zero;

            Text btnText = btnTextObject.AddComponent<Text>();
            btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            btnText.text = "Collect";
            btnText.fontSize = 24;
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.color = Color.white;
        }

        int modId16 = 300;

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been loaded!");
            cameraLocalPos = Camera.main.transform.localPosition;
            if (buildIndex == 2)
            {
                if (inventoryCollector == null)
                {
                    inventoryCollector = GameObject.FindObjectOfType<InventoryCollector>();
                }
                InitNewBow();
                
                InvDatabase lootDataBase = GameObject.Find("LootDatabase").GetComponent<InvDatabase>();
                InvDatabase toolDataBase = GameObject.Find("ToolsDatabase").GetComponent<InvDatabase>();
                
                Register(CopyItem("MirrorOfElves", lootDataBase, 4).SetPrice(250));
                Register(CopyItem("RainbowOre", lootDataBase, 8).SetPrice(140));

                InitRainbowRod();

                Register(CopyItem("Amogus", lootDataBase, 4));
                InitPotions();

                InitSusSword();
                LoadRecipes();
            }
            else if (buildIndex == 1)
            {
                inventoryCollector = null;
                GameObject original = GameObject.Find("Link");
                if (original != null)
                {
                    UITransform utr = GameObject.Instantiate(original).GetComponent<UITransform>();
                    utr.transform.SetParent(original.transform.parent, false);
                    utr.transform.localPosition = new Vector3(original.transform.localPosition.x, original.transform.localPosition.y + 360, original.transform.localPosition.z);
                    utr.customTrans[0].position = utr.transform.localPosition * 2;
                    UILabel label = utr.GetComponentInChildren<UILabel>();
                    label.transform.SetParent(utr.transform, false);
                    GameObject.Destroy(label.GetComponent<TextDisplay>());
                    label.text = "[00FF00]TestMod by gifi47 Loaded![-]";
                    GameObject.Destroy(utr.GetComponentInChildren<BoxCollider>().gameObject);
                } else
                {
                    LoggerInstance.Msg("Link not Found!");
                }
            }
        }

        public override void OnInitializeMelon()
        {
            Application.logMessageReceived += OnLogMessageReceived;
            var harmony = new HarmonyLib.Harmony("com.testmod.translationpatch");
            harmony.PatchAll();
            Debug.Log("MyMod: Harmony patches applied!");
            LoggerInstance.Msg(Path.Combine(Application.persistentDataPath, "recipe"));
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                LoggerInstance.Msg("F5 was pressed!");
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                inventoryCollector.Collect(InvDatabase.CreateItem("RainbowOre", 0, 1));
                inventoryCollector.Collect(InvDatabase.CreateItem("Amogus", 0, 1));
                inventoryCollector.Collect(InvDatabase.CreateItem("MirrorOfElves", 0, 1));
                inventoryCollector.Collect(InvDatabase.CreateItem("ExperiencePotion", 0, 1));
            }
            if (Input.GetKeyDown(KeyCode.F7))
            {
                inventoryCollector.Collect(InvDatabase.CreateItem("CustomBow", 0, 250));
            }
            if (Input.GetKeyDown(KeyCode.F8))
            {
                inventoryCollector.Collect(InvDatabase.CreateItem("RainbowRod", 0, 70));
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                inventoryCollector.Collect("Stick", 0, 100);
            }
            if (Input.GetKeyDown(KeyCode.F10))
            {
                if (lootPrefab == null)
                {
                    lootPrefab = Resources.Load<GameObject>("loot/Diamond");
                    LoggerInstance.Msg($"loaded: {lootPrefab}");
                }
                int num = UnityEngine.Random.Range(1, 100);
                GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(
                    lootPrefab, 
                    Camera.main.transform.position + UnityEngine.Random.insideUnitSphere * 0.3f + new Vector3(0, 4, 0),
                    Quaternion.identity);
                gameObject.SendMessage("SetCount", num);
                gameObject.SendMessage("SetPickup", true);
            }
            if (Input.GetKeyDown(KeyCode.F11))
            {
                //Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
            }
            if (Input.GetKeyDown(KeyCode.F12))
            {
                inventoryCollector.Collect("Bow", 0, 100);
                inventoryCollector.Collect("Arrow", 0, 100);
            }
        }

        private void InitPotions()
        {
            InvDatabase toolDataBase = GameObject.Find("LootDatabase").GetComponent<InvDatabase>();

            toolDataBase.items[15].options = (InvBaseItem.Option.Collectable | InvBaseItem.Option.Consumable);
            toolDataBase.items[15].slot = InvBaseItem.Slot.Weapon;

            GameObject eatToolAttachment = toolDataBase.items[29].attachment;

            GameObject spiderVenomTool = GameObject.Instantiate(eatToolAttachment, eatToolAttachment.transform.parent);
            spiderVenomTool.transform.localPosition = eatToolAttachment.transform.localPosition;
            spiderVenomTool.transform.localRotation = eatToolAttachment.transform.localRotation;
            spiderVenomTool.transform.localScale = eatToolAttachment.transform.localScale;
            spiderVenomTool.gameObject.name = "SpiderVenomPotion";

            GameObject hand = spiderVenomTool.GetComponent<EatTool>().handTool;
            GameObject spiderVenomHand = GameObject.Instantiate(hand, hand.transform.parent);
            spiderVenomHand.transform.localPosition = hand.transform.localPosition;
            spiderVenomHand.transform.localRotation = hand.transform.localRotation;
            spiderVenomHand.transform.localScale = hand.transform.localScale;
            spiderVenomHand.gameObject.name = "SpiderVenomPotion";
            GameObject.Destroy(spiderVenomHand.GetComponent<AutoLoadResource>());
            

            spiderVenomTool.GetComponent<EatTool>().handTool = spiderVenomHand;

            GameObject originalPotion = Resources.Load<GameObject>("toolsprefab/HealthPotion");
            GameObject potion = GameObject.Instantiate(originalPotion, originalPotion.transform.localPosition, originalPotion.transform.localRotation);
            foreach (var mr in potion.GetComponentsInChildren<MeshRenderer>())
            {
                (mr.sharedMaterial = new Material(mr.sharedMaterial)).mainTexture = LoadModResource.LoadResource<Texture2D>("BottleSpiderVenom");
            }
            spiderVenomHand.AddComponent<LoadInstantiatedResource>().AssignInstantiatedResource(potion);

            toolDataBase.items[15].attachment = spiderVenomTool;
            toolDataBase.items[15].effects.Add(
                new InvEffect()
                {
                    id=InvEffect.Identifier.PoisonDamage,
                    modifier=InvEffect.Modifier.Added,
                    amount=50,
                    duration=10
                }
            );
            //Register(toolDataBase.items[15]);

            GameObject potion2 = GameObject.Instantiate(originalPotion, originalPotion.transform.localPosition, originalPotion.transform.localRotation);
            foreach (var mr in potion2.GetComponentsInChildren<MeshRenderer>())
            {
                (mr.sharedMaterial = new Material(mr.sharedMaterial)).mainTexture = LoadModResource.LoadResource<Texture2D>("BottleExperiencePotion");
            }

            InvBaseItem experiencePotion = CopyItem("ExperiencePotion", toolDataBase, 29);

            GameObject experiencePotionTool = GameObject.Instantiate(eatToolAttachment, eatToolAttachment.transform.parent);
            experiencePotionTool.transform.localPosition = eatToolAttachment.transform.localPosition;
            experiencePotionTool.transform.localRotation = eatToolAttachment.transform.localRotation;
            experiencePotionTool.transform.localScale = eatToolAttachment.transform.localScale;
            experiencePotionTool.gameObject.name = "ExperiencePotion";

            GameObject experiencePotionHand = GameObject.Instantiate(hand, hand.transform.parent);
            experiencePotionHand.transform.localPosition = hand.transform.localPosition;
            experiencePotionHand.transform.localRotation = hand.transform.localRotation;
            experiencePotionHand.transform.localScale = hand.transform.localScale;
            experiencePotionHand.gameObject.name = "ExperiencePotion";
            GameObject.Destroy(experiencePotionHand.GetComponent<AutoLoadResource>());
            experiencePotionHand.AddComponent<LoadInstantiatedResource>().AssignInstantiatedResource(potion2);

            experiencePotionTool.GetComponent<EatTool>().handTool = experiencePotionHand;

            experiencePotion.attachment = experiencePotionTool;

            experiencePotion.effects = new List<InvEffect>();
            experiencePotion.effects.Add(
                new InvEffect()
                {
                    id = (InvEffect.Identifier)((byte)126),
                    modifier = InvEffect.Modifier.Added,
                    amount=50,
                    duration=0
                }
            );

            Register(experiencePotion);
        }

        private void InitSusSword()
        {
            InvDatabase toolDataBase = GameObject.Find("ToolsDatabase").GetComponent<InvDatabase>();

            InvBaseItem susSword = CopyItem("SusSword", toolDataBase, 2);

            var susSwordTool = GameObject.Instantiate(susSword.attachment, susSword.attachment.transform.parent);
            susSwordTool.transform.localPosition = susSword.attachment.transform.localPosition;
            susSwordTool.transform.localRotation = susSword.attachment.transform.localRotation;
            susSwordTool.transform.localScale = susSword.attachment.transform.localScale;
            susSwordTool.gameObject.name = "SusSword";

            var swordAttack = susSwordTool.GetComponent<SwordAttack>();
            swordAttack.attackSounds = new AudioClip[2];
            swordAttack.attackSounds[0] = LoadModResource.LoadResource<AudioClip>("sus1");
            swordAttack.attackSounds[1] = LoadModResource.LoadResource<AudioClip>("sus2");
            swordAttack.hitSounds = new AudioClip[1];
            swordAttack.hitSounds[0] = LoadModResource.LoadResource<AudioClip>("sus3");

            var originalHand = swordAttack.handTool;
            var newHand = GameObject.Instantiate(originalHand, originalHand.transform.parent);
            newHand.transform.localPosition = originalHand.transform.localPosition;
            newHand.transform.localRotation = originalHand.transform.localRotation;
            newHand.transform.localScale = originalHand.transform.localScale;
            newHand.gameObject.name = "SusSword";
            GameObject.Destroy(newHand.GetComponent<AutoLoadResource>());
            newHand.AddComponent<LoadModResource>().modResourceName = "SusSword";
            
            // need patch
            //LoadModResource.LoadResource<GameObject>("SusSword");

            swordAttack.handTool = newHand;

            susSword.attachment = susSwordTool;

            susSword.effects = new List<InvEffect>() 
            { 
                new InvEffect() 
                { 
                    id = InvEffect.Identifier.NormalDamage, 
                    modifier = InvEffect.Modifier.Percent, 
                    amount = 0.35f, 
                    duration = 0
                }
            };

            susSword.price = 1500;

            Register(susSword);
        }

        private void InitRainbowRod()
        {

            InvDatabase toolDataBase = GameObject.Find("ToolsDatabase").GetComponent<InvDatabase>();

            InvBaseItem rod = CopyItem("RainbowRod", toolDataBase, 1);
            
            GameObject origStaffTool = rod.attachment;
            GameObject rainbowStaffTool = GameObject.Instantiate(origStaffTool, origStaffTool.transform.parent);
            rainbowStaffTool.transform.localPosition = origStaffTool.transform.localPosition;
            rainbowStaffTool.transform.localRotation = origStaffTool.transform.localRotation;
            rainbowStaffTool.transform.localScale = origStaffTool.transform.localScale;
            rainbowStaffTool.gameObject.name = "RainbowRod";
            MagicAttack magicAttackComponent = rainbowStaffTool.GetComponent<MagicAttack>();
            magicAttackComponent.needMana = 60;
            magicAttackComponent.coolDown = 1.1f;
            GameObject particles = LoadModResource.LoadResource<GameObject>("RainbowParticles");
            particles.AddComponent<DestroyThisTimed>().destroyTime = 2f;
            magicAttackComponent.magicEffect = particles;

            GameObject newStaffObj = GameObject.Instantiate(magicAttackComponent.staffObj, magicAttackComponent.staffObj.transform.parent);
            newStaffObj.transform.localPosition = magicAttackComponent.staffObj.transform.localPosition;
            newStaffObj.transform.localRotation = magicAttackComponent.staffObj.transform.localRotation;
            newStaffObj.transform.localScale = magicAttackComponent.staffObj.transform.localScale;
            newStaffObj.gameObject.name = "RainbowRod";
            GameObject.Destroy(newStaffObj.GetComponent<AutoLoadResource>());
            LoadModResource modRes = newStaffObj.AddComponent<LoadModResource>();
            modRes.modResourceName = "RainbowRod";

            newStaffObj.GetComponent<HandTools>().PositionOffset += new Vector3(0, 0.1f, 0);

            magicAttackComponent.staffObj = newStaffObj;

            rod.attachment = rainbowStaffTool;

            rod.extraOptions = InvBaseItem.ExtraOption.RequestDrop;
            rod.effects = new List<InvEffect>();
            rod.effects.Add(
                new InvEffect()
                {
                    id = InvEffect.Identifier.EarthMagic,
                    modifier = InvEffect.Modifier.Added,
                    amount = 5,
                    duration = 0
                }
            );
            rod.effects.Add(
                new InvEffect()
                {
                    id = InvEffect.Identifier.FireMagic,
                    modifier = InvEffect.Modifier.Added,
                    amount = 5,
                    duration = 0
                }
            );
            rod.effects.Add(
                new InvEffect()
                {
                    id = InvEffect.Identifier.FireMagic,
                    modifier = InvEffect.Modifier.Added,
                    amount = 5,
                    duration = 5
                }
            );
            rod.effects.Add(
                new InvEffect()
                {
                    id = InvEffect.Identifier.LightningMagic,
                    modifier = InvEffect.Modifier.Added,
                    amount = 5,
                    duration = 5
                }
            );
            rod.effects.Add(
                new InvEffect()
                {
                    id = InvEffect.Identifier.IceMagic,
                    modifier = InvEffect.Modifier.Added,
                    amount = 5,
                    duration = 0
                }
            );
            rod.effects.Add(
                new InvEffect()
                {
                    id = InvEffect.Identifier.Magic,
                    modifier = InvEffect.Modifier.Added,
                    amount = 5,
                    duration = 0
                }
            );
            rod.effects.Add(
                new InvEffect()
                {
                    id = InvEffect.Identifier.DarkMagic,
                    modifier = InvEffect.Modifier.Added,
                    amount = 5,
                    duration = 0
                }
            );
            rod.effects.Add(
                new InvEffect()
                {
                    id = InvEffect.Identifier.HealingMagic,
                    modifier = InvEffect.Modifier.Added,
                    amount = 5,
                    duration = 0
                }
            );
            rod.effects.Add(
                new InvEffect()
                {
                    id = InvEffect.Identifier.HealingMagic,
                    modifier = InvEffect.Modifier.Added,
                    amount = 20,
                    duration = 20
                }
            );
            rod.price = 680;
            rod.durability = 70;
            Register(rod);
        }

        private void InitNewBow()
        {
            Bow bow = Object.FindObjectOfType<Bow>();
            if (bow == null)
            {
                Object[] rezult = Object.FindObjectsOfTypeAll(typeof(Bow));
                LoggerInstance.Msg($"Bows found: {rezult.Length}");
                if (rezult.Length > 0)
                {
                    bow = (Bow)rezult[0];
                }
            }
            Bow bowComponent = null;
            if (bow != null)
            {
                bowComponent = GameObject.Instantiate(bow, bow.transform.parent);
                bowComponent.transform.localPosition = bow.transform.localPosition;
                bowComponent.transform.localRotation = bow.transform.localRotation;
                bowComponent.transform.localScale = bow.transform.localScale;
                bowComponent.coolDown = 0.7f;
                bowComponent.gameObject.name = "CustomBow";
                bowComponent.sound = new AudioClip[2];
                bowComponent.sound[0] = LoadModResource.LoadResource<AudioClip>("CustomBowShoot1ed");
                bowComponent.sound[1] = LoadModResource.LoadResource<AudioClip>("CustomBowShoot2ed");
            }

            InvDatabase invBase = GameObject.Find("ToolsDatabase").GetComponent<InvDatabase>();
            InvBaseItem originalBow = invBase.items[3];

            GameObject bowGameObject = bowComponent.gameObject;
            if (bowComponent == null)
            {
                bowComponent = originalBow.attachment.GetComponent<Bow>();
                bowGameObject = bowComponent.gameObject;
            } else
            {
                CustomBow customBowComponent = bowGameObject.AddComponent<CustomBow>();
                ComponentExtensions.CopyFields(bowComponent, customBowComponent);


                GunEnabler gunEnablerComponent = bowGameObject.GetComponent<GunEnabler>();

                GameObject originalBowTool = gunEnablerComponent.gunTool;

                GameObject customBowTool = GameObject.Instantiate(originalBowTool);
                customBowTool.name = "CustomBow";
                customBowTool.transform.parent = originalBowTool.transform.parent;
                customBowTool.transform.localPosition = originalBowTool.transform.localPosition;
                customBowTool.transform.localRotation = originalBowTool.transform.localRotation;
                customBowTool.transform.localScale = originalBowTool.transform.localScale;

                GameObject.Destroy(customBowTool.GetComponent<AutoLoadResource>());
                LoadModResource modRes = customBowTool.AddComponent<LoadModResource>();
                modRes.modResourceName = "CustomBow";

                gunEnablerComponent.gunTool = customBowTool;
                customBowComponent.bowObject = customBowTool;

                GameObject.Destroy(bowComponent);
            }

            GameObject newLoot = LootParentHack.Make(originalBow.loot, "CustomBow");

            InvBaseItem customBow = new InvBaseItem() { 
                id16=modId16++, 
                attachment = bowGameObject,
                category = originalBow.category,
                color = originalBow.color,
                durability = 250,
                effects = originalBow.effects,
                extraOptions = originalBow.extraOptions,
                iconAtlas = LoadModResource.IconAtlas,
                iconName = "CustomBow",
                loot = newLoot,
                name = "CustomBow",
                options = originalBow.options,
                price = originalBow.price,
                slot = originalBow.slot,
                stats = originalBow.stats
            };
            invBase.items.Add(customBow);

            Register("CustomBow", customBow);
        }

        public InvBaseItem CopyItemExact(string itemName, InvDatabase invDataBase, int originalItemId)
        {
            InvBaseItem originalItem = invDataBase.items[originalItemId];

            InvBaseItem item = new InvBaseItem()
            {
                id16 = modId16++,
                attachment = originalItem.attachment,
                category = originalItem.category,
                color = originalItem.color,
                durability = originalItem.durability,
                effects = originalItem.effects,
                extraOptions = originalItem.extraOptions,

                iconAtlas = originalItem.iconAtlas,
                iconName = originalItem.iconName,

                loot = originalItem.loot,
                name = itemName,                        //originalItem.name,
                options = originalItem.options,
                price = originalItem.price,
                slot = originalItem.slot,
                stats = originalItem.stats
            };
            invDataBase.items.Add(item);

            return item;
        }

        public InvBaseItem CopyItem(string itemName, InvDatabase invDataBase, int originalItemId)
        {
            InvBaseItem originalItem = invDataBase.items[originalItemId]; 

            InvBaseItem item = new InvBaseItem()
            {
                id16 = modId16++,
                attachment = originalItem.attachment,
                category = originalItem.category,
                color = originalItem.color,
                durability = originalItem.durability,
                effects = originalItem.effects,
                extraOptions = originalItem.extraOptions,

                iconAtlas = LoadModResource.IconAtlas,  //originalItem.iconAtlas,
                iconName = itemName,                    //originalItem.iconName,

                loot = LootParentHack.Make(originalItem.loot, itemName),
                name = itemName,                        //originalItem.name,
                options = originalItem.options,
                price = originalItem.price,
                slot = originalItem.slot,
                stats = originalItem.stats
            };
            invDataBase.items.Add(item);

            return item;
        }

        public void Register(string itemName, InvBaseItem itemBase)
        {
            System.Type databaseType = typeof(InvDatabase);
            FieldInfo myField = databaseType.GetField("byNames", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            var field = (Dictionary<string, InvBaseItem>)myField.GetValue(null);
            if (field.ContainsKey(itemName))
            {
                field[itemName] = itemBase;
            } else 
                field.Add(itemName, itemBase);
            myField.SetValue(null, field);

            LoggerInstance.Msg($"Item {itemName} registred");
        }

        public void Register(InvBaseItem itemBase)
        {
            System.Type databaseType = typeof(InvDatabase);
            FieldInfo myField = databaseType.GetField("byNames", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

            var field = (Dictionary<string, InvBaseItem>)myField.GetValue(null);
            field.Add(itemBase.name, itemBase);
            myField.SetValue(null, field);

            LoggerInstance.Msg($"Item {itemBase.name} registred");
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            MelonLogger.Msg($"[HOOKED LOG] {type}: {condition}");
        }

        public override void OnDeinitializeMelon()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        protected void LoadRecipes()
        {
            string path = Application.dataPath + "/../" + "Mods/Recipes/TestMod";
            if (!Directory.Exists(path))
            {
                LoggerInstance.Msg($"Directory {path} for mod recipes doesn't exist!");
                return;
            }
            RecipeManager recipeManager = GameObject.Find("RecipeManager").GetComponent<RecipeManager>();
            if (recipeManager == null)
            {
                LoggerInstance.Msg($"RecipeManager not Found!");
                return;
            }
            foreach (string text in Directory.GetFiles(path, "*.xml"))
            {
                try
                {
                    using (StreamReader streamReader = new StreamReader(text))
                    {
                        Recipe recipe = recipeManager.parse(streamReader);  
                        if (recipeManager.categorized.ContainsKey(recipe.result.baseItem.category))
                        {
                            var AddRecipe = AccessTools.Method(typeof(RecipeManager), "AddRecipe", new System.Type[] { typeof(List<Recipe>), typeof(Recipe) });
                            AddRecipe.Invoke(recipeManager, new object[] { recipeManager.categorized[recipe.result.baseItem.category], recipe });
                            LoggerInstance.Msg("Mod recipe added " + recipe.name);
                        } else
                        {
                            var AddRecipe = AccessTools.Method(typeof(RecipeManager), "AddRecipe", new System.Type[] { typeof(List<Recipe>), typeof(Recipe) });
                            AddRecipe.Invoke(recipeManager, new object[] { recipeManager.categorized[InvBaseItem.CreativeCategory.Weapons], recipe });
                            LoggerInstance.Msg("Mod recipe added " + recipe.name);
                        }
                    }
                }
                catch (System.Exception message)
                {
                    LoggerInstance.Msg("Unable to load user recipe from file " + text);
                    LoggerInstance.Msg($"Error msg:{message.Message}");
                    UnityEngine.Debug.Log("Unable to load user recipe from file " + text);
                    UnityEngine.Debug.LogError(message);
                }
            }
        }

        private void DumpScriptInfo(Assembly assembly)
        {
            foreach (System.Type type in assembly.GetTypes())
            {
                if (type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    try
                    {
                        // Try to get GUID through MonoScript (may not work at runtime)
                        var monoScriptType = typeof(MonoBehaviour).Assembly.GetType("UnityEngine.MonoScript");
                        var method = monoScriptType.GetMethod("FromMonoBehaviour");
                        var monoScript = method.Invoke(null, new object[] { System.Activator.CreateInstance(type) });
                        var guidProp = monoScriptType.GetProperty("guid");
                        string guid = guidProp?.GetValue(monoScript, null)?.ToString() ?? "Unknown";

                        MelonLogger.Msg($"Type: {type.Name} | GUID: {guid}");
                    }
                    catch (System.Exception e)
                    {
                        MelonLogger.Warning($"Failed to get GUID for {type.Name}: {e.Message}");
                    }
                }
            }
        }

        private void PrintTypeGUID(System.Type type)
        {
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                try
                {
                    // Try to get GUID through MonoScript (may not work at runtime)
                    var monoScriptType = typeof(MonoBehaviour).Assembly.GetType("UnityEngine.MonoScript");
                    MelonLogger.Msg($"1");
                    var method = monoScriptType.GetMethod("FromMonoBehaviour");
                    MelonLogger.Msg($"2");
                    var monoScript = method.Invoke(null, new object[] { System.Activator.CreateInstance(type) });
                    MelonLogger.Msg($"3");
                    var guidProp = monoScriptType.GetProperty("guid");
                    MelonLogger.Msg($"4");
                    string guid = guidProp?.GetValue(monoScript, null)?.ToString() ?? "Unknown";

                    MelonLogger.Msg($"Type: {type.Name} | GUID: {guid}");
                }
                catch (System.Exception e)
                {
                    MelonLogger.Warning($"Failed to get GUID for {type.Name}: {e.Message}");
                }
            }
        }

        private string GetScriptGuid(MonoBehaviour script)
        {
            // This uses undocumented Unity internals - may break in different versions
            var unityType = script.GetType().BaseType;
            var scriptType = typeof(UnityEngine.Object)
                .Assembly
                .GetType("UnityEngine.ScriptableObject")
                ?.GetMethod("GetScriptTypeFromClassName")?
                .Invoke(null, new object[] { script.GetType().Name });
            if (scriptType == null)
                MelonLogger.Msg("scriptType is null");
            else
                if (scriptType.GetType().GetProperty("guid") == null)
            {
                MelonLogger.Msg("guid property is null");
            } else if (scriptType.GetType().GetProperty("guid").GetValue(scriptType, null) == null)
            {
                MelonLogger.Msg("guid property value is null");
            }
            return scriptType?.GetType().GetProperty("guid")?.GetValue(scriptType, null)?.ToString() ?? "Unknown";
        }
    }

    public class SingleAssetLoader : MonoBehaviour
    {
        public string assetPath = "D:/Projects/BS_raw_proj/Block Story/Assets/AssetBundles/testmodresources";
        public string assetName = "CustomBow";

        protected void Start()
        {
            string bundlePath = assetPath;// Path.Combine(Application.dataPath, "Mods/MyBundle.bundle");


            if (System.IO.File.Exists(bundlePath))
            {
                byte[] bundleData = System.IO.File.ReadAllBytes(bundlePath);
                AssetBundle bundle = AssetBundle.LoadFromMemory(bundleData);

                /*AssetBundleCreateRequest bundle = AssetBundle.LoadFromFileAsync(bundlePath);
                yield return bundle;
                if (bundle != null)
                {
                    GameObject prefab = bundle.assetBundle.LoadAsset<GameObject>(assetName); // Имя объекта в AssetBundle
                    Instantiate(prefab);
                }*/
                if (bundle != null)
                {
                    if (bundle.Contains(assetName))
                    {
                        GameObject prefab = bundle.LoadAsset<GameObject>(assetName); // Имя объекта в AssetBundle
                        
                        Instantiate(prefab);
                    }
                    else
                    {
                        Debug.Log("Asset Bundle don't contains " + assetName);
                    }
                }
            }
        }
    }
}
