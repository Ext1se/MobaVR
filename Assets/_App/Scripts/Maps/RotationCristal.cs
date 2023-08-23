using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationCristal : MonoBehaviour
{
    public float rotationSpeed = 60.0f; // �������� �������� � �������� � �������
    public float pistonSpeed = 1.0f; // �������� �������� ����� � ����
    public float pistonRange = 0.3f; // ���������� �������� ����� � ���� �� ��������� �������

    private Vector3 initialPosition;
    private bool movingUp = true;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        // �������� ������� ������ ����� ��� Y
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // �������� ������� ����� � ���� ��� �������
        if (movingUp)
        {
            transform.position += Vector3.up * pistonSpeed * Time.deltaTime;
            if (transform.position.y >= initialPosition.y + pistonRange)
                movingUp = false;
        }
        else
        {
            transform.position -= Vector3.up * pistonSpeed * Time.deltaTime;
            if (transform.position.y <= initialPosition.y)
                movingUp = true;
        }
    }
}
