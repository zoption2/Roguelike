using UnityEngine;

namespace SlingShot
{
    public class TouchZone : MonoBehaviour
    {
        public GameObject ballPrefab; 

        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
                    Instantiate(ballPrefab, touchPosition, Quaternion.identity);
                }
            }
        }
    }
}
