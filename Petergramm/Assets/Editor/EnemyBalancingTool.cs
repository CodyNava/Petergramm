using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EnemyBalancingTool : EditorWindow
    {
        public string enemyName;
        public float hitPoints;
        public float damage;
        public float speed;
        
        
        [MenuItem("Balancing Tools/Enemy Balancing")]
        public static void ShowWindow()
        {
            GetWindow(typeof(EnemyBalancingTool));
        }
    }
}