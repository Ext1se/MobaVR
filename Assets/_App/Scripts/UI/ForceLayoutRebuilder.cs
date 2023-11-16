using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MobaVR
{
    public class ForceLayoutRebuilder : MonoBehaviour
    {
        private RectTransform m_RectTransform;

        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(WaitAndUpdateLayout());
        }

        private void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
        }
        
        private IEnumerator WaitAndUpdateLayout()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.1f);
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_RectTransform);
            m_RectTransform.ForceUpdateRectTransforms();
        }
    }
}