using System.Collections.Generic;
using _01_Scripts._02_Grid.GridData;
using _01_Scripts._07_Enemy.Runtime;
using _01_Scripts._08_GlobalManager.GridToEnemyConnector;
using _01_Scripts._08_GlobalManager.Pooling;
using NaughtyAttributes;
using Unity.Profiling;
using UnityEngine;

namespace _01_Scripts._01_Tower.Projectiles
{
   public abstract class ProjectileRuntime : MonoBehaviour
   {
      [SerializeField] private byte flySpeed;
      [SerializeField] private byte damageType;
      [SerializeField] private short damage;
      private float _refreshRate;
      private float _refreshTime;
      private byte _bounceCount;
      private byte _slowPercent;
      private EnemyHealth _target;
      // [SerializeField] private ProjectilePooling projectilePooling;
      private const int BounceRange = 2;
      private readonly HashSet<EnemyHealth> _alreadyBouncedTo = new();

      public short Damage => damage;
      public byte DamageType => damageType;
      public byte SlowPercent => _slowPercent;
      
      private static readonly ProfilerMarker BounceDistMarker = new ProfilerMarker("Proj_BounceDistMarker");
      private static readonly ProfilerMarker BounceContainsMarker = new ProfilerMarker("Proj_BounceContainsMarker");
      private static readonly ProfilerMarker MoveToTargetMarker = new ProfilerMarker("Proj_MoveToTarget");
      private static readonly ProfilerMarker RefreshMarker = new ProfilerMarker("Proj_RefreshMarker");
      private static readonly ProfilerMarker DetectCollisionMarker = new ProfilerMarker("Proj_DetectCollisionMarker");
      private static readonly ProfilerMarker CollisionCallerMarker = new ProfilerMarker("Proj_CollisionCallerMarker");

      //DEBUG//
      private void OnValidate()
      {
         // projectilePooling = GetComponentInParent<ProjectilePooling>();
      }

      private void OnEnable() { _alreadyBouncedTo.Clear(); }

      public void ApplyStats(byte type, byte speed, short dmg, byte bounces, byte slow)
      {
         damageType = type;
         flySpeed = speed;
         damage = dmg;
         _bounceCount = bounces;
         _slowPercent = slow;
         _refreshRate = 0.02f;
      }

      protected void Refresh()
      {
         using var _ = RefreshMarker.Auto();

         _refreshTime -= Time.deltaTime;
         if (_refreshTime > 0) return;

         _refreshTime = _refreshRate;
         MoveToTarget();
      }

      public void GetTarget(EnemyHealth target)
      {
         if (!target || target == _target) return;
         _target = target;
      }

      protected bool DetectCollisions()
      {
         using var _ = DetectCollisionMarker.Auto();
         //var epsilon = new Vector3(0.1f, 0f, 0.1f);
         return this.transform.position == _target.transform.position;
      }

      protected bool ValidateEnemies() => !_target || !_target.isActiveAndEnabled;

      protected void CollisionCaller(ProjectileRuntime proj)
      {
         using var _ = CollisionCallerMarker.Auto();
         _target.Collision(proj);
         _alreadyBouncedTo.Add(_target);
      }

      
      protected bool FindTargetToBounce()
      {
         if (_bounceCount == 0) return false;

         Vector3 currentPos = transform.position;
         float closestSqrDistance = BounceRange * BounceRange;
         EnemyHealth currentClosestEnemy = _target;
         var allEnemiesInRange = currentPos.ToGrid().GetEnemiesInRange(BounceRange);

         for (var i = 0; i < allEnemiesInRange.Count; i++)
         {
            EnemyHealth enemy = allEnemiesInRange[i];
            if (enemy == _target) continue;

            using ProfilerMarker.AutoScope _ = BounceContainsMarker.Auto();

            {
               if (_alreadyBouncedTo.Contains(enemy)) continue;
            }

            using ProfilerMarker.AutoScope __ = BounceDistMarker.Auto();

            {
               Vector3 delta = enemy.transform.position - currentPos;
               float sqrDistance = delta.sqrMagnitude;

               if (sqrDistance > closestSqrDistance) continue;

               closestSqrDistance = sqrDistance;
               currentClosestEnemy = enemy;
            }
         }

         if (currentClosestEnemy == _target)
         {
            _bounceCount = 0;
            return false;
         }

         _alreadyBouncedTo.Add(currentClosestEnemy);
         _target = currentClosestEnemy;
         _bounceCount--;
         return true;
      }

      private void MoveToTarget()
      {
         using var _ = MoveToTargetMarker.Auto();
         this.transform.position = Vector3.MoveTowards(
            this.transform.position,
            _target.transform.position,
            flySpeed * _refreshRate
         );
      }

      public abstract ProjectileRuntime Get();
      public abstract ProjectileRuntime Get(Vector3 position);
   }

   public class ProjectileRuntime<T> : ProjectileRuntime where T : ProjectileRuntime<T>
   {
      public override ProjectileRuntime Get() { return GenericPool<ProjectileRuntime<T>>.GetFromPool(this); }

      public override ProjectileRuntime Get(Vector3 position)
      {
         return GenericPool<ProjectileRuntime<T>>.GetFromPool(this, position);
      }

      protected void ReturnToPool()
      {
         this.gameObject.SetActive(false);
         GenericPool<ProjectileRuntime<T>>.ReturnToPool(this);
      }
   }
}