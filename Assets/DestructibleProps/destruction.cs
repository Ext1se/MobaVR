using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

public class destruction : MonoBehaviour
{
    public GameObject[] Chunks;
    private Vector3[] originalPosition; // Переменная для сохранения позиции оригинального объекта
    private Quaternion[] originalRotation; // Переменная для сохранения поворота оригинального объекта

    public GameObject[] HidingObjs; //список объектов, которые будут скрыты после давки.
    [Range(1, 1000)]
    public int Health = 100;
    private int ObjectHealth;
    public float ExplosionForce = 200; //сила, приложенная к каждому фрагменту разбитого объекта.
    public float ChunksRotation = 20; //сила вращения добавляется к каждому куску, когда он взрывается.
    public float strength = 5; //Как легко объект меняется.
    public bool BreakByClick = false;
    public bool DestroyAftertime = true; //если true, то куски будут уничтожены по истечении времени.
    public float time = 15; //время до того, как фрагменты будут удалены со сцены.
    public GameObject FX;
    public bool
        AutoDestroy =
            true; //если true, то объект будет автоматически разорван по истечении "AutoDestTime" с момента запуска игры.
    public float AutoDestTime = 2; //Время автоматического уничтожения (отсчитывается от начала игры).
    PhotonView photonView;

    public AudioClip[] sounds;

    private AudioSource audioSource;

    [Space]
    [Header("Demo features. Need to refactor")]
    //TODO: remove it
    [SerializeField] private bool m_IsEasyDestroy = true;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        
        //TODO
        if (m_IsEasyDestroy)
        {
            Health = (int)(Health / 1.8f);
        }
    }





    void Start()
    {
        photonView = GetComponent<PhotonView>();


        if (AutoDestroy)
        {
            Invoke("Crushing", AutoDestTime);
        }

        if (GetComponent<AudioSource>())
        {
            GetComponent<AudioSource>().pitch = Random.Range(0.7f, 1.1f);
        }

        if (HidingObjs.Length != 0)
        {
            foreach (GameObject hidingObj in HidingObjs)
            {
                hidingObj.SetActive(true);
            }
        }

        ObjectHealth = Health;
    }

    void OnCollisionEnter(Collision other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if (other.gameObject.GetComponent<BulletsDamage>())
        {
            int damage = other.gameObject.GetComponent<BulletsDamage>().Damage;
            photonView.RPC("DecreaseHealth", RpcTarget.All, damage);
        }
        else if (other.relativeVelocity.magnitude > strength)
        {
            photonView.RPC("DecreaseHealth", RpcTarget.All, 1);
        }
    }

    [PunRPC]
    public void DecreaseHealth(int damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Crushing();
        }
    }


    void OnMouseDown()
    {
        if (BreakByClick)
        {
            Crushing();
            BreakByClick = false;
        }
    }

    void Crushing()
    {
        if (HidingObjs.Length != 0)
        {
            foreach (GameObject hidingObj in HidingObjs)
            {
                hidingObj.SetActive(false);
            }
        }

        if (FX)
        {
            FX.SetActive(true);
        }

        PlayRandomSound();


        //if (GetComponent<AudioSource>())
        //{
        //    GetComponent<AudioSource>().Play();
        //}

        if (GetComponent<Renderer>())
        {
            GetComponent<Renderer>().enabled = false;
        }

        GetComponent<Collider>().enabled = false;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }


        // Инициализируем массивы с размером, равным количеству кусков
        originalPosition = new Vector3[Chunks.Length];
        originalRotation = new Quaternion[Chunks.Length];


        // Пробегаемся по каждому куску
        for (int i = 0; i < Chunks.Length; i++)
        {
            GameObject chunk = Chunks[i];

            // Сохраняем позицию и поворот куска в массивы
            originalPosition[i] = chunk.transform.position;
            originalRotation[i] = chunk.transform.rotation;

            chunk.SetActive(true);
            chunk.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * -ExplosionForce);
            chunk.GetComponent<Rigidbody>()
                 .AddRelativeTorque(Vector3.forward * -ChunksRotation * Random.Range(-5f, 5f));
            chunk.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * -ChunksRotation * Random.Range(-5f, 5f));
        }


        ////пробегаемся по каждому куску
        //foreach (GameObject chunk in Chunks){


        //	// Сохраняем позицию и поворот оригинального объекта
        //	originalPosition = transform.position;
        //	originalRotation = transform.rotation;


        //	chunk.SetActive(true);
        //	chunk.GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * -ExplosionForce);
        //	chunk.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.forward * -ChunksRotation*Random.Range(-5f, 5f));
        //	chunk.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * -ChunksRotation*Random.Range(-5f, 5f));
        //}


        if (DestroyAftertime)
        {
            Invoke("DestructObject", time);
        }
    }

    void DestructObject()
    {
        //Destroy(gameObject);

        // Пробегаемся по каждому куску
        for (int i = 0; i < Chunks.Length; i++)
        {
            GameObject chunk = Chunks[i];

            // Восстанавливаем позицию и поворот куска из массивов originalPosition и originalRotation
            if (originalPosition != null && originalRotation != null)
            {
                chunk.transform.position = originalPosition[i];
                chunk.transform.rotation = originalRotation[i];
            }
            
            // Выключаем кусок
            chunk.SetActive(false);
        }
    }


    private void OnDisable()
    {
        Restore();
    }

    [ContextMenu("Restore")]
    public void Restore()
    {
        if (HidingObjs.Length != 0)
        {
            foreach (GameObject hidingObj in HidingObjs)
            {
                hidingObj.SetActive(true);
            }
        }

        if (FX)
        {
            FX.SetActive(false);
        }

        //if (GetComponent<Renderer>())
        //{
        //	GetComponent<Renderer>().enabled = true;
        //}
        
        CancelInvoke(nameof(DestructObject));
        DestructObject();

        GetComponent<Collider>().enabled = true;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }

        Health = ObjectHealth;
        
        /*
        if (Health <= 0)
        {
            photonView.RPC("ResetHealth", RpcTarget.All);
        }
        */
    }

    [PunRPC]
    public void ResetHealth()
    {
        Health = ObjectHealth;
    }

    public void PlayRandomSound()
    {
        if (sounds != null && sounds.Length > 0)
        {
            // Генерируем случайный индекс для выбора случайного звука из массива
            int randomIndex = Random.Range(0, sounds.Length);

            // Воспроизводим звук, соответствующий случайному индексу
            AudioClip soundToPlay = sounds[randomIndex];
            audioSource.PlayOneShot(soundToPlay);
        }

    }


}