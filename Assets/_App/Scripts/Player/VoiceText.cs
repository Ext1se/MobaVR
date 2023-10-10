using UnityEngine;


    public class VoiceText : MonoBehaviour
    {
        public GameObject textKrik;  // Объект, состояние которого нужно синхронизировать

        void OnEnable()
        {
            // Включаем связанный объект, когда объект, содержащий этот скрипт, включается
            if (textKrik != null)
            {
                textKrik.SetActive(true);
            }
        }

        void OnDisable()
        {
            // Выключаем связанный объект, когда объект, содержащий этот скрипт, выключается
            if (textKrik!= null)
            {
                textKrik.SetActive(false);
            }
        }
    }