using UnityEngine;
using UnityEngine.EventSystems;

public class HoverActivator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool Block = false;

    [SerializeField] private GameObject _targetObject;
    [SerializeField] private MultiObjectActivator _multiActivator;
    [SerializeField] private int _objectIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_targetObject)
        {
            _targetObject.SetActive(true);
        }
        if (_multiActivator)
        {
            _multiActivator.ActivateOnly(_objectIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Block) return;

        if (_targetObject)
        {
            _targetObject.SetActive(false);
        }
        if (_multiActivator)
        {
            _multiActivator.ActivateDefault();
        }
    }
}
