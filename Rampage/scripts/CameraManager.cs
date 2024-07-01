using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;
    public Camera cam;
    public float cameraZ = -100;
    public GameObject bg;
    SpriteRenderer bgsr;
    public CanvasScaler canvasScaler;
    public Vector2 cameraMin;
    public Vector2 cameraMax;

    private void Awake()
    {
        Instance = this;
        cam = Camera.main;
        bgsr = bg.GetComponent<SpriteRenderer>();
        canvasScaler.scaleFactor = cam.pixelWidth / 1920;
    }
    void Update()
    {
        Vector3 cameraNewPos = Player.instance.GetPlayerPos();
        cameraNewPos.z = cameraZ;

        if (cam == null)
            Camera.main.transform.position = cameraNewPos;
        else
            cam.transform.position = cameraNewPos;

        if (Input.mouseScrollDelta.y > 0 && Application.isFocused)
            cam.orthographicSize *= Input.mouseScrollDelta.y / 1.05f;

        if (Input.mouseScrollDelta.y < 0 && Application.isFocused)
            cam.orthographicSize *= Mathf.Abs(Input.mouseScrollDelta.y * 1.05f);

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 10f, 50f);

        float bgSize = bg.transform.localScale.x * bgsr.size.x;
        cameraMin = new(-bgSize / 2, -bgSize / 2);
        cameraMax = new(bgSize / 2, bgSize / 2);
        Vector3 camera00 = cam.ScreenToWorldPoint(new(0, 0));
        Vector3 camera11 = cam.ScreenToWorldPoint(new(cam.pixelWidth, cam.pixelHeight));
        Vector3 centerToTopRightAngle = (camera11 - camera00) / 2;
        canvasScaler.scaleFactor = (float)cam.pixelWidth / 1920f;

        cam.transform.position = new(Mathf.Clamp(cam.transform.position.x, cameraMin.x + centerToTopRightAngle.x, cameraMax.x - centerToTopRightAngle.x),
                                     Mathf.Clamp(cam.transform.position.y, cameraMin.y + centerToTopRightAngle.y, cameraMax.y - centerToTopRightAngle.y),
                                     transform.position.z);
    }
}


