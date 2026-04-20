using _01_Scripts._01_Tower.Data;
using UnityEngine;

namespace _01_Scripts._01_Tower.RuntTime
{
    public class TowerAttack : MonoBehaviour
    {
        [SerializeField] private TowerRuntime towerRuntime;
        private float _cd;
        void Update()
        {
            if (!towerRuntime || !towerRuntime.TowerBase || !towerRuntime.TowerBase.attackData) return;

            this._cd -= Time.deltaTime;
        
            float attacksPerSecond = this.towerRuntime.CurrentStats.attacksPerSecond;

            if (attacksPerSecond <= 0f) return;
        
            float attackInterval = 1f / attacksPerSecond;

            if (this._cd <= 0f)
            {
                this._cd = attackInterval;
                this.Fire();
            }
        }

        private void Fire()
        {
            TowerStats stats = this.towerRuntime.CurrentStats;
            TowerEffectValues effects = this.towerRuntime.CurrentEffects;
            TowerAttackSO attackData = this.towerRuntime.TowerBase.attackData;

            int projectileCount = attackData.baseProjectileCount;
            int additionalTargets = effects.additionalTargets;
            float slowPercent = effects.slowPercent;
            int bounceCount = effects.bounceCount;
        
            //TODO ATTACKING
            
            Debug.Log($"{this.name} DMG {stats.damage}, Range {stats.range}, ");
        }
    }
}
