using UnityEngine;

namespace MobaVR
{
    public class DontDestroyWithCheck : MonoBehaviour
    {
        [SerializeField] private bool m_IsCheckExist = true;
        
        private void Awake()
        {
            if (m_IsCheckExist)
            {
                GameObject oldGameObject = GameObject.Find(gameObject.name);
                if (oldGameObject != null && oldGameObject != gameObject)
                {
                    //Destroy(gameObject);
                    return;
                }
            }
            
            DontDestroyOnLoad(gameObject);
        }
    }
}