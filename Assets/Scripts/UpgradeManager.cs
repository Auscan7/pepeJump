using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    // References to the UI buttons and texts
    public Button healthUpgradeButton;
    public Button damageUpgradeButton;
    public Button armorUpgradeButton;

    [Header("Health Upgrade")]
    public TextMeshProUGUI healthCurrentValueText;
    public TextMeshProUGUI healthUpgradeValueText;
    public TextMeshProUGUI healthUpgradePriceText;
    public TextMeshProUGUI healthUpgradeLevelText;
    public TextMeshProUGUI healthMaxedText;

    [Space(5)]

    [Header("Damage Upgrade")]
    public TextMeshProUGUI damageCurrentValueText;
    public TextMeshProUGUI damageUpgradeValueText;
    public TextMeshProUGUI damageUpgradePriceText;
    public TextMeshProUGUI damageUpgradeLevelText;
    public TextMeshProUGUI damageMaxedText;

    [Space(5)]

    [Header("Armor Upgrade")]
    public TextMeshProUGUI armorCurrentValueText;
    public TextMeshProUGUI armorUpgradeValueText;
    public TextMeshProUGUI armorUpgradePriceText;
    public TextMeshProUGUI armorUpgradeLevelText;
    public TextMeshProUGUI armorMaxedText;

    [Space(5)]

    // Lists to store the upgrade prices and values
    [Header("Price and Stats")]
    public List<int> healthUpgradePrices = new List<int>();
    public List<float> healthUpgradeValues = new List<float>();

    public List<int> damageUpgradePrices = new List<int>();
    public List<float> damageUpgradeValues = new List<float>();

    public List<int> armorUpgradePrices = new List<int>();
    public List<float> armorUpgradeValues = new List<float>();

    // Current upgrade levels
    private int healthUpgradeLevel = 0;
    private int damageUpgradeLevel = 0;
    private int armorUpgradeLevel = 0;

    // Max levels for each upgrade
    public int maxHealthUpgradeLevel = 10;
    public int maxDamageUpgradeLevel = 10;
    public int maxArmorUpgradeLevel = 10;

    // Reference to the "Maxed" text
    public TextMeshProUGUI maxedText;

    // Reference to the MoneyManager
    private MoneyManager moneyManager;

    private Player player;

    void Start()
    {
        player = GameObject.FindObjectOfType<Player>();
        moneyManager = MoneyManager.Instance;

        // Initialize the UI texts and buttons
        InitializeUI();
        LoadUpgrades();
        
    }

    void InitializeUI()
    {
        // Set the initial values and texts
        healthCurrentValueText.text = player.maxHealth.ToString();
        damageCurrentValueText.text = player.damage.ToString();
        armorCurrentValueText.text = player.armor.ToString();

        // Set the upgrade prices and values
        healthUpgradePriceText.text = "$" + healthUpgradePrices[0].ToString();
        healthUpgradeValueText.text = "+" + healthUpgradeValues[0].ToString();

        damageUpgradePriceText.text = "$" + damageUpgradePrices[0].ToString();
        damageUpgradeValueText.text = "+" + damageUpgradeValues[0].ToString();

        armorUpgradePriceText.text = "$" + armorUpgradePrices[0].ToString();
        armorUpgradeValueText.text = "+" + armorUpgradeValues[0].ToString();

        // Set the upgrade levels
        healthUpgradeLevelText.text = "Lvl" + healthUpgradeLevel;
        damageUpgradeLevelText.text = "Lvl" + damageUpgradeLevel;
        armorUpgradeLevelText.text = "Lvl" + armorUpgradeLevel;

        // Add listeners to the upgrade buttons
        healthUpgradeButton.onClick.AddListener(OnHealthUpgradeButtonClick);
        damageUpgradeButton.onClick.AddListener(OnDamageUpgradeButtonClick);
        armorUpgradeButton.onClick.AddListener(OnArmorUpgradeButtonClick);
    }

    void OnHealthUpgradeButtonClick()
    {
        if (healthUpgradeLevel < maxHealthUpgradeLevel && moneyManager.currentMoney >= healthUpgradePrices[healthUpgradeLevel])
        {
            // Upgrade the health
            player.maxHealth += healthUpgradeValues[healthUpgradeLevel];
            player.currentHealth += healthUpgradeValues[healthUpgradeLevel];
            healthCurrentValueText.text = player.maxHealth.ToString();
            player.UpdateHealthBar();

            // Update the upgrade level and price
            healthUpgradeLevel++;
            healthUpgradePriceText.text = "$" + healthUpgradePrices[healthUpgradeLevel].ToString();
            healthUpgradeValueText.text = "+" + healthUpgradeValues[healthUpgradeLevel].ToString();
            healthUpgradeLevelText.text = "Lvl" + healthUpgradeLevel;

            // Deduct the money
            moneyManager.currentMoney -= healthUpgradePrices[healthUpgradeLevel - 1];
            moneyManager.UpdateMoneyUI();
            SaveUpgrades();
            // Save the money
            moneyManager.SaveMoney();

            // Reset the maxed text
            healthMaxedText.text = "";
        }
        else if (healthUpgradeLevel >= maxHealthUpgradeLevel)
        {
            // Disable the health upgrade button and UI
            healthUpgradeButton.interactable = false;
            healthUpgradePriceText.text = "";
            healthUpgradeValueText.text = "";
            healthUpgradeLevelText.text = "";
            healthMaxedText.text = "Maxed";
        }
    }

    void OnDamageUpgradeButtonClick()
    {
        if (damageUpgradeLevel < maxDamageUpgradeLevel && moneyManager.currentMoney >= damageUpgradePrices[damageUpgradeLevel])
        {
            // Upgrade the damage
            player.damage += damageUpgradeValues[damageUpgradeLevel];
            damageCurrentValueText.text = player.damage.ToString();

            // Update the upgrade level and price
            damageUpgradeLevel++;
            damageUpgradePriceText.text = "$" + damageUpgradePrices[damageUpgradeLevel].ToString();
            damageUpgradeValueText.text = "+" + damageUpgradeValues[damageUpgradeLevel].ToString();
            damageUpgradeLevelText.text = "Lvl" + damageUpgradeLevel;

            // Deduct the money
            moneyManager.currentMoney -= damageUpgradePrices[damageUpgradeLevel - 1];
            moneyManager.UpdateMoneyUI();
            SaveUpgrades();
            // Save the money
            moneyManager.SaveMoney();

            // Reset the maxed text
            damageMaxedText.text = "";
        }
        else if (damageUpgradeLevel >= maxDamageUpgradeLevel)
        {
            // Disable the damage upgrade button and UI
            damageUpgradeButton.interactable = false;
            damageUpgradePriceText.text = "";
            damageUpgradeValueText.text = "";
            damageUpgradeLevelText.text = "";
            damageMaxedText.text = "Maxed";
        }
    }

    void OnArmorUpgradeButtonClick()
    {
        if (armorUpgradeLevel < maxArmorUpgradeLevel && moneyManager.currentMoney >= armorUpgradePrices[armorUpgradeLevel])
        {
            // Upgrade the armor
            player.armor += armorUpgradeValues[armorUpgradeLevel];
            armorCurrentValueText.text = player.armor.ToString();

            // Update the upgrade level and price
            armorUpgradeLevel++;
            armorUpgradePriceText.text = "$" + armorUpgradePrices[armorUpgradeLevel].ToString();
            armorUpgradeValueText.text = "+" + armorUpgradeValues[armorUpgradeLevel].ToString();
            armorUpgradeLevelText.text = "Lvl" + armorUpgradeLevel;

            // Deduct the money
            moneyManager.currentMoney -= armorUpgradePrices[armorUpgradeLevel - 1];
            moneyManager.UpdateMoneyUI();
            SaveUpgrades();
            // Save the money
            moneyManager.SaveMoney();

            // Reset the maxed text
            armorMaxedText.text = "";
        }
        else if (armorUpgradeLevel >= maxArmorUpgradeLevel)
        {
            // Disable the armor upgrade button and UI
            armorUpgradeButton.interactable = false;
            armorUpgradePriceText.text = "";
            armorUpgradeValueText.text = "";
            armorUpgradeLevelText.text = "";
            armorMaxedText.text = "Maxed";
        }
    }
    private void SaveUpgrades()
    {
        // Save the upgrade levels
        PlayerPrefs.SetInt("HealthUpgradeLevel", healthUpgradeLevel);
        PlayerPrefs.SetInt("DamageUpgradeLevel", damageUpgradeLevel);
        PlayerPrefs.SetInt("ArmorUpgradeLevel", armorUpgradeLevel);
        PlayerPrefs.Save();
    }
    private void LoadUpgrades()
    {
        // Load the upgrade levels
        healthUpgradeLevel = PlayerPrefs.GetInt("HealthUpgradeLevel", 0);
        damageUpgradeLevel = PlayerPrefs.GetInt("DamageUpgradeLevel", 0);
        armorUpgradeLevel = PlayerPrefs.GetInt("ArmorUpgradeLevel", 0);

        // Update the player's stats based on the loaded upgrade levels
        player.maxHealth = 100f + healthUpgradeValues.GetRange(0, healthUpgradeLevel).Sum();
        player.currentHealth = 100f + healthUpgradeValues.GetRange(0, healthUpgradeLevel).Sum();
        player.damage = 20f + damageUpgradeValues.GetRange(0, damageUpgradeLevel).Sum();
        player.armor = 5f + armorUpgradeValues.GetRange(0, armorUpgradeLevel).Sum();

        // Update the UI with the loaded upgrade levels
        healthUpgradeLevelText.text = "Lvl" + healthUpgradeLevel;
        damageUpgradeLevelText.text = "Lvl" + damageUpgradeLevel;
        armorUpgradeLevelText.text = "Lvl" + armorUpgradeLevel;

        // Update the price and value texts based on the loaded upgrade levels
        healthUpgradePriceText.text = "$" + healthUpgradePrices[healthUpgradeLevel].ToString();
        healthUpgradeValueText.text = "+" + healthUpgradeValues[healthUpgradeLevel].ToString();

        damageUpgradePriceText.text = "$" + damageUpgradePrices[damageUpgradeLevel].ToString();
        damageUpgradeValueText.text = "+" + damageUpgradeValues[damageUpgradeLevel].ToString();

        armorUpgradePriceText.text = "$" + armorUpgradePrices[armorUpgradeLevel].ToString();
        armorUpgradeValueText.text = "+" + armorUpgradeValues[armorUpgradeLevel].ToString();


        // Update the current value texts in UI
        healthCurrentValueText.text = player.maxHealth.ToString();
        damageCurrentValueText.text = player.damage.ToString();
        armorCurrentValueText.text = player.armor.ToString();

        // Update Health Upgrade UI
        if (healthUpgradeLevel < maxHealthUpgradeLevel)
        {
            healthUpgradeLevelText.text = "Lvl" + healthUpgradeLevel;
            healthUpgradePriceText.text = "$" + healthUpgradePrices[healthUpgradeLevel].ToString();
            healthUpgradeValueText.text = "+" + healthUpgradeValues[healthUpgradeLevel].ToString();
            healthMaxedText.text = "";  // Clear maxed text
            healthUpgradeButton.interactable = true;  // Enable button
        }
        else
        {
            // Maxed out
            healthUpgradeLevelText.text = "";
            healthUpgradePriceText.text = "";
            healthUpgradeValueText.text = "";
            healthMaxedText.text = "Maxed";
            healthUpgradeButton.interactable = false;  // Disable button
        }

        // Update Damage Upgrade UI
        if (damageUpgradeLevel < maxDamageUpgradeLevel)
        {
            damageUpgradeLevelText.text = "Lvl" + damageUpgradeLevel;
            damageUpgradePriceText.text = "$" + damageUpgradePrices[damageUpgradeLevel].ToString();
            damageUpgradeValueText.text = "+" + damageUpgradeValues[damageUpgradeLevel].ToString();
            damageMaxedText.text = "";  // Clear maxed text
            damageUpgradeButton.interactable = true;  // Enable button
        }
        else
        {
            // Maxed out
            damageUpgradeLevelText.text = "";
            damageUpgradePriceText.text = "";
            damageUpgradeValueText.text = "";
            damageMaxedText.text = "Maxed";
            damageUpgradeButton.interactable = false;  // Disable button
        }

        // Update Armor Upgrade UI
        if (armorUpgradeLevel < maxArmorUpgradeLevel)
        {
            armorUpgradeLevelText.text = "Lvl" + armorUpgradeLevel;
            armorUpgradePriceText.text = "$" + armorUpgradePrices[armorUpgradeLevel].ToString();
            armorUpgradeValueText.text = "+" + armorUpgradeValues[armorUpgradeLevel].ToString();
            armorMaxedText.text = "";  // Clear maxed text
            armorUpgradeButton.interactable = true;  // Enable button
        }
        else
        {
            // Maxed out
            armorUpgradeLevelText.text = "";
            armorUpgradePriceText.text = "";
            armorUpgradeValueText.text = "";
            armorMaxedText.text = "Maxed";
            armorUpgradeButton.interactable = false;  // Disable button
        }
    }
}