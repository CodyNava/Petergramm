using _01_Scripts._01_Tower.Placement;
using _01_Scripts._10_UI;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.HealthAndCurrency
{
    public class CurrencyManager : MonoBehaviour
    {
        [SerializeField] private int gold;
        [SerializeField] private HealthAndCurrencyDisplay healthAndCurrencyDisplay;
        [SerializeField] private TowerPlacement towerPlacement;
        [SerializeField] private int startingGold;



        private void Start()
        {
            RefreshGold(startingGold);
        }
        private void OnEnable()
        {
            towerPlacement.TowerPlacedOrRemoved +=  RefreshGold;
            GridToEnemyConnector.GridToEnemyConnector.DropGoldAfterDeathEvent += RefreshGold;
        }

        private void OnDisable()
        {
            towerPlacement.TowerPlacedOrRemoved -=  RefreshGold;
            GridToEnemyConnector.GridToEnemyConnector.DropGoldAfterDeathEvent -= RefreshGold;
        }

        private void RefreshGold(float goldGainLose)
        {
            gold += (int)goldGainLose;
            healthAndCurrencyDisplay.RefreshCurrency(gold);
        }


    }
}
