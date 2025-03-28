using MelonLoader;
using System;
using System.Collections;
using UnityEngine;

namespace TestMod
{
    // Token: 0x020001C9 RID: 457
    public class CustomBow : Use
    {
        // Token: 0x17000075 RID: 117
        // (get) Token: 0x06000B19 RID: 2841 RVA: 0x0005BF38 File Offset: 0x0005A338
        // (set) Token: 0x06000B1A RID: 2842 RVA: 0x0005BF40 File Offset: 0x0005A340
        private InvGameItem gameItem
        {
            get
            {
                return this._gameItem;
            }
            set
            {
                if (this._gameItem != value)
                {
                    this._gameItem = value;
                    if (this._gameItem != null)
                    {
                        if (this._gameItem.name == "Arrow")
                        {
                            this.arrowType = ArrowType.Regular;
                            this.arrow = this.regularArrow;
                        }
                        else if (this._gameItem.name == "Fire Arrow")
                        {
                            this.arrowType = ArrowType.Fire;
                            this.arrow = this.fireArrow;
                        }
                        else if (this._gameItem.name == "Magic Arrow")
                        {
                            this.arrowType = ArrowType.Magic;
                            this.arrow = this.magicArrow;
                        }
                        else if (this._gameItem.name == "Poisoned Arrow")
                        {
                            this.arrowType = ArrowType.Poisoned;
                            this.arrow = this.poisonedArrow;
                        }
                        else if (this._gameItem.name == "Explosive Arrow")
                        {
                            this.arrowType = ArrowType.Explosive;
                            this.arrow = this.explosiveArrow;
                        }
                        else if (this._gameItem.name == "Teleportation Arrow")
                        {
                            this.arrowType = ArrowType.Teleport;
                            this.arrow = this.teleportationArrow;
                        }
                        else
                        {
                            this._gameItem = null;
                        }
                    }
                    else
                    {
                        this._gameItem = null;
                    }
                    if (this.arrowBow != null)
                    {
                        if (this.gameItem != null)
                        {
                            this.arrowBow.SetArrow(this.arrowType);
                        }
                        else
                        {
                            this.arrowBow.DisableArrows();
                        }
                    }
                }
            }
        }

        // Token: 0x17000076 RID: 118
        // (get) Token: 0x06000B1B RID: 2843 RVA: 0x0005C0E7 File Offset: 0x0005A4E7
        private int arrowCount
        {
            get
            {
                if (this.gameItem != null && this.leftHand != null && this.leftHand.item != null)
                {
                    return this.leftHand.item.count;
                }
                return 0;
            }
        }

        // Token: 0x06000B1C RID: 2844 RVA: 0x0005C121 File Offset: 0x0005A521
        private void Awake()
        {
            this.fireSound = this.soundSource.GetComponent<AudioSource>();
        }

        // Token: 0x06000B1D RID: 2845 RVA: 0x0005C134 File Offset: 0x0005A534
        protected void OnEnable()
        {
            base.StartCoroutine(this.Initialize());
        }

        // Token: 0x06000B1E RID: 2846 RVA: 0x0005C144 File Offset: 0x0005A544
        private IEnumerator Initialize()
        {
            yield return new WaitForEndOfFrame();
            while (this.arrowBow == null)
            {
                this.arrowBow = this.bowObject.GetComponentInChildren<AssignArrow>();
                yield return null;
            }
            while (this.animation == null)
            {
                this.animation = this.bowObject.GetComponentInChildren<Animation>();
                yield return null;
            }
            this.playerItemStorage.RefreshSlot += this.UpdateCount;
            this.playerQuickSlotStorage.RefreshSlot += this.UpdateCount;
            this.equipment.EquippedLeftHand += this.UpdateCount;
            this.equipment.EquippedLeftHand += this.OnArrowChanged;
            this.OnArrowChanged();
            this.Reload();
            yield break;
        }

        // Token: 0x06000B1F RID: 2847 RVA: 0x0005C160 File Offset: 0x0005A560
        private void OnDisable()
        {
            this.playerItemStorage.RefreshSlot -= this.UpdateCount;
            this.playerQuickSlotStorage.RefreshSlot -= this.UpdateCount;
            this.equipment.EquippedLeftHand -= this.UpdateCount;
            this.equipment.EquippedLeftHand -= this.OnArrowChanged;
            if (this.leftHand != null)
            {
                this.leftHand.OnItemChange -= this.OnCountChanged;
            }
        }

