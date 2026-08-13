using UnityEngine;
using UnityEngine.UI;

public class ChestRewardManager : MonoBehaviour
{
    public static ChestRewardManager Instance;

    [Header("UI")]
    public GameObject chestRewardPanel;
    public RelicTooltipTrigger relicTooltip;

    private RelicData currentRelic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        chestRewardPanel.SetActive(false);
    }

    // Called when the player clicks a Chest Node
    public void OpenChest()
    {
        currentRelic = RelicManager.Instance.GetRandomChestRelic();

        if (currentRelic == null)
        {
            Debug.Log("No more relics to receive.");
            return;
        }

        relicTooltip.SetRelic(currentRelic);

        chestRewardPanel.SetActive(true);
    }

    // Attach to RelicIcon's OnClick
    public void TakeRelic()
    {
        if (currentRelic == null)
            return;

        RelicManager.Instance.AddRelic(currentRelic);

        chestRewardPanel.SetActive(false);

        currentRelic = null;
    }

    public void CloseChest()
    {
        currentRelic = null;
        chestRewardPanel.SetActive(false);
    }
}