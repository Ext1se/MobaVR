using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoletBookMulayg : MonoBehaviour
{
    
    public float moveSpeed = 5f; // Скорость перемещения к цели
    public bool isMoving = false; // Указывает, перемещается ли книга
    
    public Transform targetBookHand; // Цель для перемещенияs
    
    // Start is called before the first frame update
    void Start()
    {
        targetBookHand = GameObject.Find("TargetBookHand").transform;


    }


    void Update()
    {

        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetBookHand.position, moveSpeed * Time.deltaTime);
        }
        
    }
}
