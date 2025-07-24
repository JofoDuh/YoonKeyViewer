using UnityEngine;

namespace YoonKeyViewer.Component
{
    public class scrYoonKeyViewer : MonoBehaviour
    {
        public RectTransform sizeTransform;
        public RectTransform locationTransform;
        public AsyncImage Table;
        public AsyncImage leftHand;
        public AsyncImage rightHand;
        public AsyncImage leftLeg;
        public AsyncImage rightLeg;
        public AsyncImage Yoon;
        public AsyncImage YoonSmash;
        public AsyncImage YoonClear;
        public AsyncImage FeetKeyboard;
        public Key[] keys;
        public Key[] fKeys;
        public bool isSmashing;
        public bool winkOn;
        public bool gameResult;
        private bool _isNervous;
        public bool isNervous 
        { 
            get 
            {
                return _isNervous;
            }
            set
            {
                _isNervous = value;
                //Yoon.image.sprite = value ? YoonBundleManager.Instance.YoonNervous : YoonBundleManager.Instance.YoonIdle;
            }
        }
    }
}