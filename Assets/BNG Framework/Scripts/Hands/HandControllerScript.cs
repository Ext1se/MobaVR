using UnityEngine;

public class HandControllerScript : MonoBehaviour
{
    public enum HandType
    {
        MainHands,
        ZombieHands,
        OculusControllers,
        PicoControllers,
        PigHooves
    }

    public HandType activeHandType;

    private void Start()
    {
        // Инициализация с нужными руками при старте
        SetActiveHand(activeHandType);
    }

    public void SetActiveHand(HandType handType)
    {
        // Отключение всех рук
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // Включение нужных рук
        Transform findHand = transform.Find(handType.ToString());
        if (findHand != null)
        {
            findHand.gameObject.SetActive(true);
        }
        activeHandType = handType;
    }
}