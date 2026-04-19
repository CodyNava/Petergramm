using UnityEngine;

namespace _01_Scripts._01_Tower.Data
{
   [CreateAssetMenu(fileName = "ProjectileSO", menuName = "TD/ProjectileSO")]
   public class ProjectileSO : ScriptableObject
   {
      public GameObject projectilePrefab;
      public float speed;
   }
}