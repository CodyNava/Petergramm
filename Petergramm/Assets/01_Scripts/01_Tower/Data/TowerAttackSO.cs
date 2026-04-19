using UnityEngine;

namespace _01_Scripts._01_Tower.Data
{
   [CreateAssetMenu(menuName = "TD/Tower Attack")]
   public class TowerAttackSO : ScriptableObject
   {
      public ProjectileSO projectile;
      public int baseProjectileCount = 1;
   }
}