using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public float levelEndDelay = 2f; // Time to wait before showing the level selection

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Trigger the level end process
            StartCoroutine(LevelComplete());
        }
    }

    private IEnumerator LevelComplete()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(levelEndDelay);

        // Mark the current level as completed and unlock the next level
        LevelManager.Instance.CompleteLevel();

        // Only show the level selection UI if it's not already open
        if (!UIManager.Instance.levelSelectionPanel.activeInHierarchy)
        {
            UIManager.Instance.ShowLevelSelection();
        }
    }
}
