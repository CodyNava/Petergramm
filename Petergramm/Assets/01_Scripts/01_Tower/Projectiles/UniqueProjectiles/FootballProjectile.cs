using _01_Scripts._08_GlobalManager.Pooling;
using Unity.Profiling;

namespace _01_Scripts._01_Tower.Projectiles.UniqueProjectiles
{
   public class FootballProjectile : ProjectileRuntime<FootballProjectile>
   {
      private static readonly ProfilerMarker DetectionMarker = new ProfilerMarker("Proj_DetectionMarkerFootballProjectile");
      private void FixedUpdate()
      {
         Refresh();
         Detection();
         // if (!Target) ReturnToPool();
      }
      
      private void Detection()
      {
         using var _= DetectionMarker.Auto();
         
         if (ValidateEnemies()) { ReturnToPool(); return; }
         if (!DetectCollisions()) return;
         CollisionCaller(this);
         if (FindTargetToBounce()) return;
         ReturnToPool();
      }
   }
}