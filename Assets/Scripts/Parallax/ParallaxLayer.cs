using UnityEngine;

[System.Serializable]
public class ParallaxLayer
{
    [SerializeField] private Transform background;
    [SerializeField] private float parallaxMultiplier;
    [SerializeField] private float ImageWidthOffset = 10;
    private float imageFullWidth;
    private float imageHalfWidth;

    public void CalculateImageWidth()
    {
        imageFullWidth = background.GetComponent<SpriteRenderer>().bounds.size.x;
        imageHalfWidth = imageFullWidth / 2;
    }

    public void Move(float distanceToMove)
    {
        background.position += new Vector3(distanceToMove * parallaxMultiplier, 0);
    }


    public void LoopBackground(float cameraLeftEdge, float cameraRightEdge)
    {
        float imageRightEdge = background.position.x + imageHalfWidth - ImageWidthOffset;
        float imageLeftEdge = background.position.x - imageHalfWidth + ImageWidthOffset;

        if (imageRightEdge < cameraLeftEdge)
        {
            background.position += Vector3.right * imageFullWidth;
        }
        else if (imageLeftEdge > cameraRightEdge)
        {
            background.position += Vector3.right * -imageFullWidth;
        }

    }
}
