using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int currentLevel = 1; // Tracks the current level
    public static LevelManager Instance; // Singleton instance

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
        // Ensure the first level is unlocked at the start
        if (!IsLevelUnlocked(1))
        {
            UnlockLevel(1);
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.CloseLevelSelection(); // Close the level selection panel
        }

        if (IsLevelUnlocked(levelIndex))
        {
            currentLevel = levelIndex;
            SceneManager.LoadScene("Level" + levelIndex); // Make sure your scenes are named "Level1", "Level2", etc.
        }
    }

    public void CompleteLevel()
    {
        if (currentLevel < GetTotalLevels())
        {
            UnlockLevel(currentLevel + 1); // Unlock the next level
        }

        // Show the level selection UI after a short delay
        //Invoke("ShowLevelSelection", 2f); // 2-second delay
    }

    private void ShowLevelSelection()
    {
        UIManager.Instance.ShowLevelSelection(); // Assuming UIManager has a ShowLevelSelection method
    }

    public bool IsLevelUnlocked(int levelIndex)
    {
        return PlayerPrefs.GetInt("Level" + levelIndex + "Unlocked", 0) == 1;
    }

    private void UnlockLevel(int levelIndex)
    {
        PlayerPrefs.SetInt("Level" + levelIndex + "Unlocked", 1);
        PlayerPrefs.Save(); // Ensure the data is saved immediately
    }

    private int GetTotalLevels()
    {
        return 3; // Set this to the total number of levels in your game
    }
}

