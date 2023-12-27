using System.Collections.Generic;
using MobaVR.Sound;
using UnityEngine;

namespace MobaVR.Content
{
    public class PveModeContent : MonoBehaviour
    {
        [SerializeField] private List<MonsterPointSpawner> m_Spawners = new List<MonsterPointSpawner>();
        [SerializeField] private Lich m_Lich;
        [SerializeField] private PveLichModeSound m_Sound;
        [SerializeField] private PveModeView m_PveModeView;

        public List<MonsterPointSpawner> Spawners => m_Spawners;
        public Lich Lich => m_Lich;
        public PveLichModeSound Sound => m_Sound;
        public PveModeView ModeView => m_PveModeView;
    }
}