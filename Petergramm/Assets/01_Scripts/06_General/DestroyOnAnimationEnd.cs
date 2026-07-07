using UnityEngine;

namespace _01_Scripts._06_General
{
    public class DestroyOnAnimationEnd : MonoBehaviour
    {
       public void DestroyOnAnimEnd()
       {
          Destroy(gameObject);//todo pooling later
       }
    }
}
