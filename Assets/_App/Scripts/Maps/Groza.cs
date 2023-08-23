using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Groza : MonoBehaviour
{
    public GameObject lightning1;
    public GameObject lightning2;
    public float minDelay = 3.0f; // ����������� �������� ����� ��������� ��������
    public float maxDelay = 15.0f; // ������������ �������� ����� ��������� ��������

    private void Start()
    {
        // ������ ���� �������
        StartCoroutine(FlashLightning());
    }

    System.Collections.IEnumerator FlashLightning()
    {
        while (true)
        {
            // �������� ���������� ������� ����� ��������� ��������
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            // ������� ��������� ������ ��� �������
            GameObject selectedLightning = Random.Range(0, 2) == 0 ? lightning1 : lightning2;

            // �������� ������
            selectedLightning.SetActive(true);

            // ������� 1 �������
            yield return new WaitForSeconds(2f);

            // ��������� ������
            selectedLightning.SetActive(false);
        }
    }
}
