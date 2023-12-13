using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MobaVR;
using RootMotion.FinalIK;
using BNG;

    //скрипт, который делает копии всех скинов игроков и переноси их в сервис, и отключает там. От туда, в книгу потом их вытаскивает скрипт ZonaBook

public class ServiseSkin : MonoBehaviour
{
    public GameObject targetObject; // GameObject, который будет источником копирования
    public Transform destinationParent; // Родительский объект для копии

    private void Start()
    {
        CopyAllChildren();
    }

    private void CopyAllChildren()
    {
        // Копирование всех дочерних элементов из источника
        CopyChildren(targetObject.transform, destinationParent);
    }

    private void CopyChildren(Transform source, Transform parent)
    {
        foreach (Transform child in source)
        {
            // Создание копии дочернего объекта
            GameObject copiedChild = Instantiate(child.gameObject, parent);
            copiedChild.transform.localPosition = Vector3.zero; // Установка локальной позиции копии
            copiedChild.transform.localRotation = Quaternion.identity; // Установка локального поворота копии
            copiedChild.transform.localScale = Vector3.one; // Установка локального масштаба копии

            // Рекурсивное копирование дочерних элементов каждого ребенка
            CopyChildren(child, copiedChild.transform);

            // Отключение компонентов VR IK
            SkinRagdoll skinRagdoll = copiedChild.GetComponent<SkinRagdoll>();
            if (skinRagdoll != null)
            {
                skinRagdoll.enabled = false;
            }
            
            VRIK vrIK = copiedChild.GetComponent<VRIK>();
            if (vrIK != null)
            {
                vrIK.enabled = false;
            }

            // Если элемент "Hands" существует и найден, то включить его
            Transform handsObject = copiedChild.transform.Find("Body/Base/Hands");
            if (handsObject != null)
            {
                //handsObject.gameObject.SetActive(false);
                handsObject.gameObject.SetActive(true);
            }
            
            // Установка видимости кожи
            if (copiedChild.TryGetComponent(out Skin skin))
            {
                skin.SetVisibilityFace(true);
                skin.SetVisibilityVR(true);
            }

            // Скрывание копированного объекта
            copiedChild.gameObject.SetActive(false);
        }
    }
}
