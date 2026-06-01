using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance;

    public int collectedGas = 0;
    public int requiredGas = 20;

    public TextMeshProUGUI gasText;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void CollectGas()
    {
        collectedGas++;

        if (collectedGas > requiredGas)
            collectedGas = requiredGas;

        UpdateUI();
    }

    void UpdateUI()
    {
        gasText.text = "Benzin: " + collectedGas + "/" + requiredGas;
    }
}