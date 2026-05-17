using System;
using UnityEngine;

namespace _01_Scripts._01_Tower.Projectiles
{
    public class ProjectileRuntime : MonoBehaviour
    {
        
        [SerializeField] private byte flySpeed;
        [SerializeField] private byte damageType;
        [SerializeField] private short damage;
        private float _refreshRate;
        [SerializeField] private Transform _target;

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
            _refreshRate -= Time.deltaTime;
            if (_refreshRate > 0) return;
            
            _refreshRate = 0.01f;
            MoveToTarget();
        }

        public void GetTarget(Transform target)
        {
            if (!target || target == _target) return;
            _target = target;
        }
        
        private void MoveToTarget()
        {

            if (!_target)
            {
                //todo ask for new target
                Destroy(gameObject);
                return;
            }
            
            var v = Time.deltaTime * flySpeed;
            this.transform.position = Vector3.MoveTowards(this.transform.position, _target.position, v);
            
            
            
            //TODO Tracking einbauen, am besten nicht jeden tick sondern eher sonder 10 mal pro sec o.ä
            
            
            
        }
        
        
        
        
    }
}
