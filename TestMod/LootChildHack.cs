using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestMod
{
    public class LootChildHack : MonoBehaviour
    {


        protected void OnDestroy()
        {
            Destroy(this.transform.parent.gameObject);
        }
    }
}
