using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._01_Tower.Data
{
   [CreateAssetMenu(menuName = "TD/Tower Upgrade")]
   public class TowerUpgradeSO : ScriptableObject
   {
      public string upgradeName;
      public int maxStacks;

      public List<TowerStatModifier> statModifiers = new();
      public List<UpgradeEffectModifier> effectModifiers = new();
   }
}