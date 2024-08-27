using UnityEngine;

public class DeleteArea : MonoBehaviour
{
    public RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}