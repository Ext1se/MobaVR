using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace MobaVR
{
    public class MonsterPointSpawner : MonoBehaviourPunCallbacks
    {
        [HideInInspector] public List<Monster> MonsterControllers = new List<Monster>();
        [SerializeField] private List<Monster> m_MonsterPrefabs = new List<Monster>();
        [SerializeField] private List<string> m_MonsterPaths = new List<string>();
        [SerializeField] private ParticleSystem m_CreatedEffect;
        [SerializeField] protected TargetType m_TargetType = TargetType.PLAYER;

        public float StartDelay = 0;
        public int MaxTotalCountMonster = -1;
        public int MaxCountMonster = -1;
        public float DelayBetweenMonster = 10f;
        public float DelayBetweenMaxMonster = 4f;
        public bool CanSpawn = false;

        private int m_CurrentTotalCount = 0;

        public Action<Monster> OnMonsterInit;  
        public Action<Monster> OnMonsterDie;
        public Action OnCountLimit;
        public Action OnTotalLimit;
        public Action<bool> OnStateChange;
        
        public AudioClip[] sounds;  // Массив звуков, которые вы хотите воспроизводить
        private AudioSource audioSource;
        
        public TargetType TargetType
        {
            get => m_TargetType;
            set => m_TargetType = value;
        }

        private void OnValidate()
        {
            if (m_CreatedEffect == null)
            {
                m_CreatedEffect = GetComponentInChildren<ParticleSystem>();
            }
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            //RpcGenerateMonsters();
            m_CurrentTotalCount = 0;
        }

        public void GenerateMonsters()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            
            //photonView.RPC(nameof(RpcGenerateMonsters), RpcTarget.AllBuffered);
            photonView.RPC(nameof(RpcGenerateMonsters), RpcTarget.All);
        }

        [PunRPC]
        public void RpcGenerateMonsters()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            
            if (PhotonNetwork.IsMasterClient)
            {
                //ClearMonsters();

                CanSpawn = true;
                //StartCoroutine(GenerateMonsterWave(0f));
                StartCoroutine(GenerateMonsterWave(StartDelay));
            }
        }

        //public void ClearMonsters(bool isDie = false)
        public void ClearMonsters(bool isDie = true)
        {
            m_CurrentTotalCount = 0;
            
            if (!gameObject.activeSelf)
            {
                return;
            }

            if (photonView.ViewID <= 0)
            {
                return;
            }
            
            //photonView.RPC(nameof(RpcClearMonsters), RpcTarget.AllBuffered, isDie);
            photonView.RPC(nameof(RpcClearMonsters), RpcTarget.All, isDie);
        }

        [PunRPC]
        private void RpcClearMonsters(bool isDie)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            m_CurrentTotalCount = 0;
            CanSpawn = false;

            StopAllCoroutines();

            //Monster[] monsters = GetComponentsInChildren<Monster>();
            //foreach (Monster monster in monsters)

            while (MonsterControllers.Count != 0)
            {
                Monster monster = MonsterControllers[0];
                if (isDie)
                {
                    monster.Die();
                }
                else
                {
                    //Destroy(monster.gameObject);
                    PhotonNetwork.Destroy(monster.gameObject);
                }
                
                if (monster != null)
                {
                    MonsterControllers.Remove(monster);
                }
            }
            
            /*
            foreach (Monster monster in MonsterControllers)
            {
                if (isDie)
                {
                    monster.Die();
                }
                else
                {
                    Destroy(monster.gameObject);
                }
            }
            */

            MonsterControllers.Clear();
        }

        public bool TryGenerateMonster()
        {
            /*
            if (MaxCountMonster == -1 || MaxTotalCountMonster == -1)
            {
                SpawnMonster();
                return true;
            }
            */

            if (MaxTotalCountMonster != -1 && m_CurrentTotalCount >= MaxTotalCountMonster)
            {
                return false;
            }

            if (MaxCountMonster != -1 && MonsterControllers.Count >= MaxCountMonster)
            {
                return false;
            }
            
            SpawnMonster();
            return true;
            
            /*
            if (MaxCountMonster == -1 || MonsterControllers.Count < MaxCountMonster)
            {
                SpawnMonster();
                return true;
            }
            else
            {
                return false;
            }
            */
        }

        private IEnumerator GenerateMonsterWave(float delayBetweenMonster)
        {
            yield return new WaitForSeconds(delayBetweenMonster);
            if (!CanSpawn)
            {
                yield break;
            }

            if (TryGenerateMonster())
            {
                StartCoroutine(GenerateMonsterWave(DelayBetweenMonster));
            }
            else
            {
                StartCoroutine(GenerateMonsterWave(DelayBetweenMaxMonster));
            }
        }

        private void SpawnMonster()
        {
            if (CanSpawn)
            {
                //photonView.RPC(nameof(RpcSpawnMonster), RpcTarget.AllBuffered);
                photonView.RPC(nameof(RpcSpawnMonster), RpcTarget.All);
            }
        }

        [PunRPC]
        private void RpcSpawnMonster()
        {
            m_CreatedEffect.Play();//TODO
            
            PlayRandomSoundOnce();//запускаем  звук создания монстра
            
            if (!CanSpawn)
            {
                return;
            }
            
            if (!gameObject.activeSelf)
            {
                return;
            }

            m_CurrentTotalCount++;
            
            int randomMonster = Random.Range(0, m_MonsterPrefabs.Count);
            //Monster monsterController = Instantiate(m_MonsterPrefabs[randomMonster], transform) as Monster;
            GameObject monsterGameObject =
                PhotonNetwork.Instantiate($"Monsters/{m_MonsterPrefabs[randomMonster].name}",
                                          transform.position,
                                          Quaternion.Euler(0,180,0));
            Monster monster = monsterGameObject.GetComponent<Monster>();
            monster.TargetType = m_TargetType;
            //monster.transform.localPosition = Vector3.zero;
            monster.OnInit = () =>
            {
                OnMonsterInit?.Invoke(monster);
                monster.Activate();
            };
            monster.OnDeath = () =>
            {
                OnMonsterDie?.Invoke(monster);
                MonsterControllers.Remove(monster);
                //GenerateMonster();

                /*
                if (m_WorldController.MonsterScore >= 10 && m_WorldController.MonsterScore < 20)
                {
                    WaveLimitMonster = 3;
                    DelayBetweenMonster = 5f;
                }

                if (m_WorldController.MonsterScore >= 20 && m_WorldController.MonsterScore < 30)
                {
                    WaveLimitMonster = 4;
                    DelayBetweenMonster = 5f;
                }

                if (m_WorldController.MonsterScore >= 30 && m_WorldController.MonsterScore < 50)
                {
                    WaveLimitMonster = 5;
                    DelayBetweenMonster = 4f;
                }

                if (m_WorldController.MonsterScore >= 50)
                {
                    WaveLimitMonster = 6;
                    DelayBetweenMonster = 4f;
                }
                */
            };

            MonsterControllers.Add(monster);
        }
  
        
        public void PlayRandomSoundOnce()
        {
            if (sounds.Length > 0)
            {
                int randomIndex = Random.Range(0, sounds.Length);
                audioSource.clip = sounds[randomIndex];
                audioSource.Play();
            }
        }
        
        
        
        
    }
}