using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._01_Tower.Data
{
   [CreateAssetMenu(menuName = "TD/Tower Base")]
   public class TowerBaseSO : ScriptableObject
   {
      public string towerName;
      public Sprite icon;
      public GameObject towerPrefab;
      public TowerStats baseStats;
      public TowerAttackSO attackData;

      public List<TowerEffectModifier> innateEffects = new();
      public List<TowerUpgradeSO> availableUpgrades = new();
   }
}