        // Token: 0x06000B20 RID: 2848 RVA: 0x0005C1EC File Offset: 0x0005A5EC
        private void Update()
        {
            if (this.attackTimer > 0f)
            {
                this.attackTimer -= Time.deltaTime;
            }
            if (this.attackTimer < 0f && this.arrowCount > 0 && !this.reloaded)
            {
                this.Reload();
            }
        }

        // Token: 0x06000B21 RID: 2849 RVA: 0x0005C248 File Offset: 0x0005A648
        private void Firing(Ray ray)
        {
            bool flag = false;
            RaycastHit raycastHit;
            if (Physics.Raycast(ray, out raycastHit, (float)this.reach, this.hitLayers))
            {
                if (this.terrain.RayCastForSolid(ray.origin, raycastHit.point))
                {
                    BlockHit block = base.HitBlocks(ray.origin, ray.direction);
                    flag = UseBlock.instance.VerifyUsable(block);
                }
                else
                {
                    GameObject gameObject = raycastHit.collider.gameObject;
                    if (!PlayerMounted.mounted)
                    {
                        flag = (!gameObject.CompareTag("Enemy") && !gameObject.CompareTag("Animal") && !gameObject.CompareTag("NPC"));
                    }
                }
            }
            else
            {
                BlockHit block2 = base.HitBlocks(ray.origin, ray.direction);
                flag = UseBlock.instance.VerifyUsable(block2);
            }
            if (!flag)
            {
                this.Shoot();
            }
        }

        // Token: 0x06000B22 RID: 2850 RVA: 0x0005C33C File Offset: 0x0005A73C
        private void Reload()
        {
            if (this.arrowBow != null)
            {
                if (this.gameItem != null)
                {
                    this.attackTimer = 0f;
                    this.reloaded = true;
                    this.arrowBow.SetArrow(this.arrowType);
                }
                else
                {
                    this.arrowBow.DisableArrows();
                }
            }
        }

        // Token: 0x06000B23 RID: 2851 RVA: 0x0005C398 File Offset: 0x0005A798
        private void Shoot()
        {
            if (this.gameItem != null && this.reloaded)
            {
                this.reloaded = false;
                base.StartCoroutine(this.Fire());
            }
        }

        // Token: 0x06000B24 RID: 2852 RVA: 0x0005C3C4 File Offset: 0x0005A7C4
        protected IEnumerator Fire()
        {
            Melon<Test>.Logger.Msg("animation name: " + this.animationName);
            foreach (AnimationState state in animation)
            {
                Melon<Test>.Logger.Msg("Animation Clip: " + state.name);
            }
            this.animation.CrossFade(this.animationName);
            yield return new WaitForSeconds(1f);
            Transform bowTransform = this.bowObject.transform;
            GameObject prefab = UnityEngine.Object.Instantiate<GameObject>(this.arrow, bowTransform.position, bowTransform.rotation);
            ItemMotor motor = prefab.GetComponent<ItemMotor>();

            motor.velocity = Camera.main.transform.forward * (this.force + this.attributes[InvStat.Identifier.BowForce].maxValue * 0.25f);

            StartCoroutine(FireAdditional());

            this.attackTimer = this.coolDown;
            this.inventory.Consume();
            this.leftHand.item.count--;
            if (this.arrowCount <= 0)
            {
                this.equipment.Unequip(InvBaseItem.Slot.LeftHand);
                this.leftHand.item = null;
            }
            this.arrowBow.DisableArrows();
            if (!NGUITools.mute)
            {
                this.fireSound.clip = this.sound[UnityEngine.Random.Range(0, this.sound.Length)];
                this.fireSound.Play();
            }
            if (this.indicator != null)
            {
                this.indicator.AutoIndicator(this.coolDown, this.iconName);
            }
            yield break;
        }

        protected IEnumerator FireAdditional()
        {
            Transform bowTransform = this.bowObject.transform;
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.1f);
                GameObject prefab = UnityEngine.Object.Instantiate<GameObject>(this.arrow, bowTransform.position, bowTransform.rotation);
                Arrow arrowComponent = prefab.GetComponent<Arrow>();
                GhostArrow ghostArrowComponent = prefab.AddComponent<GhostArrow>();
                ComponentExtensions.CopyFields(arrowComponent, ghostArrowComponent);

