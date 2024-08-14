using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for EventSystem

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton instance
    public GameObject levelSelectionPanel; // Panel to show level selection
    public Button toggleButton; // Button to toggle the level selection panel
    public Button[] levelButtons; // Buttons for each level

    private bool isPanelActive = false;

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
        // Ensure the panel is initially inactive
        levelSelectionPanel.SetActive(false);

        // Add listener to the toggle button to open/close the panel
        toggleButton.onClick.AddListener(ToggleLevelSelection);

        // Update the level buttons according to unlocked levels
        UpdateLevelButtons();
    }

    private void ToggleLevelSelection()
    {
        isPanelActive = !isPanelActive;
        levelSelectionPanel.SetActive(isPanelActive);

        if (!isPanelActive)
        {
            // Deselect any currently selected UI element
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ShowLevelSelection()
    {
        // Ensure the panel is active
        levelSelectionPanel.SetActive(true);
        isPanelActive = true;

        // Update the level buttons to reflect current level unlock status
        UpdateLevelButtons();
    }

    public void CloseLevelSelection()
    {
        levelSelectionPanel.SetActive(false);
        isPanelActive = false;
    }

    public void UpdateLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(i + 1);
            levelButtons[i].interactable = isUnlocked;

            int levelIndex = i + 1; // Capture the index for the button
            levelButtons[i].onClick.RemoveAllListeners(); // Clear previous listeners

            if (isUnlocked)
            {
                levelButtons[i].onClick.AddListener(() => LevelManager.Instance.LoadLevel(levelIndex));
            }
        }
    }
}

