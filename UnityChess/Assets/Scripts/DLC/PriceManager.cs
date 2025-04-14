using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PriceManager : MonoBehaviour
{
    public GameObject MoneyAmount;
    public bool PurchaseItem(float price)
    {
        TMP_Text moneyText = MoneyAmount.GetComponent<TMP_Text>();
        float.TryParse(moneyText.text, out float availableMoney);
        if (availableMoney >= price)
        {
            StartCoroutine(DecreaseNumberGradually(moneyText, availableMoney,
                availableMoney - price, 2));
            return true;
        }

        return false;
    }

    IEnumerator DecreaseNumberGradually(TMP_Text moneyText, float startValue, float endValue, float totalTime)
    {
        float elapsedTime = 0f;
        float currentValue = startValue;
        float rateOfChange = 0f;

        while (currentValue > endValue)
        {
            elapsedTime += Time.deltaTime;
            rateOfChange = Mathf.Lerp(0f, 1f, elapsedTime / totalTime);
            currentValue -= rateOfChange * Time.deltaTime * (startValue - endValue) / totalTime;

            moneyText.text = Mathf.RoundToInt(currentValue).ToString();

            yield return null;
        }

        // Ensure the final value is exactly the endValue
        moneyText.text = endValue.ToString();
    }
}
