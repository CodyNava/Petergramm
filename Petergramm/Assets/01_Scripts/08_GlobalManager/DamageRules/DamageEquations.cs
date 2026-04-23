using System;
using System.Collections.Generic;
using _01_Scripts._01_Tower.Data;
using _01_Scripts._07_Enemy.Data;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.DamageRules
{
   
   [Serializable]
   public struct DamageArmorModifier
   {
      public EnemyArmorTypes armorTypes;
      [Tooltip("1 = 100%, 2 = 200%")]
      public float multiplier;
   }
   
   [Serializable]
   public struct DamageCalcList
   {
      public TowerDamageType damageType;
      public List<DamageArmorModifier>  modifiers;
   }
}

