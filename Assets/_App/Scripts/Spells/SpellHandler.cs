using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MobaVR
{
    public class SpellHandler : MonoBehaviour
    {
        private const string TAG = nameof(SpellHandler);

        [SerializeField] private PlayerVR m_PlayerVR;
        [SerializeField] private List<SpellMap> m_Spells;

        [SerializeField] [ReadOnly] private List<SpellBehaviour> m_ActiveSpells = new();

        public List<SpellMap> Spells => m_Spells;
        public List<SpellBehaviour> SpellBehaviours => m_Spells.Select(map => map.SpellBehaviour).Distinct().ToList();
        public List<SpellBehaviour> ActiveSpells => m_ActiveSpells;
        public bool HasActiveSpell => m_ActiveSpells.Count > 0;
        public SpellBehaviour ActiveSpell => m_ActiveSpells.Count > 0 ? m_ActiveSpells[0] : null;

        private void OnValidate()
        {
            if (m_PlayerVR == null)
            {
                TryGetComponent(out m_PlayerVR);

                if (m_PlayerVR == null)
                {
                    m_PlayerVR = GetComponentInParent<PlayerVR>();
                }
            }
        }

        private void Awake()
        {
            foreach (SpellMap spellMap in m_Spells)
            {
                SpellBehaviour spellBehaviour = spellMap.SpellBehaviour;
                spellBehaviour.Init(this, m_PlayerVR);

                spellBehaviour.OnStarted += () => { OnSpellStarted(spellBehaviour); };
                spellBehaviour.OnPerformed += () => { OnSpellPerformed(spellBehaviour); };
                spellBehaviour.OnCompleted += () => { OnSpellCompleted(spellBehaviour); };
            }
        }

        private void OnSpellStarted(SpellBehaviour spellBehaviour)
        {
            Debug.Log($"{TAG}: OnSpellStarted: {spellBehaviour.SpellName}");
        }

        private void OnSpellPerformed(SpellBehaviour spellBehaviour)
        {
            Debug.Log($"{TAG}: OnSpellPerformed: {spellBehaviour.SpellName}");

            /*
            foreach (SpellBehaviour activeSpell in m_ActiveSpells)
            {
                activeSpell.TryInterrupt();
            }
            */

            for (var i = m_ActiveSpells.Count - 1; i >= 0; i--)
            {
                SpellBehaviour activeSpell = m_ActiveSpells[i];
                activeSpell.TryInterrupt();
            }
            

            m_ActiveSpells.Add(spellBehaviour);
        }

        private void OnSpellCompleted(SpellBehaviour spellBehaviour)
        {
            Debug.Log($"{TAG}: OnSpellCompleted: {spellBehaviour.SpellName}");

            //int position = m_ActiveSpells.FindIndex(behaviour => spellBehaviour.SpellName.Equals(behaviour.SpellName));
            int position = m_ActiveSpells.IndexOf(spellBehaviour);
            if (position >= 0)
            {
                m_ActiveSpells.RemoveAt(position);
            }
        }

        private void Update()
        {
            foreach (SpellMap spellMap in m_Spells)
            {
                Debug.Log($"{spellMap.SpellBehaviour.name}: isPerformed: {spellMap.SpellBehaviour.IsPerformed()}");
                //$"isPressed: {spellMap.SpellBehaviour.IsPressed()}, " +
                //$"inProcess: {spellMap.SpellBehaviour.IsInProgress()}"
            }
        }
    }
}