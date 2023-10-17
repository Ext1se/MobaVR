using UnityEngine;

namespace MobaVR
{
    public class ProviderFactory : MonoBehaviour
    {
        [SerializeField] private ApiProviderType m_ProviderType = ApiProviderType.LOCAL;

        [Header("Prefabs")]
        [SerializeField] private BaseApiProvider m_LocalProvider;
        [SerializeField] private BaseApiProvider m_RemoteProvider;
        //[SerializeField] private BaseApiProvider m_DevelopProvider;

        private void Awake()
        {
            CreateProvider();
        }

        public void CreateProvider()
        {
            BaseApiProvider provider = null;

            switch (m_ProviderType)
            {
                case ApiProviderType.LOCAL:
                    provider = m_LocalProvider;
                    break;
                case ApiProviderType.REMOTE:
                    provider = m_RemoteProvider;
                    break;
            }

            Instantiate(provider);
        }
    }
}