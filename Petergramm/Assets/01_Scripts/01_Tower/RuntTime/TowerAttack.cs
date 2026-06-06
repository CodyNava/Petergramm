using System.Collections.Generic;
using _01_Scripts._01_Tower.Data;
using _01_Scripts._01_Tower.Projectiles;
using _01_Scripts._01_Tower.Projectiles.UniqueProjectiles;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.Pooling;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace _01_Scripts._01_Tower.RuntTime
{
    public class TowerAttack : MonoBehaviour
    {
        [SerializeField] private TowerRuntime towerRuntime;
        [SerializeField] private Transform _projSpawnTransform;
        [SerializeField] private List<GameObject> _targets;
        private float _cd;

        private void OnValidate()
        {
            towerRuntime = this.GetComponent<TowerRuntime>();
            _projSpawnTransform = this.GetComponentInChildren<Transform>().GetChild(0).GetChild(0).transform;
        }

       // private void OnDrawGizmos()
       // {
       //     Handles.color = new Color(1, 0, 0, 0.1f);
       //     Handles.DrawSolidDisc(this.transform.position, this.transform.up,
       //         0.5f + towerRuntime.CurrentStats.range);
       // }

        private void Update()
        {
            if (!towerRuntime || !towerRuntime.TowerBase || !towerRuntime.TowerBase.attackData) return;
            this._cd -= Time.deltaTime;


            var attacksPerSecond = this.towerRuntime.CurrentStats.attacksPerSecond;

            if (attacksPerSecond <= 0f) return;
            var attackInterval = 1f / attacksPerSecond;

            if (!(this._cd <= 0f)) return;

            this._cd = attackInterval;
            FindNextTarget();
            this.Fire();
        }

        private void ApplyStatsToProjectiles(TowerAttackSO attackData, TowerStats stats,
            ProjectileRuntime projectileRuntime, TowerEffectValues effectValues)
        {
            var type = (byte)attackData.damageType;
            var speed = attackData.projectile.speed;
            var damage = stats.damage;
            var bounces = effectValues.bounceCount;
            var slow = effectValues.slowPercent;
            projectileRuntime.ApplyStats(type, speed, damage, (byte)bounces, (byte)slow);
        }


        private ProjectileRuntime SpawnProjectiles(TowerAttackSO attackData)
        {
            ProjectileRuntime projectileObject;
            switch (attackData.projectileType)
            {
                case TowerProjectileType.Basketball:
                   projectileObject = GenericPool<BasketballProjectile>.GetFromPool(attackData.projectile.projectilePrefab.gameObject);
                break;
                case TowerProjectileType.Baseball:
                   projectileObject = GenericPool<BaseballProjectile>.GetFromPool(attackData.projectile.projectilePrefab.gameObject);
                break;
                default:
                    projectileObject = GenericPool<ProjectileRuntime>.GetFromPool(attackData.projectile.projectilePrefab.gameObject);
                    break;
            }
            
            projectileObject.gameObject.transform.position = _projSpawnTransform.position;
            projectileObject.gameObject.SetActive(true);
            return projectileObject;
        }

        private void GiveProjectileTarget(Transform target, ProjectileRuntime projectileRuntime) =>
            projectileRuntime.GetTarget(target);

        private void FindNextTarget()
        {
            _targets.Clear();
            var r = towerRuntime.CurrentStats.range;
            var cel = EnemyList.Enemies;
            var pos = transform.position;
            foreach (var enemy in cel)
            {
                if (!enemy) continue;
                if (Distance(pos, enemy.transform.position).magnitude > r) continue;
                _targets.Add(enemy);
            }
        }

        private static Vector3 Distance(Vector3 a, Vector3 b) => b - a;

        private void Fire()
        {
            if (_targets.Count <= 0) return;
            TowerStats stats = this.towerRuntime.CurrentStats;
            TowerEffectValues effects = this.towerRuntime.CurrentEffects;
            TowerAttackSO attackData = this.towerRuntime.TowerBase.attackData;
            
            var projectileCount = effects.projectileAmount + stats.baseProjectileAmount;
            
            for (int i = 0; i < projectileCount; ++i)
            {
                if (i == _targets.Count) break;
                FireProjectiles(stats, attackData, effects, i);
            }
        }

        private void FireProjectiles(TowerStats stats, TowerAttackSO attackData, TowerEffectValues effectValues,
            int target)
        {
            var projectileObject = SpawnProjectiles(attackData);

            ApplyStatsToProjectiles(attackData, stats, projectileObject, effectValues);
            GiveProjectileTarget(_targets[target].transform, projectileObject);
        }
    }
}