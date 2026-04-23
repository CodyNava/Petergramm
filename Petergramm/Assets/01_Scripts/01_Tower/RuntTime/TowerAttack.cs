using _01_Scripts._01_Tower.Data;
using UnityEngine;

namespace _01_Scripts._01_Tower.RuntTime
{
    public class TowerAttack : MonoBehaviour
    {
        [SerializeField] private TowerRuntime towerRuntime;
        
        private float _cd;

        private void OnValidate()=>
            towerRuntime = this.GetComponent<TowerRuntime>();
        

        private void Update()
        {
            if (!towerRuntime || !towerRuntime.TowerBase || !towerRuntime.TowerBase.attackData) return;

            this._cd -= Time.deltaTime;
        
            float attacksPerSecond = this.towerRuntime.CurrentStats.attacksPerSecond;

            if (attacksPerSecond <= 0f) return;
            
            if (this._cd <= 0f)
            {
                this._cd = attacksPerSecond;
                this.Fire();
            }
        }

        private void ApplyStatsToProjectiles(TowerAttackSO attackData, TowerStats stats)
        {
            var projectileRuntime = attackData.projectile.projectilePrefab.GetComponent<ProjectileRuntime>();
            var type = attackData.projectile.DamageType;
            var speed = attackData.projectile.speed;
            var damage = stats.damage;
            
            projectileRuntime.ApplyStats(type,speed,damage);
        }

        private void SpawnProjectiles(GameObject projectilePrefab)
        {
            Instantiate(projectilePrefab, this.transform.position, this.transform.rotation);
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
            var damageType = attackData.projectile.DamageType = (byte)attackData.damageType;
            
            ApplyStatsToProjectiles(attackData, stats);
            SpawnProjectiles(attackData.projectile.projectilePrefab);
            
            
            
            //TODO ATTACKING
            
            Debug.Log($"{this.name} DMG {stats.damage}, Range {stats.range}, DmgType {damageType} ");
            Debug.Log($"{this.name} projectileCount {projectileCount}, additionalTargets {additionalTargets}, " +
                      $"slowPercent {slowPercent}, bounceCount {bounceCount} ");
        }
    }
}
