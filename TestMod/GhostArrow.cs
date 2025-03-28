using System;
using System.Collections.Generic;
using System.Collections;
using Blocksters.MathLib;
using Blocksters.Terrain.chunk;
using Blocksters.Terrain.collider;
using UnityEngine;

namespace TestMod
{


    // Token: 0x020001C5 RID: 453
    public class GhostArrow : MonoBehaviour
    {

        public static Material ghostMaterial;
        // Token: 0x17000074 RID: 116
        // (get) Token: 0x06000B01 RID: 2817 RVA: 0x0005B524 File Offset: 0x00059924
        protected Quaternion floatingRotation
        {
            get
            {
                if (this._floatingRotation != Quaternion.identity)
                {
                    return this._floatingRotation;
                }
                Vector3 eulerAngles = base.gameObject.transform.localRotation.eulerAngles;
                this._floatingRotation = Quaternion.Euler(0f, eulerAngles.y, eulerAngles.z);
                return this._floatingRotation;
            }
        }

        protected virtual void Awake()
        {
            if (ghostMaterial == null)
            {
                StartCoroutine(LoadMaterial());
            }
        }

        protected IEnumerator LoadMaterial()
        {
            var resourceRequest = Resources.LoadAsync<Material>("models/mati/materials/eye-monster");
            yield return resourceRequest;
            ghostMaterial = resourceRequest.asset as Material;
        }

        // Token: 0x06000B02 RID: 2818 RVA: 0x0005B58C File Offset: 0x0005998C
        protected virtual void Start()
        {
            this.audio = base.GetComponent<AudioSource>();
            this.player = GameObject.FindGameObjectWithTag("Player").transform;
            this.stats = GameObject.FindWithTag("Settings").GetComponent<StatsCollector>();
            this.motor = base.gameObject.GetComponent<ItemMotor>();

            if (this.collider != null)
            {
                this.collider.enabled = false;
            }

            this.world = MasterDatabase.instance.terrain.world;
            this.GetComponentInChildren<MeshRenderer>().sharedMaterial = ghostMaterial;
        }

        // Token: 0x06000B03 RID: 2819 RVA: 0x0005B678 File Offset: 0x00059A78
        private void OnDestroy()
        {
            if (this.enemy != null)
            {
                this.enemy.OnDeath -= this.OnEnemyDeath;
            }
        }

        // Token: 0x06000B04 RID: 2820 RVA: 0x0005B6AD File Offset: 0x00059AAD
        protected virtual void OnTerrainColliderHit(TerrainColliderHit hit)
        {
            this.hitBlock = hit.block;
            base.StartCoroutine(this._HandleBlockDestroyed(hit.voxel));
        }

        // Token: 0x06000B05 RID: 2821 RVA: 0x0005B6D0 File Offset: 0x00059AD0
        protected virtual IEnumerator _HandleBlockDestroyed(Vector3i voxel)
        {
            yield return new WaitForSeconds(0.1f);
            UnityEngine.Object.Destroy(base.gameObject);
        }

        // Token: 0x06000B06 RID: 2822 RVA: 0x0005B6F4 File Offset: 0x00059AF4
        protected void ResetTarget()
        {
            base.transform.parent = null;
            this.motor.enabled = true;
            this.collider.enabled = true;
            this.enemy = null;
            if (this.trail != null)
            {
                this.trail.SetActive(true);
            }
        }

        // Token: 0x06000B07 RID: 2823 RVA: 0x0005B74C File Offset: 0x00059B4C
        private void OnTriggerEnter(Collider collision)
        {
            Transform transform = collision.transform;
            if (transform.tag == "Player" || transform.tag == "Pet" || transform.tag == "Inventory")
            {
                return;
            }
            if (!this.motor.enabled)
            {
                return;
            }
            GameObject gameObject = transform.gameObject;
            Mount component = gameObject.GetComponent<Mount>();
            if (component != null && component.mounted)
            {
                return;
            }
            if (!this.didHit && (gameObject.CompareTag("Enemy") || gameObject.CompareTag("NPC") || gameObject.CompareTag("Animal") || gameObject.CompareTag("Pet")))
            {
                this.enemy = transform.GetComponent<Health>();
                this.StartHitEnemy(gameObject, transform);
                if (!this.enemy.IsDead())
                {
                    DestroySelf();
                    this.didHit = true;
                }
            }
        }

        // Token: 0x06000B08 RID: 2824 RVA: 0x0005B918 File Offset: 0x00059D18
        private void SetCount(int count)
        {
            this.count = count;
            if (this.countlabel != null)
            {
                this.countlabel.text = ((count <= 1) ? string.Empty : count.ToString());
            }
        }

        // Token: 0x06000B09 RID: 2825 RVA: 0x0005B966 File Offset: 0x00059D66
        public void Kill()
        {
            if (this.enemy != null)
            {
                this.enemy.OnDeath -= this.OnEnemyDeath;
            }
            UnityEngine.Object.Destroy(base.gameObject);
        }

        // Token: 0x06000B0A RID: 2826 RVA: 0x0005B99B File Offset: 0x00059D9B
        private void OnEnemyDeath()
        {
            if (UnityEngine.Random.Range(0, 100) > 79)
            {
                this.ResetTarget();
            }
            else
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }

