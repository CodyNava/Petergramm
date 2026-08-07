using System.Collections.Generic;
using _01_Scripts._07_Enemy.Runtime;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.Pooling;
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
      private float _bounceRange = 5f;
      private List<EnemyHealth> _alreadyBouncedTo = new();

      public short Damage => damage;
      public byte DamageType => damageType;
      public byte SlowPercent => _slowPercent;
      public byte BounceCount => _bounceCount;
      public EnemyHealth Target => _target;

      //DEBUG//

      private static readonly ProfilerMarker BounceMarker = new ProfilerMarker("Proj_BounceMarker");
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
      }

      protected bool FindTargetToBounce()
      {
         using (BounceMarker.Auto())
         {
            if (_bounceCount == 0) return false;

            var currentClosestDistance = 1000f;
            var currentClosestEnemy = _target;
            var allEnemies = EnemyList.EnemyHealths;
            var possibleTargetFound = false;

            for (int i = 0; i < allEnemies.Count; i++)
            {
               if (allEnemies[i] == _target) continue;
               if (_alreadyBouncedTo.Contains(allEnemies[i])) continue;

               float dist = Distance(this.gameObject.transform.position, allEnemies[i].transform.position).magnitude;
               if (dist > _bounceRange) continue;

               if (dist < currentClosestDistance) //00000
               {
                  currentClosestDistance = dist;
                  currentClosestEnemy = allEnemies[i];
                  possibleTargetFound = true;
               }
            }

            if (!possibleTargetFound)
            {
               _bounceCount = 0;
               return false;
            }

            _alreadyBouncedTo.Add(currentClosestEnemy);
            _target = currentClosestEnemy;
            _bounceCount--;
            return true;
         }
      }

      private static Vector3 Distance(Vector3 a, Vector3 b) => b - a;

      private void MoveToTarget()
      {
         using var _ = MoveToTargetMarker.Auto();
         this.transform.position = Vector3.MoveTowards(this.transform.position, _target.transform.position, flySpeed * _refreshRate);
         
      }

      public abstract ProjectileRuntime Get();
      public abstract ProjectileRuntime Get(Vector3 position);
      
   }

   public class ProjectileRuntime<T> : ProjectileRuntime where T : ProjectileRuntime<T>
   {
      public override ProjectileRuntime Get()
      {
        return GenericPool<ProjectileRuntime<T>>.GetFromPool(this);
      }
      
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