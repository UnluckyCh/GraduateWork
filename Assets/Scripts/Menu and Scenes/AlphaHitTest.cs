using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaHitTest : MonoBehaviour, ICanvasRaycastFilter
{
    [SerializeField, Range(0, 1)] private float _alphaThreshold = 0.1f;

    private Image _image;
    private Sprite _sprite;
    private Texture2D _texture;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _sprite = _image.sprite;
        _texture = _sprite.texture;
    }

    public bool IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _image.rectTransform, screenPos, eventCamera, out Vector2 local);

        Rect rect = _image.rectTransform.rect;
        float x = (local.x - rect.x) / rect.width;
        float y = (local.y - rect.y) / rect.height;

        if (x < 0 || x > 1 || y < 0 || y > 1) return false;

        int texX = Mathf.RoundToInt(x * _sprite.rect.width + _sprite.rect.x);
        int texY = Mathf.RoundToInt(y * _sprite.rect.height + _sprite.rect.y);

        try
        {
            Color pixel = _texture.GetPixel(texX, texY);
            return pixel.a >= _alphaThreshold;
        }
        catch
        {
            return false;
        }
    }
}