        // Token: 0x06000B0B RID: 2827 RVA: 0x0005B9C4 File Offset: 0x00059DC4
        protected virtual void Update()
        {
            if (Inventory.isPaused)
            {
                return;
            }
            this.killTime += Time.deltaTime;
            if (this.killTime >= this.timeBeforeDestroy)
            {
                this.Kill();
            }
            if (!this.motor.grounded && !this.touchedGround && this.enemy == null)
            {
                float sqrMagnitude = this.motor.velocity.sqrMagnitude;
                if (sqrMagnitude > 5f)
                {
                    Quaternion localRotation = Quaternion.LookRotation(this.motor.velocity);
                    base.transform.localRotation = localRotation;
                }
                else if (sqrMagnitude > 0f)
                {
                    Quaternion localRotation2 = Quaternion.Slerp(base.transform.localRotation, this.floatingRotation, Time.deltaTime);
                    base.transform.localRotation = localRotation2;
                }
            }
            if (!this.touchedGround && ((!this.motor.enabled && this.enemy == null) || this.motor.grounded))
            {
                this.touchedGround = true;
                base.GetComponent<BoxCollider>().enabled = false;
                if (!this.motor.inWater)
                {
                    this.StartHitGround(base.gameObject, base.transform);
                }
                this.DestroySelf();
                this.ActivateButton();
            }
        }

        // Token: 0x06000B0C RID: 2828 RVA: 0x0005BB69 File Offset: 0x00059F69
        public void DestroySelf()
        {
            base.StartCoroutine(this.DestroySelfT());
        }

        // Token: 0x06000B0D RID: 2829 RVA: 0x0005BB78 File Offset: 0x00059F78
        private IEnumerator DestroySelfT()
        {
            yield return new WaitForEndOfFrame();
            UnityEngine.Object.Destroy(base.gameObject);
            yield break;
        }

        // Token: 0x06000B0E RID: 2830 RVA: 0x0005BB94 File Offset: 0x00059F94
        protected virtual void StartHitEnemy(GameObject target, Transform location)
        {

            AudioSource.PlayClipAtPoint(this.audio.clip, base.transform.position);
            try
            {
                this.item.ApplyEffects(this.player.gameObject, target);
                UnityEngine.Object.Instantiate<GameObject>(this.hitEffect, location.position, location.rotation);
            } 
            catch (Exception) 
            { 
                DestroySelf();
            }
        }

        // Token: 0x06000B0F RID: 2831 RVA: 0x0005BBEB File Offset: 0x00059FEB
        protected virtual void StartHitGround(GameObject target, Transform location)
        {
            AudioSource.PlayClipAtPoint(this.audio.clip, base.transform.position);
        }

        // Token: 0x06000B10 RID: 2832 RVA: 0x0005BC08 File Offset: 0x0005A008
        public void ActivateButton()
        {
            Collider[] array = Physics.OverlapSphere(base.transform.position, 0.5f);
            foreach (Collider collider in array)
            {
                if (collider.CompareTag("Button"))
                {
                    collider.gameObject.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
                }
            }
        }

        // Token: 0x04000CD0 RID: 3280
        public static float collectSpeed = 10f;

        // Token: 0x04000CD1 RID: 3281
        protected static Dictionary<string, InvBaseItem> items = new Dictionary<string, InvBaseItem>();

        // Token: 0x04000CD2 RID: 3282
        [NonSerialized]
        public InvBaseItem item;

        // Token: 0x04000CD3 RID: 3283
        public new string name;

        // Token: 0x04000CD4 RID: 3284
        public float minDistance;

        // Token: 0x04000CD5 RID: 3285
        public GameObject hitEffect;

        // Token: 0x04000CD6 RID: 3286
        public float timeBeforeDestroy = 3f;

        // Token: 0x04000CD7 RID: 3287
        public SphereCollider collider;

        // Token: 0x04000CD8 RID: 3288
        public int count = 1;

        // Token: 0x04000CD9 RID: 3289
        public UILabel countlabel;

        // Token: 0x04000CDA RID: 3290
        public GameObject trail;

        // Token: 0x04000CDB RID: 3291
        public bool attach = true;

        // Token: 0x04000CDC RID: 3292
        protected IWorld world;

        // Token: 0x04000CDD RID: 3293
        protected Block hitBlock;

        // Token: 0x04000CDE RID: 3294
        protected ItemMotor motor;

        // Token: 0x04000CDF RID: 3295
        protected bool touchedGround;

        // Token: 0x04000CE0 RID: 3296
        protected StatsCollector stats;

        // Token: 0x04000CE1 RID: 3297
        protected Transform player;

        // Token: 0x04000CE2 RID: 3298
        protected bool didHit;

        // Token: 0x04000CE3 RID: 3299
        protected HealthBase enemy;

        // Token: 0x04000CE4 RID: 3300
        protected Transform collectTarget;

        // Token: 0x04000CE5 RID: 3301
        protected InventoryCollector inventory;

        // Token: 0x04000CE6 RID: 3302
        private float killTime;

        // Token: 0x04000CE7 RID: 3303
        protected Quaternion _floatingRotation = Quaternion.identity;

        // Token: 0x04000CE8 RID: 3304
        protected AudioSource audio;

        // Token: 0x04000CE9 RID: 3305
        protected static AudioSource lootSound;
    }

}
