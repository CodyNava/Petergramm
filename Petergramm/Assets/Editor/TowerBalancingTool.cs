using _01_Scripts._01_Tower.Data;
using UnityEditor;

namespace Editor
{
    public class TowerBalancingTool : EditorWindow
    {
        //todo: erweitern um DamageType
        public TowerBaseSO towerPrefab;
        public string towerName;
        public float hitPoints;
        public float damage;
        public float range;
        public float attacksPerSecond;


        [MenuItem("Balancing Tools/Tower Balancing")]
        public static void ShowWindow()
        {
            GetWindow(typeof(TowerBalancingTool));
        }

        private void OnGUI()
        {
            towerPrefab = EditorGUILayout.ObjectField("Tower", towerPrefab, typeof(TowerBaseSO), false) as TowerBaseSO;
            if (towerPrefab != null)
            {
                towerName = towerPrefab.towerName;
                hitPoints = towerPrefab.baseStats.maxHp;
                damage = towerPrefab.baseStats.damage;
                range = towerPrefab.baseStats.range;
                attacksPerSecond = towerPrefab.baseStats.attacksPerSecond;

                EditorGUILayout.Space();
                EditorGUI.indentLevel = 1;
                
                towerName = EditorGUILayout.TextField("Name", towerName);
                hitPoints = EditorGUILayout.FloatField("HitPoints", hitPoints);
                damage = EditorGUILayout.FloatField("Damage", damage);
                range = EditorGUILayout.FloatField("Range", range);
                attacksPerSecond = EditorGUILayout.Slider("Attacks per Second", attacksPerSecond, 0f, 5f);
                
                EditorGUILayout.Space();
                EditorGUILayout.FloatField("DPS", damage * attacksPerSecond);

                UpdateValues();
            }
        }

        private void UpdateValues()
        {
            towerPrefab.towerName = towerName;
            towerPrefab.baseStats.maxHp = hitPoints;
            towerPrefab.baseStats.damage = damage;
            towerPrefab.baseStats.range = range;
            towerPrefab.baseStats.attacksPerSecond = attacksPerSecond;
        }
    }
}