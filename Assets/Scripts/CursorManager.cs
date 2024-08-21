using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D customCursor;
    public Vector2 cursorHotspot = Vector2.zero;

    void Start()
    {
        ChangeCursor(customCursor, cursorHotspot);
    }

    public void ChangeCursor(Texture2D cursorTexture, Vector2 hotspot)
    {
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    public void ResetCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
