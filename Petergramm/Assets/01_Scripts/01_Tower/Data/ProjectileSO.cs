using System;
using UnityEngine;

namespace _01_Scripts._01_Tower.Data
{
   [CreateAssetMenu(menuName = "TD/ProjectileSO")]
   public class ProjectileSO : ScriptableObject
   {
      public GameObject projectilePrefab;
      [NonSerialized] public byte DamageType;
      public byte speed;
   }
}