using System.Collections.Generic;
using _01_Scripts._01_Tower.Data;
using _01_Scripts._07_Enemy.Data;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.DamageRules
{
   [CreateAssetMenu(menuName = "Damage Equations/EquationData")]
   public class DamageEquationDataSO : ScriptableObject
   {
      public List<DamageCalcList> rows = new();

      private float GetMultiplier(TowerDamageType damageType, EnemyArmorTypes armorType)
      {
         foreach (var row in rows)
         {
            if (row.damageType != damageType) continue;

            foreach (var modifier in row.modifiers)
            {
               if (modifier.armorTypes == armorType) return modifier.multiplier;
            }
         }

         return 1f;
      }

      public float GetFinalDamage(float baseDamage, TowerDamageType damageType, EnemyArmorTypes armorType)
      {
         return baseDamage * GetMultiplier(damageType, armorType);
      }




   }
}
