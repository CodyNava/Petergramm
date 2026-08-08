using System.Collections;
using System.Collections.Generic;
using _01_Scripts._01_Tower.Projectiles;
using _01_Scripts._01_Tower.Projectiles.UniqueProjectiles;
using _01_Scripts._08_GlobalManager.EnemyList;
using _01_Scripts._08_GlobalManager.Pooling;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts._09_Debugging
{
   public class DebugButtons : MonoBehaviour
   {
      [SerializeField] private TextMeshProUGUI enemyCounter;
      [SerializeField] private TextMeshProUGUI projCounter;


      private void Update()
      {
         RefreshEnemyCounter();
         RefreshProjectileCounter();
      }

      private void RefreshEnemyCounter()
      {
         enemyCounter.text = $"Enemy\nCount\n{EnemyList.EnemyGameObjects.Count.ToString()}";
      }
      
      private void RefreshProjectileCounter()
      {
         projCounter.text = $"Proj\nCount\n{GenericPool<ProjectileRuntime<BasketballProjectile>>.ReturnPoolItemsDebug()}";
      }

      [Button]
      public void StartAnimation()
      {
         
      }
   }
}