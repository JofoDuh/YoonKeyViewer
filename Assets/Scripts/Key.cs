using UnityEngine;
using UnityEngine.UI;

namespace YoonKeyViewer.Component
{
    public class Key : MonoBehaviour
    {
        public Image image;
        public sbyte enable = -1;

        private void Update()
        {
            if (enable == -1) return;
            image.enabled = enable == 1;
            enable = -1;
        }
    }
}