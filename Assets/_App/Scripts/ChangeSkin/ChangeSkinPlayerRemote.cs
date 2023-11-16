using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ChangeSkinPlayerRemote : MonoBehaviourPun
{

    [Header("Íîìåð ñêèíà")]
    public int NomerSkins;
    public int OldNomerSkins;
    PhotonView PV;


    [Header("×àñòè ãîëîâû")]
    [Tooltip("Õâîñò")]
    public GameObject TopKnot;

    [Tooltip("Áîðîäêà")]
    public GameObject Beard;

    [Tooltip("Áàêåíáàðäû")]
    public GameObject Sideburns;

    [Tooltip("Øåÿ")]
    public GameObject Neck;

    [Tooltip("Ïðè÷¸ñêà")]
    public GameObject Hair;


    [Header("Ñêèíû")]
    [Tooltip("Äîáàâèòü íóæíîå êîëè÷åñòâî ñêèíîâ")]
    public GameObject[] Skins;
    public GameObject[] OldSkin;




    // Start is called before the first frame update
    void Start()
    {
        NomerSkins = 0;
        PV = GetComponent<PhotonView>();
        //PV.RPC("DisableAllSkinsExceptFirst", RpcTarget.AllBuffered);
        PV.RPC("DisableAllSkinsExceptFirst", RpcTarget.All);
    }



    [PunRPC]
    public void DisableAllSkinsExceptFirst()
    {
        for (int i = 1; i < Skins.Length; i++)
        {
            Skins[i].SetActive(false);
        }
    }




    public void ChangeSkin(int value)
    {
        //OldNomerSkins = NomerSkins;
        //NomerSkins = value;
        PV.RPC("RPC_ChangeSkin", RpcTarget.AllBuffered, value);
    }

    [PunRPC]
    public void RPC_ChangeSkin(int value)
    {
        Debug.Log("Ñêèí ñìåíèëñÿ íà " + NomerSkins);
        // Îòêëþ÷àåì òåêóùèé ñêèí
        Skins[NomerSkins].SetActive(false);

        NomerSkins = value;
        // Âêëþ÷àåì íîâûé ñêèí ëîêàëüíî
        Skins[NomerSkins].SetActive(true);
        ActiveFase();
    }

    //àêòèâèðóåì ðàñòèòåëüíîñòü íà ëèöå, â çàâèñèìîñòè îò ñêèíà
    public void ActiveFase()
    {
        Debug.Log("Âêëþ÷èëàñü áîðîäà ");

        //Õâîñò àêòèâåí òîëüêî íà ýòèõ ñêèíàõ.
        TopKnot.SetActive(NomerSkins == 0);

        //Áîðîäà àêòèâíà òîëüêî íà ýòèõ ñêèíàõ.
        //Beard.SetActive();

        //Áàêåíáàðäû àêòèâíû òîëüêî íà ýòèõ ñêèíàõ.
        Sideburns.SetActive(NomerSkins == 1);

        //Øåÿ àêòèâíà òîëüêî íà ýòèõ ñêèíàõ.
        Neck.SetActive(NomerSkins == 1 || NomerSkins == 2 || NomerSkins == 4 || NomerSkins == 5);

        //Ïðè÷¸ñêà àêòèâíà òîëüêî íà ýòèõ ñêèíàõ.
        Hair.SetActive(NomerSkins == 0 || NomerSkins == 3);


        if (NomerSkins > 5)
        {
            for (int i = 1; i < OldSkin.Length; i++)
            {
                OldSkin[i].SetActive(false);
            }
        }
    }





}
