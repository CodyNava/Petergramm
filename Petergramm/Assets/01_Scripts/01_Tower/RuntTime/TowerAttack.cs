using System;
using System.Collections.Generic;
using _01_Scripts._01_Tower.Data;
using _01_Scripts._01_Tower.Projectiles;
using UnityEditor;
using UnityEngine;

namespace _01_Scripts._01_Tower.RuntTime
{
    public class TowerAttack : MonoBehaviour
    {
        [SerializeField] private TowerRuntime towerRuntime;
        [SerializeField] private Transform _projSpawnTransform;
        [SerializeField] private SphereCollider _rangeCollider;
        [SerializeField] private List<GameObject> _targets;

        private float _cd;

        private void OnValidate()
        {
            towerRuntime = this.GetComponent<TowerRuntime>();
            _projSpawnTransform = this.GetComponentInChildren<Transform>().GetChild(0).GetChild(0).transform;
        }

        // private void OnDrawGizmos()
        // {
        //     Handles.color = new Color(1, 0, 0, 0.25f);
        //     Handles.DrawSolidDisc(this.transform.position, this.transform.up, 
        //                           0.5f + towerRuntime.CurrentStats.range);
        // }

        private void Update()
        {
            if (!towerRuntime || !towerRuntime.TowerBase || !towerRuntime.TowerBase.attackData) return;
            ValidateTargets();
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

        private void ApplyStatsToProjectiles(TowerAttackSO attackData, TowerStats stats,
            ProjectileRuntime projectileRuntime, TowerEffectValues effectValues)
        {
            var type = attackData.projectile.DamageType;
            var speed = attackData.projectile.speed;
            var damage = stats.damage;
            var bounces = effectValues.bounceCount;
            var slow = effectValues.slowPercent;
            projectileRuntime.ApplyStats(type, speed, damage, (byte)bounces, (byte)slow);
        }


        private GameObject SpawnProjectiles(GameObject projectilePrefab) => Instantiate(projectilePrefab,
            _projSpawnTransform.position, this.transform.rotation);


        private void GiveProjectileTarget(Transform target, ProjectileRuntime projectileRuntime) =>
            projectileRuntime.GetTarget(target);


        private void ValidateTargets()
        {
            for (var i = 0; i < _targets.Count; i++)
            {
                if (!_targets[i]) _targets.RemoveAt(i);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            for (int i = 0; i < _targets.Count; i++)
            {
                if (_targets[i] == other.gameObject)
                {
                    _targets.RemoveAt(i);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag($"Enemy")) return;

            _targets.Add(other.gameObject);
            Debug.Log($"{this.name} TARGET {_targets[0].name}");
        }


        private void Fire()
        {
            if (_targets.Count <= 0) return;
            TowerStats stats = this.towerRuntime.CurrentStats;
            TowerEffectValues effects = this.towerRuntime.CurrentEffects;
            TowerAttackSO attackData = this.towerRuntime.TowerBase.attackData;

            
            int projectileCount = effects.projectileAmount + stats.baseProjectileAmount;
            float slowPercent = effects.slowPercent;
            int bounceCount = effects.bounceCount;
            var damageType = attackData.projectile.DamageType = (byte)attackData.damageType;


//todo 

            for (int i = 0; i < projectileCount; ++i)
            {
                if (i == _targets.Count) break;
                FireProjectiles(stats, attackData,effects, i);
            }


            Debug.Log($"{this.name} DMG {stats.damage}, Range {stats.range}, DmgType {damageType} ");
            Debug.Log($"{this.name} projectileCount {projectileCount}, additionalTargets {projectileCount}, " +
                      $"slowPercent {slowPercent}, bounceCount {bounceCount} ");
        }

        private void FireProjectiles(TowerStats stats, TowerAttackSO attackData,TowerEffectValues effectValues, int target)
        {
            var projectileObject = SpawnProjectiles(attackData.projectile.projectilePrefab);
            var projectileData = projectileObject.GetComponent<ProjectileRuntime>();

            ApplyStatsToProjectiles(attackData, stats, projectileData, effectValues );
            GiveProjectileTarget(_targets[target].transform, projectileData);
        }
    }
}