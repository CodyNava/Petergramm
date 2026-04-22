using UnityEngine;

namespace _01_Scripts._01_Tower.Data
{
   [CreateAssetMenu(menuName = "TD/ProjectileSO")]
   public class ProjectileSO : ScriptableObject
   {
      public GameObject projectilePrefab;
      public float speed;
   }
}