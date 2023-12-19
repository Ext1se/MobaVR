using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHance.Assets.Scripts.Attributes;
using NHance.Assets.Scripts.Enums;
using NHance.Assets.Scripts.Items;
using NHance.Assets.StylizedCharacter.Scripts;
using UnityEngine;

namespace NHance.Assets.Scripts
{
    [HelpURL("https://github.com/DustCoron/NhanceRPGDocumentation/wiki/Demo-scene")]
    public class NHAvatarDemo : MonoBehaviour
    {
        [SerializeField] private NHAvatar avatar;

        [Header("Set 1")]
        public List<NHItem> Set_1 = new List<NHItem>();
        
        [Header("Set 2")]
        public List<NHItem> Set_2 = new List<NHItem>();
		
		[Header("Set 3")]
        public List<NHItem> Set_3 = new List<NHItem>();
		
        [Header("Set 4")]
        public List<NHItem> Set_4 = new List<NHItem>();
        
        [Header("Set 5")]
        public List<NHItem> Set_5 = new List<NHItem>();
        
        [Header("Set 6")]
        public List<NHItem> Set_6 = new List<NHItem>();
        
        [Header("Set 7")]
        public List<NHItem> Set_7 = new List<NHItem>();
        
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1)) Clear();
            if (Input.GetKeyDown(KeyCode.F2)) Set1();
            if (Input.GetKeyDown(KeyCode.F3)) Set2();
            if (Input.GetKeyDown(KeyCode.F4)) Set3();
            if (Input.GetKeyDown(KeyCode.F5)) Set4();
            if (Input.GetKeyDown(KeyCode.F6)) Set5();
            if (Input.GetKeyDown(KeyCode.F7)) Set6();
            if (Input.GetKeyDown(KeyCode.F8)) Set7();
        }

        public void Set7()
        {
            if(Set_7.Count == 0)
                return;
            
            avatar
                .ClearItems()
                .SetItems(Set_7.ToArray())
                .Clean()
                .Compile();
        }

        public void Set6()
        {
            if(Set_6.Count == 0)
                return;
            
            avatar
                .ClearItems()
                .SetItems(Set_6.ToArray())
                .Clean()
                .Compile();
        }

        public void Set5()
        {
            if(Set_5.Count == 0)
                return;

            avatar
                .ClearItems()
                .SetItems(Set_5.ToArray())
                .Clean()
                .Compile();
        }

        public void Set4()
        {
            if(Set_4.Count == 0)
                return;

            avatar
                .ClearItems()
                .SetItems(Set_4.ToArray())
                .Clean()
                .Compile();
        }

        public void Set3()
        {
            if(Set_3.Count == 0)
                return;

            avatar
                .ClearItems()
                .SetItems(Set_3.ToArray())
                .Clean()
                .Compile();
        }

        public void Set2()
        {
            if(Set_2.Count == 0)
                return;

            avatar
                .ClearItems()
                .SetItems(Set_2.ToArray())
                .Clean()
                .Compile();
        }

        public void Set1()
        {
            if(Set_1.Count == 0)
                return;

            avatar
                .ClearItems()
                .SetItems(Set_1.ToArray())
                .Clean()
                .Compile();
        }

        public void Clear()
        {
            //remove all items of this types from avatar
            avatar
                .ClearItems()
                .Clean()
                .Compile();
        }
    }
}
