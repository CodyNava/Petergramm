using UnityEngine;

namespace _01_Scripts._07_Enemy.Runtime.UniqueEnemies
{
   public class NormalZombie : EnemyRuntime<NormalZombie>
   {
      private void Awake() { Initialize(); }

      private void OnEnable() { OnReenter(); }
      public void Update() { Death(); }

      private void Death()
      {
         if (!CheckIfDead()) return;
         ReturnToPool();
      }
   }
}