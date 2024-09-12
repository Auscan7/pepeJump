using UnityEngine;
using TMPro;
using System.Collections;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public TextMeshProUGUI moneyText; // Updated to use TextMeshProUGUI
    private int currentMoney = 0;

    public GameObject moneySpritePrefab; // Reference to the money sprite prefab
    public RectTransform moneyTextTransform; // Reference to the RectTransform of the money text

    public AudioClip moneyAddedSoundEffect; //Reference to the money collection audio

    public int numberOfSprites = 5; // Number of money sprites to spawn
    public float travelSpeed = 1.0f; // Speed of sprite travel
    public float spawnRadius = 0.5f; // Radius for random spawn position

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

    public void AddMoney(int amount, Vector3 enemyPosition)
    {
        currentMoney += amount; // Add the amount to the current money only once
        for (int i = 0; i < numberOfSprites; i++)
        {
            Vector3 randomOffset = new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0);
            Vector3 spawnPosition = enemyPosition + randomOffset;
            GameObject moneySprite = Instantiate(moneySpritePrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(MoveMoneySpriteToUI(moneySprite));
        }
        SaveMoney();
    }

    private IEnumerator MoveMoneySpriteToUI(GameObject moneySprite)
    {
        Vector3 startPosition = moneySprite.transform.position;
        Vector3 endPosition = Camera.main.ScreenToWorldPoint(moneyTextTransform.position);
        endPosition.z = 0;

        float duration = travelSpeed;
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            moneySprite.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        // Ensure the sprite reaches the exact position
        moneySprite.transform.position = endPosition;

        // Destroy the money sprite
        Destroy(moneySprite);

        // Update the money amount and animate the money text
        UpdateMoneyUI();
        StartCoroutine(AnimateMoneyText());

        // Play a sound effect when money is added
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.PlayOneShot(moneyAddedSoundEffect);
        }

        SaveMoney();
    }

    private IEnumerator AnimateMoneyText()
    {
        Vector3 originalScale = moneyTextTransform.localScale;
        Vector3 targetScale = originalScale * 1.35f;
        float duration = 0.13f;

        // Enlarge the money text
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            moneyTextTransform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Shrink the money text back to original size
        startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            moneyTextTransform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        // Ensure it returns to its original scale
        moneyTextTransform.localScale = originalScale;
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
