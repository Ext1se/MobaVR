using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    public float rotationSpeed = 10f;

    private void Update()
    {
        // �������� ������� ���� �������� �������
        Quaternion currentRotation = transform.rotation;

        // ��������� ����� ���� �������� � ����������� ��������
        Quaternion newRotation = Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f) * currentRotation;

        // ��������� ����� ���� �������� � �������
        transform.rotation = newRotation;
    }
}
