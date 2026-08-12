using UnityEngine;

namespace _01_Scripts._10_UI
{
   public class ButtonToggle : MonoBehaviour
   {
      private bool _enabled = true;
      [SerializeField] private float yOffset;
      
      public void ToggleButton()
      {
         var lerpY = new Vector3(0f, yOffset, 0f);
         this.gameObject.transform.localPosition += _enabled ? lerpY : -lerpY;
         _enabled = !_enabled;
      }
   }
}
