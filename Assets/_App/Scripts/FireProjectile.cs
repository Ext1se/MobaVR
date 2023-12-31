﻿using UnityEngine;

namespace MobaVR
{
    public class FireProjectile : MonoBehaviour
    {
        public Rigidbody Fireball;
        public float Time = 1.0f;

        private bool CreateInstances = true;
        private Rigidbody Instance;

        void Start()
        {
            InvokeRepeating("Create", Time, Time);
        }

        void Update()
        {
            if (Instance == null)
            {
                CreateInstances = true;
            }
        }

        void Create()
        {
            if (CreateInstances)
            {
                Instance = Instantiate(Fireball, transform.position, transform.rotation);
                CreateInstances = false;
                Instance.transform.SetParent(this.transform);
            }
        }
    }
}