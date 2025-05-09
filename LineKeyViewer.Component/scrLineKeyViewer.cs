using UnityEngine;

namespace LineKeyViewer.Component;

public class scrLineKeyViewer : MonoBehaviour {
    public RectTransform sizeTransform;
    public RectTransform locationTransform;
    public AsyncImage mainImage;
    public AsyncImage leftHand;
    public AsyncImage rightHand;
    public AsyncImage head;
    public Key[] keys;
    public bool headOn;
    public bool winkOn;
    public bool gameResult;
}