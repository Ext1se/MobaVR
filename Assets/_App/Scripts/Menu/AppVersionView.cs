using TMPro;
using UnityEngine;

namespace MobaVR
{
    public class AppVersionView : MonoBehaviour
    {
        private TextMeshProUGUI m_VersionView;

        private void Awake()
        {
            if (TryGetComponent(out m_VersionView))
            {
                m_VersionView.text = Application.version;
            }
        }
    }
}