                var trailRenderer = prefab.GetComponentInChildren<TrailRenderer>();
                int colorType = i % 3;
                if (colorType == 0)
                {
                    trailRenderer.startColor = Color.red;
                    trailRenderer.endColor = Color.red;
                } else if (colorType == 1)
                {
                    trailRenderer.startColor = Color.green;
                    trailRenderer.endColor = Color.green;
                }
                else if (colorType == 2)
                {
                    trailRenderer.startColor = Color.blue;
                    trailRenderer.endColor = Color.blue;
                }
                ItemMotor motor = prefab.GetComponent<ItemMotor>();
                motor.velocity = Camera.main.transform.forward * (this.force + this.attributes[InvStat.Identifier.BowForce].maxValue * 0.25f + i * 2);
            }
        }

        // Token: 0x06000B25 RID: 2853 RVA: 0x0005C3E0 File Offset: 0x0005A7E0
        private void UpdateCount()
        {
            this.gameItem = ((this.leftHand == null) ? null : this.leftHand.item);
            if (this.gameItem != null)
            {
                if (this.gameItem.count <= 0)
                {
                    this.leftHand.item = null;
                }
                this.label.text = this.arrowCount.ToString();
                this.label.enabled = true;
            }
            else
            {
                this.label.enabled = false;
            }
        }

        // Token: 0x06000B26 RID: 2854 RVA: 0x0005C473 File Offset: 0x0005A873
        private void OnCountChanged(ItemSlot slot)
        {
            this.UpdateCount();
        }

        // Token: 0x06000B27 RID: 2855 RVA: 0x0005C47C File Offset: 0x0005A87C
        private void OnArrowChanged()
        {
            if (this.leftHand != null)
            {
                this.leftHand.OnItemChange -= this.OnCountChanged;
            }
            this.leftHand = this.equipment.leftHand;
            this.UpdateCount();
            if (this.gameItem != null)
            {
                this.leftHand.OnItemChange += this.OnCountChanged;
            }
        }

        // Token: 0x04000CF7 RID: 3319
        public GameObject bowObject;

        // Token: 0x04000CF8 RID: 3320
        public GameObject regularArrow;

        // Token: 0x04000CF9 RID: 3321
        public GameObject fireArrow;

        // Token: 0x04000CFA RID: 3322
        public GameObject magicArrow;

        // Token: 0x04000CFB RID: 3323
        public GameObject poisonedArrow;

        // Token: 0x04000CFC RID: 3324
        public GameObject explosiveArrow;

        // Token: 0x04000CFD RID: 3325
        public GameObject teleportationArrow;

        // Token: 0x04000CFE RID: 3326
        public PlayerItemStorage playerItemStorage;

        // Token: 0x04000CFF RID: 3327
        public PlayerItemStorage playerQuickSlotStorage;

        // Token: 0x04000D00 RID: 3328
        public float force;

        // Token: 0x04000D01 RID: 3329
        private float attackTimer;

        // Token: 0x04000D02 RID: 3330
        public float coolDown;

        // Token: 0x04000D03 RID: 3331
        public UILabel label;

        // Token: 0x04000D04 RID: 3332
        public AudioClip[] sound;

        // Token: 0x04000D05 RID: 3333
        public GameObject soundSource;

        // Token: 0x04000D06 RID: 3334
        public Cooldown indicator;

        // Token: 0x04000D07 RID: 3335
        public string iconName;

        // Token: 0x04000D08 RID: 3336
        private AudioSource fireSound;

        // Token: 0x04000D09 RID: 3337
        private ArrowType arrowType;

        // Token: 0x04000D0A RID: 3338
        private GameObject arrow;

        // Token: 0x04000D0B RID: 3339
        private InvGameItem _gameItem;

        // Token: 0x04000D0C RID: 3340
        [SerializeField]
        private InvEquipment equipment;

        // Token: 0x04000D0D RID: 3341
        [SerializeField]
        private CharacterAttributeList attributes;

        // Token: 0x04000D0E RID: 3342
        [SerializeField]
        private InventoryCollector inventory;

        // Token: 0x04000D0F RID: 3343
        public string animationName;

        // Token: 0x04000D10 RID: 3344
        private AssignArrow arrowBow;

        // Token: 0x04000D11 RID: 3345
        private Animation animation;

        // Token: 0x04000D12 RID: 3346
        private bool reloaded = true;

        // Token: 0x04000D13 RID: 3347
        private ItemSlot leftHand;
    }
}