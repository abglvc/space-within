using UnityEngine;

public class CameraController : MonoBehaviour {
    private Transform target;
    private Vector3 newPosition;
    public SpriteRenderer bgSpriteRenderer;
    public float rightCameraOffset;
    public float speed;
    public float xMin;
    
    void Start() {
        target = Player.sng.transform;
        if (target)
            transform.position = new Vector3(Mathf.Max(xMin, target.transform.position.x + rightCameraOffset), transform.position.y,
                transform.position.z);
    }

    void FixedUpdate() {
        if (target) {
            newPosition = new Vector3(Mathf.Max(xMin, target.transform.position.x + rightCameraOffset), transform.position.y,
                transform.position.z);
            transform.position = Vector3.Lerp(transform.position, newPosition, speed * Time.deltaTime);
        }
    }

    public SpriteRenderer AdjustBackgroundImage(Sprite bgSprite) {
        bgSpriteRenderer.sortingOrder = 6;
        bgSpriteRenderer.sprite = bgSprite;
        //BACGROUND IMAGE SET
        float cameraHeight = Camera.main.orthographicSize * 2;
        Vector2 cameraSize = new Vector2(Camera.main.aspect * cameraHeight, cameraHeight);
        Vector2 spriteSize = bgSpriteRenderer.sprite.bounds.size;
        
        Vector2 scale = bgSpriteRenderer.transform.localScale;
        if (cameraSize.x <= cameraSize.y) { // Landscape (or equal)
            scale *= cameraSize.x / spriteSize.x;
        } else { // Portrait
            scale *= cameraSize.y / spriteSize.y;
        }
        
        //bgSpriteRenderer.transform.position = Vector2.zero; // Optional
        bgSpriteRenderer.transform.localScale = scale;
        return bgSpriteRenderer;
    }
    
    public Transform Target {
        set { target = value; }
    }
}