using System.Collections.Generic;
using _01_Scripts._07_Enemy.Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EnemyCreationTool : EditorWindow
    {
        //General Settings
        [SerializeField] private string enemyName;
        [SerializeField] private int hitpoints;
        [SerializeField] private float damage;
        [SerializeField] private float range;
        [SerializeField] private float attacksPerSecond;
        [SerializeField] private EnemyMovementTypes movement;
        [SerializeField] private EnemyArmorTypes armor;

        //Passive Settings
        [SerializeField] private EnemyPassiveTypes passive;
        [SerializeField] private float passiveStrength;

        private bool _showPassives;
        private bool _newPassive;

        //Abilitiy Settings
        [SerializeField] private EnemyAbilityTypes ability;
        [SerializeField] private float amount;
        [SerializeField] private float frequency;
        [SerializeField] private GameObject summonPrefab;

        private bool _showAbilities;
        private bool _newAbility;

        //For Creation
        private EnemyBaseSO _createdEnemyBase;
        private readonly List<EnemyPassive> _passives = new();
        private readonly List<EnemyAbility> _abilities = new();

        //Editor Window
        private Page _currentPage;

        //Warnings
        private readonly List<string> _generalWarnings = new();
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
                CheckValidInput();
                if (_generalWarnings.Count == 0)
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
                else
                {
                    ShowWarning(_generalWarnings);
                }
            }
        }

        #endregion

        #region Setup

        private void EnemySetup()
        {
            throw new System.NotImplementedException();
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
                        CheckValidInput();
                        if (_passiveWarnings.Count == 0)
                        {
                            var newPassive = new EnemyPassive
                            {
                                enemyPassive = passive,
                                effectValue = passiveStrength
                            };
                            _passives.Add(newPassive);
                            _newPassive = false;
                        }
                        else
                        {
                            ShowWarning(_passiveWarnings);
                        }
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
                        "What monster is summoned by the ability"), summonPrefab, typeof(GameObject), false) as GameObject;
                    if (GUILayout.Button("Add Ability"))
                    {
                        CheckValidInput();
                        if (_abilityWarnings.Count == 0)
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
                        else
                        {
                            ShowWarning(_abilityWarnings);
                        }
                    }
                }
            }
        }

        #endregion

        #region Creation

        private void CreateEnemy()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region InputValidation

        private void CheckValidInput()
        {
            _generalWarnings.Clear();
            _passiveWarnings.Clear();
            _abilityWarnings.Clear();
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