using System.Collections;
using System.Collections.Generic;
using _01_Scripts._01_Tower.Projectiles;
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
  

      private void Update() { RefreshEnemyCounter(); }

      private void RefreshEnemyCounter()
      {
         enemyCounter.text = $"Enemy\nCount\n{EnemyList.EnemyGameObjects.Count.ToString()}";
      }

      [Button]
      public void StartAnimation()
      {
         
      }
   }
}