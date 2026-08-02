using UnityEngine;

namespace _01_Scripts._06_General
{
   public class DestroyOnAnimationEnd : MonoBehaviour
   {
      [SerializeField] private GameObject bodyToDisable;
      [SerializeField] private GameObject bodyToReset;

      public void DestroyOnAnimEnd()
      {
         if (!bodyToDisable) bodyToDisable = transform.parent.gameObject;

         bodyToDisable?.SetActive(false);
         RefreshTransformsAfterDeath();
      }

      private void RefreshTransformsAfterDeath()
      {
         bodyToReset.transform.localPosition = Vector3.zero;
         bodyToReset.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
      }
   }
}