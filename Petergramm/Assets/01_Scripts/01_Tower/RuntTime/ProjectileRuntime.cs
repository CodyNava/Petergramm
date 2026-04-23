using System;
using _01_Scripts._01_Tower.Data;
using UnityEngine;

namespace _01_Scripts._01_Tower.RuntTime
{
    public class ProjectileRuntime : MonoBehaviour
    {
        
        [SerializeField] private byte flySpeed;
        [SerializeField] private byte damageType;
        [SerializeField] private short damage;

        public short Damage => damage;
        public byte DamageType => damageType;
        public void ApplyStats(byte type, byte speed, short dmg)
        {
            damageType = type;
            flySpeed = speed;
            damage = dmg;
        }

        private void Update()
        {
            
        }

        private void MoveToTarget()
        {
            
            //TODO Tracking einbauen, am besten nicht jeden tick sondern eher sonder 10 mal pro sec o.ä
            
        }
        
        
        
        
    }
}
