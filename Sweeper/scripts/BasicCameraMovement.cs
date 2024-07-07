using UnityEngine;
using UnityEngine.InputSystem;

public class BasicCameraMovement : MonoBehaviour
{
    private Camera cam;
    public TileMap tileMap;

    private Vector3 origin;
    private Vector3 difference;
    private Vector3 camera00;
    private Vector3 camera11;
    private float tileSize;
    private bool isClickingRightMB = false;

    private Vector3 mousePosOld;
    private Vector3 mousePosNew;
    private Vector3 mousePosDifference;
    private float totalDifference;
    public static bool isDragging = false;

    private float cameraSizeOld;

    private Vector3 mousePos => cam.ScreenToWorldPoint(mouseScreenPos);
    private Vector3 mouseScreenPos => Mouse.current.position.ReadValue();

    private void Awake()
    {
        cam = Camera.main;
    }
    public void OnDrag(InputAction.CallbackContext ctx)
    {
        if(GameManager.instance != null && tileMap == null)
        {
            tileMap = GameManager.instance.GetTileMap();
            tileSize = tileMap.grid.GetTileSize();
            float gridWidth = tileMap.grid.GetWidth(), gridHeight = tileMap.grid.GetHeight();
            camera00 = new(-gridWidth * tileSize / 2, -gridHeight * tileSize / 2);
            camera11 = new(gridWidth * tileSize / 2, gridHeight * tileSize / 2);
        }

        if (ctx.started)
        {
            isDragging = false;
            totalDifference = 0;
            mousePosOld = Vector3.zero; mousePosNew = Vector3.zero;
        }


        isClickingRightMB = ctx.started || ctx.performed;
    }

    void LateUpdate()
    {

        if (Input.mouseScrollDelta.y > 0 && Application.isFocused)
            cam.orthographicSize *= Input.mouseScrollDelta.y / 1.1f;

        if (Input.mouseScrollDelta.y < 0 && Application.isFocused)
            cam.orthographicSize *= Mathf.Abs(Input.mouseScrollDelta.y * 1.1f);

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 0.05f, 2000);

        if (!isClickingRightMB) { return; }

        if (mousePosNew != Vector3.zero) mousePosOld = mousePosNew;
        else mousePosOld = mouseScreenPos;
        mousePosNew = mouseScreenPos;
        mousePosDifference = mousePosOld - mousePosNew;

        totalDifference += Mathf.Sqrt(Mathf.Pow(mousePosDifference.x, 2) + Mathf.Pow(mousePosDifference.y, 2)) / 10;

        if (totalDifference > 1)
        {
            if(!isDragging)
                origin = mousePos;

            isDragging = true;
            difference = mousePos - cam.transform.position;
            cam.transform.position = new(Mathf.Clamp(origin.x - difference.x, camera00.x, camera11.x), Mathf.Clamp(origin.y - difference.y, camera00.y, camera11.y), cam.transform.position.z);
        }
    }
}
