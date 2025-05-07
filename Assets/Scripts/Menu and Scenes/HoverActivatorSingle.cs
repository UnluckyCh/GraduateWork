using UnityEngine;
using UnityEngine.EventSystems;

public class HoverActivatorSingle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool Block = false;

    [SerializeField] private GameObject _targetObject;

    private void Start()
    {
        _targetObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Block) return;

        if (_targetObject)
        {
            _targetObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Block) return;

        if (_targetObject)
        {
            _targetObject.SetActive(false);
        }
    }
}
