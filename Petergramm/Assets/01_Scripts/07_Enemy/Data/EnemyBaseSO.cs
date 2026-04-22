using UnityEngine;

namespace _01_Scripts._07_Enemy.Data
{
   [CreateAssetMenu (menuName = "Enemy/Enemy Stats")]
   public class EnemyBaseSO : ScriptableObject
   {
      public GameObject prefab;
      public new string name;
      
      public EnemyPassive[] passives;
      public EnemyAbility[] abilities;
      public EnemyStats stats;
   }
}
