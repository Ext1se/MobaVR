using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BNG {
    // Класс для выбора модели руки
    public class HandModelSelector : MonoBehaviour {

        /// По умолчанию используемая модель руки, если в PlayerPrefs ничего не сохранено или LoadHandSelectionFromPrefs установлено в false
        [Tooltip("Индекс дочернего объекта модели руки для использования, если в PlayerPrefs ничего не сохранено или LoadHandSelectionFromPrefs установлено в false")]        
        public int DefaultHandsModel = 1;

        /// Если true, выбранная модель руки будет сохраняться и загружаться из PlayerPrefs. Если false, будет загружена DefaultHandModel.
        [Tooltip("Если true, выбранная модель руки будет сохраняться и загружаться из PlayerPrefs")]  
        public bool LoadHandSelectionFromPrefs = false;

        [Tooltip("Входной сигнал для переключения между руками")]
        public ControllerBinding ToggleHandsInput = ControllerBinding.RightThumbstickDown;

        /// Этот Transform содержит все модели рук и может использоваться для их включения / отключения
        [Tooltip("Transform, содержащий все модели рук. Можно использовать для их включения / отключения")]
        public Transform LeftHandGFXHolder;

        /// Этот Transform содержит все модели рук и может использоваться для их включения / отключения
        [Tooltip("Transform, содержащий все модели рук. Можно использовать для их включения / отключения")]
        public Transform RightHandGFXHolder;

        private int _selectedHandGFX = 0;

        /// Используется для опции IK для рук / тела
        [Tooltip("Используется для опции IK для рук / тела")]
        public CharacterIK IKBody;

        /// Это начальная точка линии для UI. При смене моделей или контроллеров, возможно, потребуется ее перемещение
        UIPointer uiPoint;

        List<Transform> leftHandModels = default;
        Transform activatedLeftModel = default;

        List<Transform> rightHandModels = default;
        Transform activatedRightModel = default;

        void Start() {
            // Получаем компонент UIPointer из дочерних объектов
            uiPoint = GetComponentInChildren<UIPointer>();

            // Кэшируем модели рук
            CacheHandModels();

            // Загружаем новую модель руки или модель по умолчанию
            if (LoadHandSelectionFromPrefs) {
                ChangeHandsModel(PlayerPrefs.GetInt("HandSelection", DefaultHandsModel), false);
            }
            else {
                ChangeHandsModel(DefaultHandsModel, false);
            }
        }

        void Update() {
            // Переключение между моделями рук с помощью правого стика
            if (ToggleHandsInput.GetDown()) {
                ChangeHandsModel(_selectedHandGFX + 1, LoadHandSelectionFromPrefs);
            }
        }

        // Кэшируем модели рук для быстрого доступа
        public void CacheHandModels() {
            leftHandModels = new List<Transform>();
            for(int x = 0; x < LeftHandGFXHolder.childCount; x++) {
                leftHandModels.Add(LeftHandGFXHolder.GetChild(x));
            }

            rightHandModels = new List<Transform>();
            for (int x = 0; x < RightHandGFXHolder.childCount; x++) {
                rightHandModels.Add(RightHandGFXHolder.GetChild(x));
            }
        }

        // Изменить текущую модель руки
        public void ChangeHandsModel(int childIndex, bool save = false) {
            // Деактивируем предыдущие модели
            if(activatedLeftModel != null) {
                activatedLeftModel.gameObject.SetActive(false);
            }
            if (activatedRightModel != null) {
                activatedRightModel.gameObject.SetActive(false);
            }

            // Устанавливаем новый индекс модели руки
            _selectedHandGFX = childIndex;
            if (_selectedHandGFX > leftHandModels.Count - 1) {
                _selectedHandGFX = 0;
            }

            // Активируем новую модель руки
            activatedLeftModel = leftHandModels[_selectedHandGFX];
            activatedRightModel = rightHandModels[_selectedHandGFX];

            activatedLeftModel.gameObject.SetActive(true);
            activatedRightModel.gameObject.SetActive(true);

            /*// Обновляем аниматоры для рук
            HandController leftControl = LeftHandGFXHolder.parent.GetComponent<HandController>();
            HandController rightControl = RightHandGFXHolder.parent.GetComponent<HandController>();

            // У физических рук свой аниматор
            bool isPhysicalHand = activatedLeftModel.name.ToLower().Contains("physical");
            if(isPhysicalHand) {
                leftControl.HandAnimator = null;
                rightControl.HandAnimator = null;
            }
            else if (leftControl && rightControl) {
                leftControl.HandAnimator = activatedLeftModel.GetComponentInChildren<Animator>(true);
                rightControl.HandAnimator = activatedRightModel.GetComponentInChildren<Animator>(true);
            }*/

            // Включаем / выключаем IK для тела (только для демо)
            if (IKBody != null) {
                IKBody.gameObject.SetActive(activatedLeftModel.transform.name.Contains("IK"));
            }

            // Изменяем позицию указателя UI в зависимости от используемой модели руки или контроллера
            if ((activatedLeftModel.transform.name.StartsWith("OculusTouchForQuestAndRift") || activatedLeftModel.transform.name.StartsWith("ControllerReferences")) && uiPoint != null) {
                uiPoint.transform.localPosition = new Vector3(0, 0, 0.0462f);
                uiPoint.transform.localEulerAngles = new Vector3(0, 0f, 0);
            }
            else if (_selectedHandGFX != 0 && uiPoint != null) {
                uiPoint.transform.localPosition = new Vector3(0.0392f, 0.0033f, 0.0988f);
                uiPoint.transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            // Если нужно, сохраняем выбор модели руки в PlayerPrefs
            if (save) {
                PlayerPrefs.SetInt("HandSelection", _selectedHandGFX);
            }
        }
    }
}
