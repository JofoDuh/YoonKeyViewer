using UnityEngine;
using UnityEngine.UI;

namespace LineKeyViewer;

public class AsyncImage : MonoBehaviour {
    public Image image;
    public Sprite sprite;
    public sbyte enable = -1;

    private void Awake() {
        image = this.GetOrAddComponent<Image>();
        image.type = Image.Type.Sliced;
    }

    private void Update() {
        if(enable != -1) {
            image.enabled = enable == 1;
            enable = -1;
        }
        if((object) sprite != null) {
            image.sprite = sprite;
            sprite = null;
        }
    }
}