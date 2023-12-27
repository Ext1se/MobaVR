using System;
using System.Collections;
using ReadyPlayerMe;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MobaVR
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Ready Player Me/Eye Animation Handler", 0)]
    public class EyeAnimationHandler : MonoBehaviour
    {
        private const int VERTICAL_MARGIN = 10;
        private const int HORIZONTAL_MARGIN = 15;
        private const float EYE_STEP = 0.006f;

        private readonly string[] HEAD_PATHS = new[]
        {
            "Root/Pelvis/spine_01/spine_02/spine_03/neck_01/head/",
            "root/pelvis/spine_01/spine_02/spine_03/neck_01/head/",
            "Armature/Root/Pelvis/spine_01/spine_02/spine_03/neck_01/head/",
            "Armature/root/pelvis/spine_01/spine_02/spine_03/neck_01/head/",
            "armature/root/pelvis/spine_01/spine_02/spine_03/neck_01/head/",
        };

        private readonly string[] LEFT_EYE_PATHS = new[]
        {
            "Eye_L",
            "eye_l",
        };

        private readonly string[] RIGHT_EYE_PATHS = new[]
        {
            "Eye_R",
            "eye_r",
        };

        private readonly string[] LEFT_EYE_DOWN_PATHS = new[]
        {
            "Eye_Down_L",
            "eyelid_lower_l",
        };

        private readonly string[] RIGHT_EYE_DOWN_PATHS = new[]
        {
            "Eye_Down_R",
            "eyelid_lower_r",
        };

        private readonly string[] LEFT_EYE_UP_PATHS = new[]
        {
            "Eye_Up_L",
            "eyelid_upper_l",
        };

        private readonly string[] RIGHT_EYE_UP_PATHS = new[]
        {
            "Eye_Up_R",
            "eyelid_upper_r",
        };

        private const string PATH_HEAD = "Root/Pelvis/spine_01/spine_02/spine_03/neck_01/head/";
        private const string LEFT_EYE_DOWN = "Eye_Down_L";
        private const string RIGHT_EYE_DOWN = "Eye_Down_R";
        private const string LEFT_EYE_UP = "Eye_Up_L";
        private const string RIGHT_EYE_UP = "Eye_Up_R";
        private const string RIGHT_EYE = "Eye_R";
        private const string LEFT_EYE = "Eye_L";

        [SerializeField] private Transform m_Head;
        [SerializeField, Range(0, 3)] private float blinkSpeed = 0.1f;
        [SerializeField, Range(1, 10)] private float blinkInterval = 3f;

        private Transform leftEye;
        private Transform rightEye;
        private Transform leftEyeDown;
        private Transform leftEyeUp;
        private Transform rightEyeDown;
        private Transform rightEyeUp;

        private WaitForSeconds blinkDelay;
        private Coroutine blinkCoroutine;

        private bool hasEyes;
        private Vector3 eyeStep;

        private Vector3 initLeftDownPosition;
        private Vector3 initRightDownPosition;
        private Vector3 initLeftUpPosition;
        private Vector3 initRightUpPosition;

        public float BlinkSpeed
        {
            get => blinkSpeed;
            set
            {
                blinkSpeed = value;
                if (Application.isPlaying)
                {
                    Initialize();
                }
            }
        }

        public float BlinkInterval
        {
            get => blinkSpeed;
            set
            {
                blinkInterval = value;
                if (Application.isPlaying)
                {
                    Initialize();
                }
            }
        }

        private void OnValidate()
        {
            if (m_Head == null)
            {
                m_Head = FindTransform(transform, HEAD_PATHS);
            }
        }

        private void Awake()
        {
            /*
            rightEye = transform.Find($"{PATH_HEAD}{RIGHT_EYE}");
            leftEye = transform.Find($"{PATH_HEAD}{LEFT_EYE}");
            rightEyeDown = transform.Find($"{PATH_HEAD}{RIGHT_EYE_DOWN}");
            leftEyeDown = transform.Find($"{PATH_HEAD}{LEFT_EYE_DOWN}");
            rightEyeUp = transform.Find($"{PATH_HEAD}{RIGHT_EYE_UP}");
            leftEyeUp = transform.Find($"{PATH_HEAD}{LEFT_EYE_UP}");
            */

            FindEyes();

            initLeftDownPosition = leftEyeDown.transform.localPosition;
            initRightDownPosition = rightEyeDown.transform.localPosition;
            initLeftUpPosition = leftEyeUp.transform.localPosition;
            initRightUpPosition = rightEyeUp.transform.localPosition;

            eyeStep = new Vector3(EYE_STEP, 0, 0);
        }

        private void FindEyes()
        {
            if (m_Head == null)
            {
                return;
            }
            
            leftEye = FindTransform(m_Head, LEFT_EYE_PATHS);
            rightEye = FindTransform(m_Head, RIGHT_EYE_PATHS);
            rightEyeDown = FindTransform(m_Head, RIGHT_EYE_DOWN_PATHS);
            leftEyeDown = FindTransform(m_Head, LEFT_EYE_DOWN_PATHS);
            rightEyeUp = FindTransform(m_Head, RIGHT_EYE_UP_PATHS);
            leftEyeUp = FindTransform(m_Head, LEFT_EYE_UP_PATHS);
        }

        private Transform FindTransform(Transform root, string[] paths)
        {
            foreach (string head in paths)
            {
                var findTransform = root.transform.Find(head);
                if (findTransform != null)
                {
                    return findTransform;
                }
            }

            return null;
        }

        private void OnDisable() => CancelInvoke();

        private void OnEnable() => Initialize();

        private void OnDestroy()
        {
            CancelInvoke();
            if (blinkCoroutine != null)
            {
                blinkCoroutine.Stop();
            }
        }

        private void Initialize()
        {
            blinkDelay = new WaitForSeconds(blinkSpeed);
            CancelInvoke();
            InvokeRepeating(nameof(AnimateEyes), 1, blinkInterval);
        }

        private void AnimateEyes()
        {
            RotateEyes();
            blinkCoroutine = BlinkEyes().Run();
        }

        private void RotateEyes()
        {
            float vertical = Random.Range(-VERTICAL_MARGIN, VERTICAL_MARGIN);
            float horizontal = Random.Range(-HORIZONTAL_MARGIN, HORIZONTAL_MARGIN);

            var rotation = Quaternion.Euler(horizontal, 0, vertical);

            leftEye.localRotation = rotation;
            rightEye.localRotation = rotation;
        }

        private IEnumerator BlinkEyes()
        {
            leftEyeDown.transform.localPosition = initLeftDownPosition - eyeStep;
            rightEyeDown.transform.localPosition = initRightDownPosition - eyeStep;

            leftEyeUp.transform.localPosition = initLeftUpPosition + eyeStep;
            rightEyeUp.transform.localPosition = initRightUpPosition + eyeStep;

            yield return blinkDelay;

            leftEyeDown.transform.localPosition = initLeftDownPosition;
            rightEyeDown.transform.localPosition = initRightDownPosition;

            leftEyeUp.transform.localPosition = initLeftUpPosition;
            rightEyeUp.transform.localPosition = initRightUpPosition;
        }
    }
}