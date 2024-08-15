using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // Import TextMeshPro namespace

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public TextMeshProUGUI moneyText; // Updated to use TextMeshProUGUI
    private int currentMoney = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadMoney();
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyUI();
        SaveMoney();
    }

    private void UpdateMoneyUI()
    {
        moneyText.text = currentMoney.ToString();
    }

    private void SaveMoney()
    {
        PlayerPrefs.SetInt("PlayerMoney", currentMoney);
        PlayerPrefs.Save();
    }

    private void LoadMoney()
    {
        currentMoney = PlayerPrefs.GetInt("PlayerMoney", 0);
    }
}
