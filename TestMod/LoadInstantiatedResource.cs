using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace TestMod
{
    public class LoadInstantiatedResource : AutoLoadResource
    {
        protected Vector3 defaultPos;
        protected Quaternion defaultRot;
        protected Vector3 defaultScl;

        public void AssignInstantiatedResource(GameObject resource)
        {
            var eventField = typeof(AutoLoadResource).GetField("_child", BindingFlags.Instance | BindingFlags.NonPublic);
            GameObject child = eventField.GetValue(this) as GameObject;

            child = resource;

            defaultPos = child.transform.localPosition;
            defaultRot = child.transform.localRotation;
            defaultScl = child.transform.localScale;
            child.transform.parent = base.transform;
            child.transform.localPosition = defaultPos;
            child.transform.localRotation = defaultRot;
            child.transform.localScale = defaultScl;

            eventField.SetValue(this, child);
        }

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
            if (this.child == null)
            {
                Debug.Log("Child is NULL!");
            } else
            {
                this.child.SetActiveRecursively(true);
                child.transform.localPosition = defaultPos;
                child.transform.localRotation = defaultRot;
                child.transform.localScale = defaultScl;
            }
            InvokeOnResourceLoaded();
        }

        protected void OnDisable()
        {
            Debug.Log($"Disabling {name}");
        }

        // Token: 0x06000477 RID: 1143 RVA: 0x000286F5 File Offset: 0x00026AF5
        protected void OnDestroy()
        {
            if (this.child != null)
            {
                UnityEngine.Object.Destroy(this.child);
            }
        }
    }
}
