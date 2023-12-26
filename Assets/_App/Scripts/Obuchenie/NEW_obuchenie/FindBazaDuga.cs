using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Скрипт. который делает дугу от места старта (зона команды) до игрока. Активируется тогда, когда стартует запуск раундов ПВП. Когда моргают зоны
public class FindBazaDuga : MonoBehaviour
{
    public GameObject Player;
    public GameObject Point;
    void OnEnable()
    {
        if (Player == null)
        {
                
            Player = GameObject.Find("CenterEyeAnchor");
                    
            if (Player != null)
            {
                RessetPlayerPoint();
            }
        }
    }
    
    
    //точка перемещается на плеера
    public void RessetPlayerPoint()
    {
        Point.transform.SetParent(Player.transform);
        // Point.transform.localPosition = Vector3.zero;
        Point.transform.localPosition = new Vector3(0, -0.4f, 0);
    }
    
    
}
