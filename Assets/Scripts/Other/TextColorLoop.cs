using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextColorLoop : MonoBehaviour {
    public Vector3[] colors;
    public int startIndex;
    private Text colorRef;
    private float nextIn = 0f;
    private Vector3 currentColor, nextColor;
    private int k = 0;
    
    private void Start() {
        colorRef = GetComponent<Text>();
        currentColor = colors[startIndex];
        k = startIndex;
        if (++k >= colors.Length) k = 0;
        nextColor = colors[k];
    }

    private float t;
    private void Update() {
        if (Time.time > nextIn) {
            StartCoroutine(InterpolateColors(currentColor, nextColor));
            if (++k >= colors.Length) k = 0;
            currentColor = nextColor;
            nextColor = colors[k];
            nextIn = Time.time + 1f;
        }
    }

    private IEnumerator InterpolateColors(Vector3 color1, Vector3 color2) {
        WaitForSeconds tick = new WaitForSeconds(0.1f);
        Vector3 inter;
        for (int i = 0; i < 10; i++) {
            inter = Vector3.Lerp(color1, color2, i*0.1f);
            colorRef.color = new Color(inter.x, inter.y, inter.z);
            yield return tick;
        }
    }
}
