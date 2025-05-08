using UnityEngine;
using UnityEngine.UI;

namespace LineKeyViewer;

public class Key : MonoBehaviour {
    public Image image;
    public sbyte enable;

    private void Awake() {
        image = this.GetOrAddComponent<Image>();
        image.type = Image.Type.Sliced;
        image.enabled = false;
        enable = -1;
    }

    private void Update() {
        if(enable == -1) return;
        image.enabled = enable == 1;
        enable = -1;
    }
}