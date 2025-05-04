using UnityEngine;

public class MultiObjectActivator : MonoBehaviour
{
    [SerializeField] private GameObject[] _objects;

    private readonly int _defaultIndex = 0;

    private void Awake()
    {
        ActivateOnly(_defaultIndex);
    }

    public void ActivateOnly(int index)
    {
        for (int i = 0; i < _objects.Length; i++)
        {
            if (_objects[i])
            {
                _objects[i].SetActive(i == index);
            }
        }
    }

    public void ActivateDefault()
    {
        ActivateOnly(_defaultIndex);
    }
}
