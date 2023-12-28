using MobaVR;
using UnityEngine;
using Photon.Pun;

public class grushaPig : MonoBehaviourPunCallbacks
{
    public GameObject grusha_celaya;
    public GameObject grusha_ogrizok;
    public perdeg_grusha grusha_perdeg;
    public AudioSource zvuk_edi;
    public AudioClip clip;
    public bool rungrusha;


    private void start()
    {
        rungrusha = false;
        grusha_celaya.SetActive(true);
        grusha_ogrizok.SetActive(false);
        grusha_perdeg.gameObject.SetActive(false);

    }
    private void OnTriggerEnter(Collider other)
    {
       
        //если груша входит в рот, то мы её кушаем и перемещаем пердёж в игрока
        if (other.CompareTag("mouth") && !rungrusha)
        {
            //photonView.RPC("EatGrusha", RpcTarget.All);
            EatGrusha();
            grusha_perdeg.transform.SetParent(other.transform);
            grusha_perdeg.Play();

            if (PhotonNetwork.IsMasterClient)
            {
                PlayerVR playerVR = other.transform.GetComponentInParent<PlayerVR>();
                if (playerVR != null)
                {
                    playerVR.SkinCollection.SetAnimalDefaultSkin();
                }
            }
        }
    }

    [PunRPC]
    void EatGrusha()
    {
        rungrusha = true;
        zvuk_edi.clip = clip;
        zvuk_edi.Play();
        grusha_celaya.SetActive(false);
        grusha_ogrizok.SetActive(true);
        grusha_perdeg.gameObject.SetActive(true);
    }
}