using System.Collections.Generic;
using _01_Scripts._08_GlobalManager.DamageRules;
using UnityEngine;

namespace _01_Scripts._07_Enemy.Data
{
   [CreateAssetMenu (menuName = "Enemy/Enemy Stats")]
   public class EnemyBaseSO : ScriptableObject
   {
      public DamageEquationDataSO damageRules;
      public EnemySpawnDataSO  waveData;
      public GameObject prefab;
      public string enemyName;
      public int enemyId;
      
      public List<EnemyPassive> passives = new();
      public List<EnemyAbility> abilities = new();
      public EnemyStats stats;
   }
}
