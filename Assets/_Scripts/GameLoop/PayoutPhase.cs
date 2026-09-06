using UnityEngine;
using UnityEngine.UI;

public class PayoutPhase : MonoBehaviour
{
    public static PayoutPhase Instance;

    [Header("UI")]
    public GameObject payoutPanel;
    public Text resultText;
    public Text payoutText;
    public Text cargoText;
    public Text penaltyText;
    public Button returnButton;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        returnButton.onClick.AddListener(ReturnToPrepare);
        payoutPanel.SetActive(false);
    }

    public void Show(float payout, float cargo, float penalty, bool won)
    {
        payoutPanel.SetActive(true);
        resultText.text = won ? "QUOTA MET" : "QUOTA FAILED";
        resultText.color = won ? Color.green : Color.red;
        payoutText.text = $"Payout: ${payout:F0}";
        cargoText.text = $"Cargo Bonus: ${cargo:F0}";
        penaltyText.text = $"Penalties: ${penalty:F0}";
    }

    private void ReturnToPrepare()
    {
        // Reload scene or reset state
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}