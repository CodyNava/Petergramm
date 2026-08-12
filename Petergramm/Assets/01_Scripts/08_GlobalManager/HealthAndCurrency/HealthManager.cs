using _01_Scripts._10_UI;
using UnityEngine;

namespace _01_Scripts._08_GlobalManager.HealthAndCurrency
{
   public class HealthManager : MonoBehaviour
   {
      [SerializeField] private int lives;
      [SerializeField] private SpriteRenderer[] humans;
      [SerializeField] private GameObject gameOverScreen;
      [SerializeField] private HealthAndCurrencyDisplay healthAndCurrencyDisplay;

      private void OnEnable() { GridToEnemyConnector.GridToEnemyConnector.GoalReached += LooseLife; }

      private void OnDisable() { GridToEnemyConnector.GridToEnemyConnector.GoalReached -= LooseLife; }

      private void Start()
      {
         lives = humans.Length;
         healthAndCurrencyDisplay.RefreshLives(lives);
      }

      private void LooseLife()
      {
         if (lives <= 0) return;
         lives--;
         humans[lives].gameObject.SetActive(false);
         healthAndCurrencyDisplay.RefreshLives(lives);
         if (lives <= 0) GameOver();
      }

      private void GameOver() => gameOverScreen.SetActive(true);
   }
}