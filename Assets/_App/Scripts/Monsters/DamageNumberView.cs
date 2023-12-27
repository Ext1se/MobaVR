using System;
using DamageNumbersPro;
using UnityEngine;

namespace MobaVR
{
    public class DamageNumberView : MonoBehaviour
    {
        [SerializeField] private DamageNumber m_DamageNumber;
        [SerializeField] private Color m_DefaultColor = new Color(0.82f, 0.37f, 0.36f);
        [SerializeField] private Color m_CritColor = new Color(0.9f, 0.37f, 0.36f);
        [SerializeField] private Color m_HealColor = new Color(0.1f, 0.9f, 0.1f);
        [SerializeField] private Color m_ImmortalColor = Color.white;
        
        public void SpawnNumber(Vector3 position, 
                                float damage, 
                                Monster.MonsterDamageType monsterDamageType,
                                float scale = 1f)
        {
            DamageNumber damageNumber = Instantiate(m_DamageNumber);
            damageNumber.faceCameraView = true;
            damageNumber.transform.position = position;
            damageNumber.transform.localScale = Vector3.one * scale;
            damageNumber.number = damage;
            damageNumber.numberSettings.customColor = true;

            switch (monsterDamageType)
            {
                case Monster.MonsterDamageType.HP:
                    damageNumber.numberSettings.color = m_DefaultColor;
                    damageNumber.transform.localScale = new Vector3(1f, 1f, 1f) * scale;
                    break;
                case Monster.MonsterDamageType.HEAL:
                    damageNumber.leftText = "+";
                    damageNumber.numberSettings.color = m_HealColor;
                    damageNumber.transform.localScale = new Vector3(1f, 1f, 1f) * scale;
                    break;
                case Monster.MonsterDamageType.IMMORTAL:
                    damageNumber.number = 0f;
                    //damageNumber.enableNumber = false;
                    //damageNumber.enableTopText = true;
                    //damageNumber.topText = "INVULNERABLE";
                    //damageNumber.prefix = "INVULNERABLE";
                    //damageNumber.prefixSettings.color = Color.white;
                    damageNumber.numberSettings.color = m_ImmortalColor;
                    damageNumber.transform.localScale = new Vector3(1f, 1f, 1f) * scale;
                    break;
                case Monster.MonsterDamageType.CRIT:
                    damageNumber.numberSettings.color = m_CritColor;
                    damageNumber.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f) * scale;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(monsterDamageType), monsterDamageType, null);
            }
        }
    }
}