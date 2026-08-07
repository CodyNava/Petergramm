using _01_Scripts._08_GlobalManager.Pooling;

namespace _01_Scripts._01_Tower.Projectiles.UniqueProjectiles
{
   public class SoccerballProjectile : ProjectileRuntime<SoccerballProjectile>
   {
      private void FixedUpdate()
      {
         Refresh();
         Detection();
         // if (!Target) ReturnToPool();
      }

      private void Detection()
      {
         if (!DetectCollisions()) return;
         if (FindTargetToBounce()) return;
         ReturnToPool();
      }
   }
}