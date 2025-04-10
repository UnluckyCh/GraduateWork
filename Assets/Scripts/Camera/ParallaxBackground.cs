using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    // ������ �� ������. ���� �� ���������, ������������ �������� ������.
    [SerializeField] private Transform cameraTransform;

    // ����������� ���������� ��� ������� ����
    [SerializeField] private float parallaxCoefficient = 0.5f;

    // ���������� ������� ������ � ��������� ����������� �������
    private Vector3 previousCameraLocalPosition;

    // ������������ ������ (�������), ������������ �������� ��������� ������� ������
    private Transform parentTransform;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // ��������������, ��� ��� �������� �������� ��������� �������
        parentTransform = transform.parent;

        // ��������� ������� ������ � ��������� ������� ��������� �������� (�������)
        previousCameraLocalPosition = parentTransform.InverseTransformPoint(cameraTransform.position);
    }

    private void LateUpdate()
    {
        // �������� ������� ������� ������ � ��������� ����������� �������
        Vector3 currentCameraLocalPosition = parentTransform.InverseTransformPoint(cameraTransform.position);

        // ������� �������� ������ � ��������� ������� ���������
        Vector3 deltaMovement = currentCameraLocalPosition - previousCameraLocalPosition;

        // ������� ��� � ��������� ������� ��������� � ������ ������������ ����������
        transform.localPosition += new Vector3(deltaMovement.x * parallaxCoefficient, deltaMovement.y * parallaxCoefficient, 0);

        // ��������� ���������� ������� ��� ���������� �����
        previousCameraLocalPosition = currentCameraLocalPosition;
    }
}
