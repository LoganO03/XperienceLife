using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float deadSpacePercentage = 0.45f;
    [SerializeField] private float easing = 0.01f;

    private float lBound;
    private float rBound;
    private float uBound;
    private float dBound;

    void Start()
    {
        lBound = deadSpacePercentage * Camera.main.pixelWidth;
        rBound = Camera.main.pixelWidth - lBound;
        dBound = deadSpacePercentage * Camera.main.pixelHeight;
        uBound = Camera.main.pixelHeight - dBound;
    }

    private void FixedUpdate()
    {
        if (player) 
        {
            Vector3 playerLoc = Camera.main.WorldToScreenPoint(player.transform.position);

            Vector3 pos = transform.position;

            if (playerLoc.x < lBound) 
            {
                pos.x -= lBound - playerLoc.x;
            }
            else if (playerLoc.x > rBound)
            {
                pos.x += playerLoc.x - rBound;
            }

            if (playerLoc.y < dBound)
            {
                pos.y -= dBound - playerLoc.y;
            }
            else if (playerLoc.y > uBound)
            {
                pos.y += playerLoc.y - uBound;
            }

            pos = Vector3.Lerp(transform.position, pos, easing);

            transform.position = pos;
        }
    }

}
