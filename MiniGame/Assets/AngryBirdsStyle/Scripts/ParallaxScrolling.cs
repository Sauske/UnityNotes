using UnityEngine;
using System.Collections;

public class ParallaxScrolling : MonoBehaviour
{
    Camera mCamera;
    // Use this for initialization
    void Start()
    {
        mCamera = Camera.main;
        previousCameraTransform = mCamera.transform.position;
    }

    /// <summary>
    /// similar tactics just like the "CameraMove" script
    /// </summary>
    void Update()
    {
        Vector3 delta = mCamera.transform.position - previousCameraTransform;
        delta.y = 0; delta.z = 0;
        transform.position += delta / ParallaxFactor;


        previousCameraTransform = mCamera.transform.position;
    }

    public float ParallaxFactor;

    Vector3 previousCameraTransform;

    ///background graphics found here:
    ///http://opengameart.org/content/hd-multi-layer-parallex-background-samples-of-glitch-game-assets
}
