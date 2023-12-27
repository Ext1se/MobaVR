using UnityEngine;
using Photon.Pun;
using BNG;
//скрипт, который запускает в пляс бочку с пивом.
public class TusaPashi : MonoBehaviourPunCallbacks
{
    private Animator animator;
    public GameObject objectToUnstatic; // Объект для разблокировки от статики
    public GameObject objectToUnstatic2; // Объект для разблокировки от статики
    public bool Run = false;
    public bool test = false;
    
    private void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component is missing on this GameObject");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        Grabber grabber = other.GetComponent<Grabber>();
        if (grabber != null && Run == false)
        {
            ActivateTusaPashi();
            Run = true;
        }
    }

    void Update()
    {
        if (test == true)
        {
            ActivateTusaPashi();
            test = false;
        }
    }

    
    private void ActivateTusaPashi()
    {
        photonView.RPC(nameof(RpcActivateTusaPashi), RpcTarget.All);
    }

    [PunRPC]
    private void RpcActivateTusaPashi()
    {
        if (animator != null)
        {
            if (objectToUnstatic != null && objectToUnstatic2 != null)
            {
                objectToUnstatic.isStatic = false;//отключаем статику у ручки
                objectToUnstatic2.isStatic = false;//отключаем статику у ручки
                animator.SetTrigger("StartTusaPashi");
            }

        }
    }
}