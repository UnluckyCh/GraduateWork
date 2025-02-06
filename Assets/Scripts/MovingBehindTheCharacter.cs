using UnityEngine;

public class MovingBehindTheCharacter : MonoBehaviour
{
    public Transform character;
    public float offsetX;
    public float offsetY;

    void Update()
    {
        if (character != null)
        {
            Vector3 characterPosition = character.position;
            transform.localPosition = new Vector3(characterPosition.x + offsetX, characterPosition.y + offsetY, transform.localPosition.z);
        }
    }
}
