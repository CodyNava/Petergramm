using System;
using TMPro;
using UnityEngine;

namespace _01_Scripts._01_Tower.Projectiles
{
    public class ProjectileRuntime : MonoBehaviour
    {
        [SerializeField] private byte flySpeed;
        [SerializeField] private byte damageType;
        [SerializeField] private short damage;
        private float _refreshRate;
        private byte _bounceCount;
        private byte _slowPercent;
        [SerializeField] private Transform _target;
        [SerializeField] private ProjectilePooling projectilePooling;

     


        public short Damage => damage;
        public byte DamageType => damageType;
        public byte SlowPercent => _slowPercent;
        public byte BounceCount => _bounceCount;

        private void OnValidate()
        {
            projectilePooling = GetComponentInParent<ProjectilePooling>();
        }

        public void ReturnToPool()
        {
         
            projectilePooling.ReturnToPool(this);
            
        }


        public void ApplyStats(byte type, byte speed, short dmg, byte bounces, byte slow)
        {
            damageType = type;
            flySpeed = speed;
            damage = dmg;
            _bounceCount = bounces;
            _slowPercent = slow;
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

        public void FindTargetToBounce()
        {
            if (_bounceCount == 0) return;
        }

        private void MoveToTarget()
        {
            if (!_target)
            {
                //todo ask for new target
                ReturnToPool();
                return;
            }

            this.transform.position = Vector3.MoveTowards(this.transform.position, _target.position, flySpeed / 100f);

            //TODO Tracking einbauen, am besten nicht jeden tick sondern eher sonder 10 mal pro sec o.ä
        }
    }
}