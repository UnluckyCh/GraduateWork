using UnityEngine;
using UnityEngine.EventSystems;

public class HoverActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool Block = false;

    [SerializeField] private GameObject _targetObject;

    public void OnPointerEnter(PointerEventData eventData)
    {
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
