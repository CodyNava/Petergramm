using UnityEngine;
using UnityEngine.Events;

namespace _01_Scripts._08_GlobalManager.TowerEventDispatcher
{
    public class TowerEventDispatcher : MonoBehaviour
    {
        public UnityEvent<string> onTowerSpawn = new();

        public void OnButtonClicked(string towerName)
        {
            onTowerSpawn.Invoke(towerName);
        }
    }
}