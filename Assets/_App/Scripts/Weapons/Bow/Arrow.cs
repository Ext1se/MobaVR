using System;
using System.Collections;
using BNG;
using UnityEngine;
using UnityEngine.Events;

namespace MobaVR.Weapons.Bow
{
    /// <summary>
    /// A Grabbable object that can stick to objects and deal damage
    /// </summary>
    public class Arrow : MonoBehaviour
    {
        private Rigidbody rb;
        private Grabbable grab;
        public bool Flying = false;
        public float ZVel = 0;
        public float TriggerDelay = 0.15f;
        public bool IsEnableCollider = true;
        public Collider ShaftCollider;
        private AudioSource impactSound;

        private float flightTime = 0f;
        private float destroyTime = 10f; // Time in seconds to destroy arrow
        private Coroutine queueDestroy;

        public BNG.Projectile ProjectileObject;

        // Get this value from the ProjectileObject
        private float arrowDamage;
        private bool isAttach = false;

        public UnityEvent<Bow> OnAttach;

        public bool IsAttach => isAttach;
        
        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            impactSound = GetComponent<AudioSource>();
            ShaftCollider = GetComponent<Collider>();
            grab = GetComponent<Grabbable>();

            if (ProjectileObject == null)
            {
                ProjectileObject = gameObject.AddComponent<BNG.Projectile>();
                ProjectileObject.Damage = 50;
                ProjectileObject.StickToObject = true;
                ProjectileObject.enabled = false;
            }

            arrowDamage = ProjectileObject.Damage;
        }

        void FixedUpdate()
        {
            bool beingHeld = grab != null && grab.BeingHeld;

            // Align arrow with velocity
            if (!beingHeld && rb != null && rb.velocity != Vector3.zero && Flying && ZVel > 0.02)
            {
                rb.rotation = Quaternion.LookRotation(rb.velocity);
            }

            ZVel = transform.InverseTransformDirection(rb.velocity).z;

            if (Flying)
            {
                flightTime += Time.fixedDeltaTime;
            }

            // Cancel Destroy if we just picked this up
            if (queueDestroy != null && grab != null && grab.BeingHeld)
            {
                StopCoroutine(queueDestroy);
            }
        }

        private Bow m_Bow;
        
        public void AttachBow(Bow bow)
        {
            m_Bow = bow;
            isAttach = true;
            OnAttach?.Invoke(bow);
        }

        public void ShootArrow(Vector3 shotForce)
        {
            flightTime = 0f;
            Flying = true;

            transform.parent = null;

            rb.isKinematic = false;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(shotForce, ForceMode.VelocityChange);
            if (m_Bow != null)
            {
                Collider[] colliders = m_Bow.GetComponents<Collider>();

                foreach (Collider bowCollider in colliders)
                {
                    //if (!bowCollider.isTrigger)
                    {
                        Physics.IgnoreCollision(bowCollider, ShaftCollider, true);
                    }
                }
            }

            //StartCoroutine(ReEnableColliderFrame());
            if (isActiveAndEnabled)
            {
                StartCoroutine(ReEnableColliderDelay());
                queueDestroy = StartCoroutine(QueueDestroy());
            }
        }

        private IEnumerator QueueDestroy()
        {
            yield return new WaitForSeconds(destroyTime);

            if (grab != null && !grab.BeingHeld && transform.parent == null)
            {
                Destroy(this.gameObject);
            }
        }

        private IEnumerator ReEnableColliderFrame()
        {
            // Wait a few frames before re-enabling collider on bow shaft
            // This prevents the arrow from shooting ourselves, the bow, etc.
            // If you want the arrow to still have physics while attached,
            // parent a collider to the arrow near the tip
            int waitFrames = 3;
            for (int x = 0; x < waitFrames; x++)
            {
                yield return new WaitForFixedUpdate();
            }

            //ShaftCollider.enabled = true;
            ShaftCollider.enabled = IsEnableCollider;
        }
        
        private IEnumerator ReEnableColliderDelay()
        {
            yield return new WaitForSeconds(TriggerDelay);
            ShaftCollider.enabled = IsEnableCollider;
        }


        private void OnCollisionEnter(Collision collision)
        {
            // Ignore parent collisions
            if (transform.parent != null && collision.transform == transform.parent)
            {
                return;
            }

            // Don't count collisions if being held
            if (grab != null && grab.BeingHeld)
            {
                return;
            }

            // Don't Count Triggers
            if (collision.collider.isTrigger)
            {
                return;
            }

            string colNameLower = collision.transform.name.ToLower();

            // Ignore other very close bows and arrows
            if (flightTime < 1 && (colNameLower.Contains("arrow") || colNameLower.Contains("bow")))
            {
                Physics.IgnoreCollision(collision.collider, ShaftCollider, true);
                return;
            }

            // ignore player collision if quick shot
            if (flightTime < 1 && collision.transform.name.ToLower().Contains("player"))
            {
                Physics.IgnoreCollision(collision.collider, ShaftCollider, true);
                return;
            }

            // Damage if possible
            float zVel = System.Math.Abs(transform.InverseTransformDirection(rb.velocity).z);
            bool doStick = true;
            if (zVel > 0.02f && !rb.isKinematic)
            {
                BngDamageable d = collision.gameObject.GetComponent<BngDamageable>();
                if (d)
                {
                    d.DealDamage(arrowDamage, collision.GetContact(0).point, collision.GetContact(0).normal, true,
                                 gameObject, collision.collider.gameObject);
                }

                // Don't stick to dead objects
                if (d != null && d.Health <= 0)
                {
                    doStick = false;
                }
            }

            // Check to stick to object
            if (!rb.isKinematic && Flying)
            {
                if (zVel > 0.02f)
                {
                    if (grab != null && grab.BeingHeld)
                    {
                        grab.DropItem(false, false);
                    }

                    if (doStick)
                    {
                        tryStickArrow(collision);
                    }

                    Flying = false;

                    playSoundInterval(2.462f, 2.68f);
                }
            }
        }

        // Attach to collider
        void tryStickArrow(Collision collision)
        {
            Rigidbody colRigid = collision.collider.GetComponent<Rigidbody>();
            transform.parent = null; // Start out with arrow being in World space

            // If the collider is static then we don't need to do anything. Just stop it.
            if (collision.gameObject.isStatic)
            {
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rb.isKinematic = true;
            }
            // Next try attaching to rigidbody via FixedJoint
            else if (colRigid != null && !colRigid.isKinematic)
            {
                FixedJoint joint = gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = colRigid;
                joint.enableCollision = false;
                joint.breakForce = float.MaxValue;
                joint.breakTorque = float.MaxValue;
            }
            else if (colRigid != null && colRigid.isKinematic && collision.transform.localScale == Vector3.one)
            {
                transform.SetParent(collision.transform);
                rb.useGravity = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeAll;
                rb.WakeUp();
            }
            // Finally, try parenting or just setting the arrow to kinematic
            else
            {
                if (collision.transform.localScale == Vector3.one)
                {
                    transform.SetParent(collision.transform);
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                }
                else
                {
                    rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                    rb.useGravity = false;
                    rb.isKinematic = true;
                }
            }
        }

        void playSoundInterval(float fromSeconds, float toSeconds)
        {
            if (impactSound)
            {
                if (impactSound.isPlaying)
                {
                    impactSound.Stop();
                }

                impactSound.time = fromSeconds;
                impactSound.pitch = Time.timeScale;
                impactSound.Play();
                impactSound.SetScheduledEndTime(AudioSettings.dspTime + (toSeconds - fromSeconds));
            }
        }
    }
}