using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EnemyCreationTool : EditorWindow
    {
        public string enemyName;
        
        
        [MenuItem("Creation Tools/Enemy Creation")]
        public static void ShowWindow()
        {
            GetWindow(typeof(EnemyCreationTool));
        }
    }
}