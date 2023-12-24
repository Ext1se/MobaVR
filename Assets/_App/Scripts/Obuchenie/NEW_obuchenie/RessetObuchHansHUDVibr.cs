using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BNG;

//Этот скрипт, при смене цены выключает вибрацию контроллеров, ставит все HUD на место, ставит все руки на метсо и включает нужные.
public class RessetObuchHansHUDVibr : MonoBehaviour
{
    
    public Grabber grabLeft;//рука, которая будет вибрировать (вставлять в неё граббер)
    public Grabber grabRight;//рука, которая будет вибрировать (вставлять в неё граббер)

    public HandAnimationController HandAnimationControllerLeft;//через этот скрипт могу управлять анимациями и вибрациями рук
    public HandAnimationController HandAnimationControlRight;//через этот скрипт могу управлять анимациями и вибрациями рук
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OffVibro()
    {

        HandAnimationControllerLeft.StopVibration();
        HandAnimationControlRight.StopVibration();
        InputBridge.Instance.VibrateController(0f, 0f, 0f, grabLeft.HandSide); //выключаем вибрацию
        InputBridge.Instance.VibrateController(0f, 0f, 0f, grabRight.HandSide); //выключаем вибрацию
    }
}
