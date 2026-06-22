using System;
using System.Collections.Generic;
using UnityEngine;


namespace _01_Scripts._01_Tower.Projectiles
{
    public class ProjectilePooling : MonoBehaviour
    {
        [SerializeField] private List<ProjectileRuntime> freshPool;
        private HashSet<ProjectileRuntime> _gravePool;


        //todo Linear -> Constant : foreach weg, für used proj ein hashset anlegen, immer das letzte vom unused removen und nutzn dann ins hashset adden
        //todo wenn dieses dann nach nutzung wieder zurück in den unused pool (wichtig immer das letzt object herausnehmen, damit kaun aufrücken uccured.
        
        //aktuelles Problem: Tower müssen dieses MonoBehaviour als ref bekommen, was in realtime beim tower platzieren, nicht ohne teures suchen geht.
        
        

        private void Start() => _gravePool = new HashSet<ProjectileRuntime>();


        public ProjectileRuntime GetFromPool()
        {
            var m = freshPool[^1];

            _gravePool.Add(m);
            freshPool.Remove(m);

            return m;
        }

        public void ReturnToPool(ProjectileRuntime projectile)
        {
            freshPool.Add(projectile);
            _gravePool.Remove(projectile); //o(1) ?
        }
    }
}