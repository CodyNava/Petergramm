using UnityEngine;

namespace _01_Scripts._08_GlobalManager.TowerEventDispatcher
{
    public class ButtonTowerLinker : MonoBehaviour
    {
        public string towerName;
        public TowerEventDispatcher dispatcher;

        public void OnButtonClicked()
        {
            if (dispatcher != null)
                dispatcher.OnButtonClicked(towerName);
        }
    }
}
