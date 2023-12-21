using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AmazingAssets.AdvancedDissolve;
using DG.Tweening;
using Photon.Pun;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace MobaVR
{
    public partial class Monster : MonoBehaviourPunCallbacks,
        IMonsterAnimatorListener,
        IExploding
    {
        #region Dependencies

        [Header("Dependencies")]
        //[SerializeField] private Transform m_Target;
        [SerializeField]
        protected MonsterPelvis m_MonsterPelvis;

        [SerializeField] protected MonsterView m_MonsterView;
        [SerializeField] protected DamageNumberView m_DamageNumber;
        [SerializeField] protected MonsterWeapon m_Weapon;

        [SerializeField] protected Treasure m_Treasure;

        //[SerializeField] protected Material m_Material;
        [SerializeField] protected List<Material> m_UniqMaterials;

        [Header("Targets")] [SerializeField] protected TargetType m_TargetType = TargetType.PLAYER;
        [SerializeField] protected WizardPlayer m_Wizard;
        [SerializeField] protected Tower m_Tower;

        #endregion

        #region Components

        [SerializeField] private Transform m_Root;
        [SerializeField] private Collider m_BodyCollider;
        [SerializeField] private Animator m_Animator;
        [SerializeField] private Rigidbody m_Rigidbody;
        [SerializeField] private NavMeshAgent m_NavMeshAgent;
        [SerializeField] private NavMeshObstacle m_NavMeshObstacle;

        #endregion

        #region Effects

        [SerializeField] private FogEnemyEffect m_BlindEffect;
        [SerializeField] private float m_BlindDuration = 3f;

        #endregion

        #region Points

        [Space] [Header("Stats")] [SerializeField] [ReadOnly]
        private MonsterState m_CurrentState = MonsterState.NOT_ACTIVE;

        [SerializeField] private float m_Health = 100f;
        [SerializeField] private float m_Damage = 20f;
        [SerializeField] private float m_RotationSpeed = 45f;
        [SerializeField] [ReadOnly] private float m_CurrentHealth = 100f;
        [SerializeField] [ReadOnly] private float m_CurrentForwardSpeed = 0f;
        [SerializeField] [ReadOnly] private bool m_IsBlind = false;
        [SerializeField] private float m_AttackRange = 4.0f;
        [SerializeField] private float m_AttackCooldown = 0.5f;
        [SerializeField] private float m_KForce = 2f;
        [SerializeField] private float m_DestroyTime = 5f;
        private float m_ReceiveDamageDelay = 0.2f;

        #endregion

        #region Animations

        private readonly int m_HashForwardSpeed = Animator.StringToHash("ForwardSpeed");
        private readonly int m_HashActivate = Animator.StringToHash("Activate");
        private readonly int m_HashFastActivate = Animator.StringToHash("FastActivate");
        private readonly int m_HashIdle = Animator.StringToHash("Idle");
        private readonly int m_HashTaunt = Animator.StringToHash("Taunt");
        private readonly int m_HashAttack = Animator.StringToHash("Attack");
        private readonly int m_HashHurt = Animator.StringToHash("Hit");
        private readonly int m_HashDeath = Animator.StringToHash("Die");
        private readonly int m_Stun = Animator.StringToHash("Stun");
        private readonly int m_Ready = Animator.StringToHash("Ready");

        #endregion

        #region Fields

        private int m_CurrentHashAttack;
        private bool m_IsReady = false;
        private bool m_CanAttack = false;
        private bool m_CanMove = false;
        private bool m_IsImmortal = false;
        private bool m_CanGetDamage = true;
        private bool m_IsAttacking = false;

        private Coroutine m_NavAgentCoroutine;

        private List<Rigidbody> m_ChildRigidbodies = new();
        private List<Collider> m_ChildColliders = new();
        private List<Renderer> m_MeshRenderers = new();
        private List<Material> m_CopyUniqMaterials = new();

        private HitData m_LastHitData = null;

        #endregion

        #region Properties

        public float Damage => m_Damage;
        public float CurrentHealth => m_CurrentHealth;
        public bool IsAttacking => m_IsAttacking;
        public bool IsLife => m_CurrentHealth > 0f;

        public TargetType TargetType
        {
            get => m_TargetType;
            set => m_TargetType = value;
        }

        public MonsterState CurrentState
        {
            get => m_CurrentState;
            set
            {
                if (!PhotonNetwork.IsMasterClient)
                {
                    return;
                }

                m_CurrentState = value;
                switch (value)
                {
                    case MonsterState.NOT_ACTIVE:
                        Init();
                        break;
                    case MonsterState.START_ACTIVATION:
                        StartActivation();
                        break;
                    case MonsterState.COMPLETE_ACTIVATION:
                        CompleteActivation();
                        break;
                    case MonsterState.START_ATTACK:
                        StartAttack();
                        break;
                    case MonsterState.COMPLETE_ATTACK:
                        CompleteAttack();
                        break;
                    case MonsterState.DEATH:
                        Die();
                        break;
                }
            }
        }

        #endregion

        #region Actions

        public Action OnInit;
        public Action OnMove;
        public Action OnAttack;
        public Action OnDeath; // TODO: Monster Die Action
        public Action<HitData> OnLastHit; // TODO: Monster Die Action
        public Action<HitData> OnHit;

        #endregion

        private void OnValidate()
        {
            if (m_Rigidbody == null)
            {
                TryGetComponent(out m_Rigidbody);
            }

            if (m_Animator == null)
            {
                TryGetComponent(out m_Animator);
            }

            if (m_NavMeshAgent == null)
            {
                TryGetComponent(out m_NavMeshAgent);
            }

            if (m_NavMeshObstacle == null)
            {
                TryGetComponent(out m_NavMeshObstacle);
            }

            if (m_BlindEffect == null)
            {
                m_BlindEffect = GetComponentInChildren<FogEnemyEffect>();
            }

            /*
            if (m_Material == null)
            {
                Renderer meshRenderer = GetComponentInChildren<Renderer>();
                if (meshRenderer != null)
                {
                    m_Material = meshRenderer.material;
                }
            }
            */

            if (m_UniqMaterials != null && m_UniqMaterials.Count == 0)
            {
                InitUniqMaterials();
            }
        }

        //TODO
        public override void OnDisable()
        {
            base.OnDisable();
            if (m_Weapon != null)
            {
                m_Weapon.OnTrigger -= HitWizard;
            }
        }

        //TODO
        public override void OnEnable()
        {
            base.OnEnable();
            if (m_Weapon != null)
            {
                m_Weapon.OnTrigger += HitWizard;
            }
        }

        protected virtual void Awake()
        {
            m_CurrentHealth = m_Health;
            m_CurrentForwardSpeed = 0f;

            m_ChildRigidbodies.AddRange(m_Root.GetComponentsInChildren<Rigidbody>());
            m_ChildColliders.AddRange(m_Root.GetComponentsInChildren<Collider>());

            /*
            foreach (Rigidbody rigidbody in m_ChildRigidbodies)
            {
                Collider childCollider = rigidbody.GetComponent<Collider>();
                m_ChildColliders.Add(childCollider);
            }
            */

            InitRenderer();

            ToggleRagDolls(false);
            Appear();
        }

        protected virtual void Start()
        {
            CurrentState = MonsterState.NOT_ACTIVE;
            CurrentState = MonsterState.START_ACTIVATION;

            //photonView.
        }

        protected virtual void Update()
        {
            //ResetTriggers();

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            if (m_TargetType == TargetType.PLAYER)
            {
                if (m_Wizard == null ||
                    (m_Wizard != null && !m_Wizard.IsLife))
                {
                    FindPlayerTarget();
                    DeactivateNavAgent();
                    return;
                }
            }

            if (m_TargetType == TargetType.TOWER)
            {
                if (m_Tower == null ||
                    (m_Tower != null && !m_Tower.IsLife))
                {
                    FindTowerTarget();
                    DeactivateNavAgent();
                    return;
                }
            }

            if (IsLife
                && !m_IsBlind
                && CurrentState != MonsterState.NOT_ACTIVE
                && CurrentState != MonsterState.DEATH)
            {
                //float distance = Vector3.Distance(m_Wizard.transform.position, transform.position);

                float distance = 1000f;
                if (m_TargetType == TargetType.PLAYER)
                {
                    distance = Vector3.Distance(m_Wizard.PointPlayer.position, transform.position);
                }

                if (m_TargetType == TargetType.TOWER)
                {
                    distance = Vector3.Distance(m_Tower.transform.position, transform.position);
                }

                if (distance < m_AttackRange)
                {
                    if (m_CanAttack)
                    {
                        m_Animator.SetTrigger(m_CurrentHashAttack);

                        m_IsAttacking = true;
                        m_CanAttack = false;
                        m_CanMove = false;

                        DeactivateNavAgent();
                    }
                }

                if (m_CanMove)
                {
                    Move();
                }
                else
                {
                    DeactivateNavAgent();
                    Rotate();
                }
            }
        }

        #region Tower Target

        public void SetTowerTarget(Tower tower)
        {
            SetTowerTarget(tower.photonView.ViewID);
        }

        public void SetTowerTarget(int idPhotonTarget)
        {
            //photonView.RPC(nameof(RpcSetTowerTarget), RpcTarget.AllBuffered, idPhotonTarget);
            photonView.RPC(nameof(RpcSetTowerTarget), RpcTarget.All, idPhotonTarget);
        }

        private void FindTowerTarget()
        {
            m_Tower = null;
            m_Tower = FindObjectOfType<Tower>();
        }

        [PunRPC]
        private void RpcSetTowerTarget(int idPhotonTarget)
        {
            PhotonView towerPhotonView = PhotonView.Find(idPhotonTarget);
            if (towerPhotonView != null)
            {
                m_Tower = towerPhotonView.GetComponent<Tower>();
            }
        }

        #endregion

        #region Wizard Target

        private void FindPlayerTarget()
        {
            m_Wizard = null;

            WizardPlayer[] players = FindObjectsOfType<WizardPlayer>();
            if (players.Length > 0)
            {
                WizardPlayer[] lifePlayers = players.Where(player => player.IsLife).ToArray();
                if (lifePlayers.Length > 0)
                {
                    WizardPlayer wizardPlayer = lifePlayers[Random.Range(0, lifePlayers.Length)];
                    m_Wizard = wizardPlayer;
                    //return wizardPlayer;
                }
            }
            //return null;
            //photonView.RPC(nameof(RpcSetWizardTarget), RpcTarget.AllBuffered, m_Wizard.photonView.ViewID);
        }

        [PunRPC]
        protected void RpcSetWizardTarget(int viewId)
        {
            WizardPlayer[] players = FindObjectsOfType<WizardPlayer>();
            m_Wizard = players.First(player => player.photonView.ViewID == viewId);
        }

        private void HitWizard(Collider other)
        {
            if (m_IsAttacking)
            {
                HitData hitData = new HitData()
                {
                    Amount = m_Damage,
                    TeamType = TeamType.OTHER,
                    PhotonOwner = photonView,
                    PhotonView = photonView
                };

                /*
                if (other.TryGetComponent(out WizardPlayer wizardPlayer))
                {
                    //wizardPlayer.Hit(Damage);
                    wizardPlayer.Hit(hitData);
                    m_Weapon.SetEnabled(false);
                }
                */

                //if (other.CompareTag("LifeCollider") && other.transform.TryGetComponent(out Damageable damageable))
                if (other.transform.TryGetComponent(out Damageable damageable))
                {
                    damageable.Hit(hitData);
                    m_Weapon.SetEnabled(false);
                }

                if (other.TryGetComponent(out HitCollider damagePlayer))
                {
                    //damagePlayer.WizardPlayer.Hit(Damage);
                    //TODO: DAMAGE
                    //damagePlayer.WizardPlayer.Hit(hitData);
                    m_Weapon.SetEnabled(false);
                }
            }
        }

        #endregion

        #region Nav Agent

        private void DeactivateNavAgent()
        {
            if (m_NavAgentCoroutine != null)
            {
                StopCoroutine(m_NavAgentCoroutine);
                m_NavAgentCoroutine = null;
            }

            if (m_NavMeshAgent.enabled)
            {
                m_NavMeshAgent.velocity = Vector3.zero;
                m_NavMeshAgent.isStopped = true;
                m_NavMeshAgent.enabled = false;
                m_NavMeshObstacle.enabled = true;
            }
        }

        private void ActivateNavAgent()
        {
            if (m_NavAgentCoroutine == null)
            {
                m_NavAgentCoroutine = StartCoroutine(LazyActivateNavAgent());
            }
        }

        private IEnumerator LazyActivateNavAgent()
        {
            m_NavMeshObstacle.enabled = false;
            yield return null;
            m_NavMeshAgent.enabled = true;
        }

        #endregion

        #region Init

        protected virtual void Init()
        {
            m_CurrentHealth = m_Health;
            m_CurrentForwardSpeed = 0f;

            m_CanMove = false;
            m_IsAttacking = false;
            m_CanAttack = false;
            m_IsImmortal = true;

            m_MonsterPelvis.SetEnabled(false);

            m_Animator.SetTrigger(m_HashIdle);
            m_CurrentHashAttack = m_HashAttack;

            if (m_NavMeshAgent != null && m_TargetType == TargetType.PLAYER && m_Wizard != null)
            {
                m_NavMeshAgent.updateRotation = true;
                m_NavMeshAgent.stoppingDistance = m_AttackRange;
                //m_NavMeshAgent.destination = m_Wizard.transform.position;
                m_NavMeshAgent.destination = m_Wizard.PointPlayer.transform.position;

                Vector3 targetPosition = new Vector3(
                    m_Wizard.PointPlayer.transform.position.x,
                    transform.position.y,
                    m_Wizard.PointPlayer.transform.position.z);

                transform.LookAt(targetPosition);
            }

            if (m_NavMeshAgent != null && m_TargetType == TargetType.TOWER && m_Tower != null)
            {
                m_NavMeshAgent.updateRotation = true;
                m_NavMeshAgent.stoppingDistance = m_AttackRange;
                m_NavMeshAgent.destination = m_Tower.transform.position;

                Vector3 targetPosition = new Vector3(
                    m_Tower.transform.position.x,
                    transform.position.y,
                    m_Tower.transform.position.z);

                transform.LookAt(targetPosition);
            }

            if (m_NavMeshObstacle != null)
            {
                m_NavMeshObstacle.enabled = false;
                m_NavMeshObstacle.carveOnlyStationary = false;
                m_NavMeshObstacle.carving = true;
            }

            OnInit?.Invoke();
        }

        public virtual void Activate()
        {
            CurrentState = MonsterState.START_ACTIVATION;
        }

        public virtual void StartActivation()
        {
            //m_OrcSoundController.PlayTaunt();
            m_MonsterView.SetEnabled(true);
            m_Animator.SetTrigger(m_HashActivate);
        }

        public virtual void CompleteActivation()
        {
            m_IsReady = true;
            m_IsAttacking = false;
            m_CanAttack = true;
            m_IsImmortal = false;
            m_CanMove = true;
            m_CurrentForwardSpeed = 0f;
            m_Animator.SetFloat(m_HashForwardSpeed, m_CurrentForwardSpeed);

            //ActivateNavAgent();

            //photonView.RPC(nameof(RpcCompleteActivate), RpcTarget.OthersBuffered);
            photonView.RPC(nameof(RpcCompleteActivate), RpcTarget.All);
        }

        [PunRPC]
        protected void RpcCompleteActivate()
        {
            m_IsReady = true;
            m_Animator.SetTrigger(m_HashFastActivate);
        }

        #endregion

        #region Atack

        public virtual void StartAttack()
        {
            m_Weapon.SetEnabled(true);

            m_CurrentForwardSpeed = 0f;
            m_Animator.SetFloat(m_HashForwardSpeed, m_CurrentForwardSpeed);
            //m_Weapon.StartAttack();
        }

        public virtual void CompleteAttack()
        {
            //m_Weapon.EndAttack();
            m_Weapon.SetEnabled(false);

            m_Animator.ResetTrigger(m_CurrentHashAttack);
            m_IsAttacking = false;

            //ReadyToAttack();

            m_CanMove = false;
            m_CurrentForwardSpeed = 0f;
            m_Animator.SetFloat(m_HashForwardSpeed, m_CurrentForwardSpeed);

            StartCoroutine(ReloadAttack());
        }

        private IEnumerator ReloadAttack()
        {
            m_CanAttack = false;
            //m_CanMove = true;
            //yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(m_AttackCooldown);
            m_CanAttack = true;

            if (IsLife)
            {
                ReadyToAttack();
            }
        }

        private void ReadyToAttack()
        {
            m_IsAttacking = false;
            m_CanAttack = true;
            m_IsImmortal = false;
            m_CanMove = true;

            m_CurrentForwardSpeed = 0f;
            m_Animator.SetFloat(m_HashForwardSpeed, m_CurrentForwardSpeed);

            //ActivateNavAgent();
        }

        #endregion

        #region Animation

        private void ResetTriggers()
        {
        }

        #endregion

        #region Move

        private void Move()
        {
            if (!m_NavMeshAgent.enabled)
            {
                ActivateNavAgent();
                return;
            }

            m_NavMeshAgent.isStopped = false;
            //m_NavMeshAgent.destination = m_Wizard.transform.position;
            if (m_TargetType == TargetType.PLAYER)
            {
                m_NavMeshAgent.destination = m_Wizard.PointPlayer.transform.position;
            }

            if (m_TargetType == TargetType.TOWER)
            {
                m_NavMeshAgent.destination = m_Tower.transform.position;
            }

            //m_CurrentForwardSpeed = m_NavMeshAgent.velocity.magnitude / m_NavMeshAgent.speed;
            m_CurrentForwardSpeed = m_NavMeshAgent.velocity.magnitude;
            m_Animator.SetFloat(m_HashForwardSpeed, m_CurrentForwardSpeed);
        }

        private void Rotate()
        {
            //Vector3 targetPosition = m_Wizard.transform.position;
            Vector3 targetPosition = Vector3.zero;

            if (m_TargetType == TargetType.PLAYER)
            {
                targetPosition = m_Wizard.PointPlayer.transform.position;
            }

            if (m_TargetType == TargetType.TOWER)
            {
                targetPosition = m_Tower.transform.position;
            }

            targetPosition.y = 0f;

            Vector3 currentPosition = transform.position;
            currentPosition.y = 0;

            Vector3 targetDirection = targetPosition - currentPosition;

            float singleStep = m_RotationSpeed * Time.deltaTime;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        #endregion

        private void ToggleRagDolls(bool useRagDoll)
        {
            //m_BodyCollider.isTrigger = useRagDoll;
            m_Animator.enabled = !useRagDoll;

            foreach (Rigidbody childRigidbody in m_ChildRigidbodies)
            {
                childRigidbody.isKinematic = !useRagDoll;
                childRigidbody.useGravity = useRagDoll;
            }

            foreach (Collider childCollider in m_ChildColliders)
            {
                childCollider.enabled = useRagDoll;
            }
        }

        #region Hit

        public void Blind()
        {
            //TODO
            photonView.RPC(nameof(RpcBlind), RpcTarget.All);
        }

        [PunRPC]
        protected void RpcBlind()
        {
            if (!m_IsReady)
            {
               return;
            }

            if (m_BlindEffect != null && m_BlindDuration > 0)
            {
                m_BlindEffect.Show(m_BlindDuration);
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }

            DeactivateNavAgent();
            m_IsBlind = true;

            m_Animator.SetTrigger(m_Stun);

            CancelInvoke(nameof(ResetBlind));
            Invoke(nameof(ResetBlind), m_BlindDuration);
        }

        private void ResetBlind()
        {
            m_Animator.SetTrigger(m_Ready);

            m_IsBlind = false;
            ActivateNavAgent();
        }

        public void Hit(HitData hitData)
        {
            //RpcHit_Monster(hitData.Amount);
            OnHit?.Invoke(hitData);
            RpcHit_Monster(hitData);
        }

        /*
                public void RpcHit(float damage)
                {
                    photonView.RPC(nameof(RpcRpcHit), RpcTarget.All, damage);
                }
        */
        [PunRPC]
        //protected void RpcHit_Monster(float damage)
        protected void RpcHit_Monster(HitData hitData)
        {
            if (IsLife)
            {
                //Vector3 position = m_MonsterView.transform.position;
                Vector3 position = m_DamageNumber.transform.position;
                hitData.Position = position;
                //GetDamage(position, hitData.Amount);
                GetDamage(hitData);
            }
        }

        public void Die()
        {
            photonView.RPC(nameof(RpcDie_Monster), RpcTarget.All, m_LastHitData);
        }

        [PunRPC]
        protected void RpcDie_Monster(HitData hitData)
        {
            //Destroy(m_Rigidbody);
            ToggleRagDolls(true);
            Die(true, hitData);
        }

        public void Explode(float explosionForce, Vector3 position, float radius, float modifier)
        {
            if (!IsLife)
            {
                /*
                foreach (var childRigidbody in m_ChildRigidbodies)
                {
                    childRigidbody.AddExplosionForce(explosionForce * m_KForce,
                                                     position,
                                                     radius,
                                                     modifier);
                }
                */

                /*
                m_Rigidbody.AddExplosionForce(explosionForce,
                                              position,
                                              radius,
                                              modifier);
                */

                photonView.RPC(nameof(RpcExplode_Monster), RpcTarget.All, explosionForce, position, radius, modifier);
            }
        }

        [PunRPC]
        protected void RpcExplode_Monster(float explosionForce, Vector3 position, float radius, float modifier)
        {
            foreach (Rigidbody childRigidbody in m_ChildRigidbodies)
            {
                //if (!m_Rigidbody.isKinematic)
                {
                    childRigidbody.AddExplosionForce(explosionForce * m_KForce,
                        //childRigidbody.AddExplosionForce(explosionForce * 1f,
                        position,
                        radius,
                        modifier);
                }
            }

            /*
            if (m_Rigidbody != null && !m_Rigidbody.isKinematic)
            {
                m_Rigidbody.AddExplosionForce(explosionForce,
                                              position,
                                              radius,
                                              modifier);
            }
            */
        }

        //protected virtual void GetDamage(Vector3 position, float damage)
        protected virtual void GetDamage(HitData hitData)
        {
            if (!IsLife || !m_CanGetDamage)
            {
                return;
            }

            if (!m_IsImmortal)
            {
                if (m_CurrentHealth >= 0)
                {
                    m_CurrentHealth -= hitData.Amount;
                    m_DamageNumber.SpawnNumber(hitData.Position, hitData.Amount, MonsterDamageType.HP);
                    //m_OrcSoundController.PlayHit();
                    //CreateBlood(position);

                    if (m_CurrentHealth <= 0)
                    {
                        m_CurrentHealth = 0;

                        if (PhotonNetwork.IsMasterClient)
                        {
                            m_LastHitData = hitData;
                        }

                        //OnLastHit?.Invoke(hitData);
                        CurrentState = MonsterState.DEATH;
                    }
                    else
                    {
                        //m_Animator.SetTrigger(m_HashHurt);
                        StartCoroutine(EnableReceiveDamage());
                    }

                    UpdateHealth();
                    return;
                }
            }
            else
            {
                m_DamageNumber.SpawnNumber(hitData.Position, hitData.Amount, MonsterDamageType.IMMORTAL);
            }
        }

        protected virtual void Die(bool isGiveTreasure, HitData hitData)
        {
            m_CurrentHealth = 0f;
            m_CanMove = false;
            m_NavMeshAgent.enabled = false;
            m_NavMeshObstacle.enabled = false;
            m_Animator.enabled = false;

            m_BodyCollider.enabled = false;
            m_MonsterPelvis.SetEnabled(true);
            m_Weapon.SetEnabled(false);

            //m_Animator.SetTrigger(m_HashDeath);

            m_MonsterView.SetEnabled(false);
            OnDeath?.Invoke();
            if (hitData != null)
            {
                OnLastHit?.Invoke(hitData);
            }

            foreach (Collider childCollider in transform.GetComponentsInChildren<Collider>())
            {
                //childCollider.enabled = false;
            }

            if (isGiveTreasure && m_Treasure != null)
            {
                m_Treasure.Generate(transform.position);
                //StartCoroutine(ShowTreasures());
            }

            //Destroy(gameObject, m_DestroyTime);
            Invoke(nameof(Dissolve), m_DestroyTime);
        }

        protected virtual void UpdateHealth()
        {
            float healthBarValue = m_CurrentHealth * 100 / m_Health;
            m_MonsterView.UpdateHealth(healthBarValue);
        }

        private IEnumerator EnableReceiveDamage()
        {
            m_CanGetDamage = false;
            yield return new WaitForSeconds(m_ReceiveDamageDelay);
            m_Animator.ResetTrigger(m_HashHurt);
            m_CanGetDamage = true;
        }

        private IEnumerator ShowTreasures()
        {
            yield return new WaitForSeconds(0.4f);
            m_Treasure.Generate(transform.position);
        }

        protected void InitUniqMaterials()
        {
            List<Renderer> meshRenderers = new();
            meshRenderers.AddRange(GetComponentsInChildren<Renderer>());

            foreach (Renderer meshRenderer in meshRenderers)
            {
                if (!m_UniqMaterials.Contains(meshRenderer.sharedMaterial))
                {
                    m_UniqMaterials.Add(meshRenderer.sharedMaterial);
                }
            }
        }

        protected virtual void InitRenderer()
        {
            foreach (Material uniqMaterial in m_UniqMaterials)
            {
                m_CopyUniqMaterials.Add(new Material(uniqMaterial));
            }

            m_MeshRenderers.AddRange(GetComponentsInChildren<Renderer>());
            foreach (Renderer meshRenderer in m_MeshRenderers)
            {
                foreach (Material uniqMaterial in m_CopyUniqMaterials)
                {
                    if (meshRenderer.sharedMaterial.name.Equals(uniqMaterial.name))
                    {
                        meshRenderer.sharedMaterial = uniqMaterial;
                        break;
                    }
                }
            }

            /*
            foreach (Renderer meshRenderer in m_MeshRenderers)
            {
                if (!m_UniqMaterials.Contains(meshRenderer.material))
                {
                    m_UniqMaterials.Add(meshRenderer.sharedMaterial);
                }
            }
            */

            /*
            foreach (Renderer meshRenderer in m_MeshRenderers)
            {
                meshRenderer.material = m_Material;
            }
            */
        }

        protected virtual void Dissolve()
        {
            float clip = 0f;
            DOTween
                .To(() => clip, x => clip = x, 1f, 2f)
                .OnUpdate(() =>
                {
                    //Dissolve(m_Material, clip);

                    //foreach (Material material in m_UniqMaterials)
                    foreach (Material material in m_CopyUniqMaterials)
                    {
                        Dissolve(material, clip);
                    }

                    /*
                    foreach (Renderer meshRenderer in m_MeshRenderers)
                    {
                        Dissolve(meshRenderer.material, clip);
                    }
                    */
                })
                .OnComplete(() =>
                {
                    if (photonView != null && gameObject != null)
                    {
                        //Destroy(gameObject, 2f);
                        if (PhotonNetwork.IsMasterClient)
                        {
                            PhotonNetwork.Destroy(gameObject);
                        }
                    }
                });
        }

        protected virtual void Appear()
        {
            float clip = 1f;
            //Dissolve(m_Material, 1f);
            //foreach (Material material in m_UniqMaterials)
            foreach (Material material in m_CopyUniqMaterials)
            {
                Dissolve(material, 1f);
            }
            /*
            foreach (Renderer meshRenderer in m_MeshRenderers)
            {
                Dissolve(meshRenderer.material, 1f);
            }
            */

            DOTween
                .To(() => clip, x => clip = x, 0f, 2f)
                .OnUpdate(() =>
                {
                    //foreach (Material material in m_UniqMaterials)
                    foreach (Material material in m_CopyUniqMaterials)
                    {
                        Dissolve(material, clip);
                    }
                    /*
                    foreach (Renderer meshRenderer in m_MeshRenderers)
                    {
                        Dissolve(meshRenderer.material, clip);
                    }
                    */
                })
                .OnComplete(() => { });
        }

        protected void Dissolve(Material material, float clip)
        {
            AdvancedDissolveProperties.Cutout.Standard.UpdateLocalProperty(
                material, AdvancedDissolveProperties.Cutout.Standard.Property.Clip, clip);
        }

        #endregion
    }
}