using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//скрипт висит на щитах у персонажа обучения, он при включении скрипта, говорит скрипту LessonAnimatorHands, чтобы тот отправил данные о показываемой анимации рук
public class OnEnableRunAnimatorHands : MonoBehaviour
{
        
        public string NameAnimationRight;//название анимации для правой руки
        public string NameAnimationLeft;//название анимации для левой руки
        public LessonAnimatorHands LessonAnimatorHandsScript;// скрипт, в котором я активирую визуальных помощников рук, которые показывают как нажать
        void OnEnable()
        {
                if (LessonAnimatorHandsScript != null)
                {
                        LessonAnimatorHandsScript.ActivateHelpHands(NameAnimationRight,NameAnimationLeft);
                }
        }
        
}
