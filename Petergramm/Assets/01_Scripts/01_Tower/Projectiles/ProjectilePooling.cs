using System;
using System.Collections.Generic;
using UnityEngine;


namespace _01_Scripts._01_Tower.Projectiles
{
    public class ProjectilePooling : MonoBehaviour
    {
        //Jeder pool wird ein Parent mit den proj als childs sein. 
        public List<ProjectileRuntime> projPool;


        public void ReturnToPool(ProjectileRuntime projectile) => projectile.gameObject.SetActive(false);
        
        
        

        public GameObject GetFromPool()
        {
            foreach (var proj in projPool)
            {
                if (proj.isActiveAndEnabled) continue;

                proj.gameObject.SetActive(true);
                return proj.gameObject;
            }

            return null;
        }
    }

   
}