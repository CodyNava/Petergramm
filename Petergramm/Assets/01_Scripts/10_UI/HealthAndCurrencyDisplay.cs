using _01_Scripts._08_GlobalManager.HealthAndCurrency;
using TMPro;
using UnityEngine;

namespace _01_Scripts._10_UI
{
    public class HealthAndCurrencyDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI healthText, currencyText;
        
        public void RefreshLives(int value)
        => healthText.text = $"Lives\n {value}";
        public void RefreshCurrency(int value)
        => currencyText.text = $"Gold\n {value}";
        
        
    }
}
