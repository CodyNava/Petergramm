using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime.UniqueEnemies
{
   public class FatZombie : EnemyRuntime<FatZombie>
   {
      private void Awake() { Initialize(); }

      private void OnEnable() { OnReenter(); }
      public void Update() { Death();
         ResetSlow();
      }

      private void Death()
      {
         if (!CheckIfDead()) return;
         ReturnToPool();
      }
   }
}