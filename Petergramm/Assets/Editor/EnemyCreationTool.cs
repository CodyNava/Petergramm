using System.Collections.Generic;
using _01_Scripts._07_Enemy.Data;
using _01_Scripts._07_Enemy.Runtime;
using _01_Scripts._08_GlobalManager.DamageRules;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EnemyCreationTool : EditorWindow
    {
        //General Settings
        [SerializeField] private string enemyName;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private int hitpoints;
        [SerializeField] private float damage;
        [SerializeField] private float range;
        [SerializeField] private float attacksPerSecond;

        //Movement Settings
        [SerializeField] private EnemyMovementTypes movement;
        [SerializeField] private float movementSpeed;

        //Armor Settings
        [SerializeField] private EnemyArmorTypes armor;
        [SerializeField] private float armorStrength;

        //Passive Settings
        [SerializeField] private EnemyPassiveTypes passive;
        [SerializeField] private float passiveStrength;

        private bool _showPassives;
        private bool _newPassive;
        private bool _passiveErrors;

        //Ability Settings
        [SerializeField] private EnemyAbilityTypes ability;
        [SerializeField] private float amount;
        [SerializeField] private float frequency;
        [SerializeField] private GameObject summonPrefab;

        private bool _showAbilities;
        private bool _newAbility;
        private bool _abilityErrors;

        //For Creation
        private EnemyBaseSO _createdEnemyBase;
        private readonly List<EnemyPassive> _passives = new();
        private readonly List<EnemyAbility> _abilities = new();

        //Editor Window
        private Page _currentPage;

        //Warnings
        private readonly List<string> _allWaringins = new();
        private readonly List<string> _passiveWarnings = new();
        private readonly List<string> _abilityWarnings = new();
        private bool _hasErrors;

        private const string EnemySOPath = "Assets/03_SO/Enemies";
        private const string EnemyPrefabPath = "Assets/04_Prefabs/Enemies";

        #region Editor

        [MenuItem("Creation Tools/Enemy Creation")]
        public static void ShowWindow()
        {
            var window = GetWindow<EnemyCreationTool>();
            window.titleContent = new GUIContent("Enemy Creation");

            window.minSize = new Vector2(450, 200);
            window.maxSize = new Vector2(1920, 720);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical(GUILayout.Width(400));

            if (GUILayout.Button("Enemy Settings"))
            {
                _currentPage = Page.Enemy;
            }

            if (GUILayout.Button("Passives"))
            {
                _currentPage = Page.Passives;
            }

            if (GUILayout.Button("Abilities"))
            {
                _currentPage = Page.Abilities;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.BeginVertical();

            switch (_currentPage)
            {
                case Page.Enemy:
                    EnemySetup();
                    break;
                case Page.Passives:
                    PassivesSetup();
                    break;
                case Page.Abilities:
                    AbilitiesSetup();
                    break;
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            if (GUILayout.Button("Create Enemy"))
            {
                _hasErrors = CheckValidInput();
                if (!_hasErrors)
                {
                    var enemyPath = $"{EnemySOPath}/{enemyName}";
                    if (!AssetDatabase.IsValidFolder(enemyPath))
                    {
                        AssetDatabase.CreateFolder(EnemySOPath, enemyName);
                    }

                    var prefabPath = $"{EnemyPrefabPath}/{enemyName}";
                    if (!AssetDatabase.IsValidFolder(prefabPath))
                    {
                        AssetDatabase.CreateFolder(EnemyPrefabPath, enemyName);
                    }

                    CreateEnemy();
                    AssetDatabase.SaveAssets();
                }
            }

            if (_hasErrors)
            {
                ShowWarning(_allWaringins);
            }
        }

        #endregion

        #region Setup

        private void EnemySetup()
        {
            enemyName = EditorGUILayout.TextField(new GUIContent("Enemy Name",
                    "The name this enemy is supposed to have in-game.\n" +
                    "<b>This field cannot be left empty.</b>"),
                enemyName);
            enemyPrefab = EditorGUILayout.ObjectField(new GUIContent("Enemy Prefab",
                    "The visual prefab for this enemy.\n" +
                    "<b>This cannot be left empty.</b>"),
                enemyPrefab, typeof(GameObject), false) as GameObject;
            hitpoints = EditorGUILayout.IntSlider(new GUIContent("Maximum HP",
                    "The amount of hit points this enemy is supposed to have.\n" +
                    "<b>This value cannot be 0.</b>"),
                hitpoints, 0, 100);
            damage = EditorGUILayout.Slider(new GUIContent("Damage",
                    "The amount of damage this enemy is supposed to deal.\n" +
                    "<b>This value cannot be 0.</b>"),
                damage, 0f, 100f);
            range = EditorGUILayout.Slider(new GUIContent("Range",
                    "The attack range of this enemy.\n" +
                    "<b>This value cannot be 0.</b>"),
                range, 0f, 50f);
            attacksPerSecond = EditorGUILayout.Slider(new GUIContent("Attacks per Second",
                    "How many attacks this enemy can do per second.\n" +
                    "<b>This value cannot be 0.</b>"),
                attacksPerSecond, 0f, 20f);
            movement = (EnemyMovementTypes)EditorGUILayout.EnumPopup(new GUIContent("Movement Type",
                "How is this enemy supposed to move."), movement);
            movementSpeed = EditorGUILayout.Slider(new GUIContent("Movement Speed",
                    "How fast is this enemy supposed to be moving?\n" +
                    "<b>This value cannot be 0.</b>"),
                movementSpeed, 0f, 10f);
            armor = (EnemyArmorTypes)EditorGUILayout.EnumPopup(new GUIContent("Armor Type",
                "The type of armor the enemy is supposed to have."), armor);
            armorStrength = EditorGUILayout.Slider(new GUIContent("Armor Strength",
                    "The strength of the armor. A higher strength means less damage.\n" +
                    "<b>This value cannot be 0.</b>"),
                armorStrength, 0f, 10f);
        }

        private void PassivesSetup()
        {
            _showPassives = EditorGUILayout.Foldout(_showPassives, "Passives");

            if (_showPassives)
            {
                if (_passives.Count != 0)
                {
                    EditorGUILayout.BeginVertical();
                    foreach (var item in _passives)
                    {
                        EditorGUILayout.HelpBox(item.enemyPassive.ToString(), MessageType.None);
                    }

                    EditorGUILayout.EndVertical();
                }

                if (!_newPassive)
                {
                    if (GUILayout.Button("Add Passive"))
                    {
                        _newPassive = true;
                    }
                }

                if (_newPassive)
                {
                    passive = (EnemyPassiveTypes)EditorGUILayout.EnumPopup(new GUIContent("Passive Type",
                        "Choose one passive for the enemy."), passive);
                    passiveStrength = EditorGUILayout.Slider(new GUIContent("Passive Strength",
                        "How strong the effect of your passive is."), passiveStrength, 0f, 5f);
                    if (GUILayout.Button("Add Passive"))
                    {
                        _passiveErrors = CheckValidInput();
                        if (!_passiveErrors)
                        {
                            var newPassive = new EnemyPassive
                            {
                                enemyPassive = passive,
                                effectValue = passiveStrength
                            };
                            _passives.Add(newPassive);
                            _newPassive = false;
                        }
                    }

                    if (_passiveErrors)
                    {
                        ShowWarning(_passiveWarnings);
                    }
                }
            }
        }

        private void AbilitiesSetup()
        {
            _showAbilities = EditorGUILayout.Foldout(_showAbilities, "Abilities");

            if (_showAbilities)
            {
                if (_abilities.Count != 0)
                {
                    EditorGUILayout.BeginVertical();
                    foreach (var item in _abilities)
                    {
                        EditorGUILayout.HelpBox(item.enemyAbility.ToString(), MessageType.None);
                    }

                    EditorGUILayout.EndVertical();
                }

                if (!_newAbility)
                {
                    if (GUILayout.Button("Add Ability"))
                    {
                        _newAbility = true;
                    }
                }

                if (_newAbility)
                {
                    ability = (EnemyAbilityTypes)EditorGUILayout.EnumPopup(new GUIContent("Ability",
                        "Choose one ability for the enemy."), ability);
                    amount = EditorGUILayout.Slider(new GUIContent("Amount",
                            "How many times the ability is triggered in a single use."),
                        amount, 0f, 10f);
                    frequency = EditorGUILayout.Slider(new GUIContent("Frequency",
                        "How often the ability is cast."), frequency, 0f, 10f);
                    summonPrefab = EditorGUILayout.ObjectField(new GUIContent("Summon Type",
                            "What monster is summoned by the ability"), summonPrefab, typeof(GameObject),
                        false) as GameObject;
                    if (GUILayout.Button("Add Ability"))
                    {
                        _abilityErrors = CheckValidInput();
                        if (!_abilityErrors)
                        {
                            var newAbility = new EnemyAbility
                            {
                                enemyAbility = ability,
                                amount = amount,
                                frequency = frequency,
                                summonedMonster = summonPrefab
                            };
                            _abilities.Add(newAbility);
                            _newAbility = false;
                        }
                        
                    }
                    if (_abilityErrors)
                    {
                        ShowWarning(_abilityWarnings);
                    }
                }
            }
        }

        #endregion

        #region Creation

        private void CreateEnemy()
        {
            _createdEnemyBase = CreateInstance<EnemyBaseSO>();

            var tempEnemy = new GameObject();
            tempEnemy.AddComponent<EnemyRuntime>();
            tempEnemy.AddComponent<EnemyAttack>();
            tempEnemy.AddComponent<EnemyHealth>();

            var enemyPreFab =
                PrefabUtility.SaveAsPrefabAsset(tempEnemy, $"{EnemyPrefabPath}/{enemyName}/{enemyName}.prefab");
            DestroyImmediate(tempEnemy);

            AssetDatabase.CreateAsset(_createdEnemyBase, $"{EnemySOPath}/{enemyName}/{enemyName}.asset");
            _createdEnemyBase.enemyName = enemyName;
            _createdEnemyBase.damageRules =
                AssetDatabase.LoadAssetAtPath<DamageEquationDataSO>($"Assets/03_SO/EquationRules/DamageRules.asset");
            _createdEnemyBase.prefab = enemyPreFab;
            _createdEnemyBase.stats.maxHp = hitpoints;
            _createdEnemyBase.stats.damage = damage;
            _createdEnemyBase.stats.attacksPerSecond = attacksPerSecond;
            _createdEnemyBase.stats.range = range;
            var newMovement = new EnemyMovement { movementType = movement, moveSpeed = movementSpeed };
            _createdEnemyBase.stats.movement = newMovement;
            var newArmor = new EnemyArmor { armorType = armor, armorValue = armorStrength };
            _createdEnemyBase.stats.armor = newArmor;

            foreach (var pass in _passives)
            {
                _createdEnemyBase.passives.Add(pass);
            }

            foreach (var item in _abilities)
            {
                _createdEnemyBase.abilities.Add(item);
            }

            var runtime = enemyPreFab.GetComponent<EnemyRuntime>();
            var enemyAsset = AssetDatabase.LoadAssetAtPath<EnemyBaseSO>($"{EnemySOPath}/{enemyName}/{enemyName}.asset");
            runtime.EnemyBase = enemyAsset;
            PrefabUtility.SavePrefabAsset(enemyPreFab);
        }

        #endregion

        #region InputValidation

        private bool CheckValidInput()
        {
            _allWaringins.Clear();
            _passiveWarnings.Clear();
            _abilityWarnings.Clear();

            if (AssetDatabase.LoadAssetAtPath<EnemyBaseSO>($"{EnemySOPath}/{enemyName}/{enemyName}.asset"))
            {
                _allWaringins.Add("Enemy already exists.");
            }

            if (string.IsNullOrWhiteSpace(enemyName))
            {
                _allWaringins.Add("Missing Enemy Name.");
            }

            if (enemyPrefab == null)
            {
                _allWaringins.Add("Missing Enemy Prefab.");
            }

            if (hitpoints <= 0)
            {
                _allWaringins.Add("Hit Points cannot be 0.");
            }

            if (damage <= 0)
            {
                _allWaringins.Add("Damage cannot be 0.");
            }

            if (range <= 0)
            {
                _allWaringins.Add("Range cannot be 0.");
            }

            if (attacksPerSecond <= 0)
            {
                _allWaringins.Add("AttacksPerSecond cannot be 0.");
            }

            if (movementSpeed <= 0)
            {
                _allWaringins.Add("Movement speed cannot be 0.");
            }

            if (armorStrength <= 0)
            {
                _allWaringins.Add("ArmorStrength cannot be 0.");
            }

            if (passiveStrength <= 0)
            {
                _allWaringins.Add("Passive Strength cannot be 0.");
                _passiveWarnings.Add("Passive Strength cannot be 0.");
            }

            if (amount <= 0)
            {
                _allWaringins.Add("Ability Amount cannot be 0.");
                _abilityWarnings.Add("Ability Amount cannot be 0.");
            }

            if (frequency <= 0)
            {
                _allWaringins.Add("Ability Frequency cannot be 0.");
                _abilityWarnings.Add("Ability Frequency cannot be 0.");
            }

            return _allWaringins.Count != 0 || _passiveWarnings.Count != 0 || _abilityWarnings.Count != 0;
        }

        private void ShowWarning(List<string> warnings)
        {
            foreach (var warning in warnings)
            {
                EditorGUILayout.HelpBox(warning, MessageType.Error);
            }
        }

        #endregion
    }
}