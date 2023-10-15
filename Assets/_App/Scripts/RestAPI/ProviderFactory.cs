using UnityEngine;

namespace MobaVR
{
    public class ProviderFactory : MonoBehaviour
    {
        [SerializeField] private BaseApiProvider m_LocalProvider;
        [SerializeField] private BaseApiProvider m_RemoteProvider;
        //[SerializeField] private BaseApiProvider m_DevelopProvider;
    }
}