using UnityEngine;

public class MovingAndScalingBehindTheCharacter : MonoBehaviour
{
    public Transform character;
    public Vector3 offset;

    private Vector3 initialCharacterScale;
    private Vector3 initialTransformScale;

    void Start()
    {
        if (character != null)
        {
            initialCharacterScale = character.localScale;
            initialTransformScale = transform.localScale;
        }
    }

    void Update()
    {
        if (character != null)
        {
            // Calculate the current scale ratio relative to the initial scale of the character
            Vector3 currentCharacterScale = character.localScale;
            Vector3 scaleRatio = new Vector3(
                currentCharacterScale.x / initialCharacterScale.x,
                currentCharacterScale.y / initialCharacterScale.y,
                currentCharacterScale.z / initialCharacterScale.z
            );

            // Apply the scale ratio to the offset
            Vector3 scaledOffset = new Vector3(
                offset.x * scaleRatio.x,
                offset.y * scaleRatio.y,
                offset.z * scaleRatio.z
            );

            // Update the position and scale relative to the character
            Vector3 characterPosition = character.position;
            transform.localPosition = new Vector3(
                characterPosition.x + scaledOffset.x,
                characterPosition.y + scaledOffset.y,
                transform.localPosition.z
            );

            // Apply the scale ratio to the initial transform scale
            Vector3 newTransformScale = new Vector3(
                initialTransformScale.x * scaleRatio.x,
                initialTransformScale.y * scaleRatio.y,
                initialTransformScale.z * scaleRatio.z
            );

            transform.localScale = newTransformScale;
        }
    }
}
