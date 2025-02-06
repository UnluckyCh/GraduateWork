using UnityEngine;

public class KeyFollowScript : MonoBehaviour
{
    public Transform target; // Целевой объект, за которым будет следовать ключ
    public Vector2 followOffset; // Двумерный оффсет для точки преследования
    public float followSpeed = 5f; // Скорость следования

    private bool isFollowing = false;

    // Метод для начала следования
    public void StartFollowing()
    {
        isFollowing = true;
    }

    // Метод для остановки следования
    public void StopFollowing()
    {
        isFollowing = false;
    }

    void Update()
    {
        if (isFollowing && target != null)
        {
            // Получаем позицию цели в двумерном пространстве
            Vector2 targetPosition = target.position;

            // Добавляем двумерный оффсет к позиции цели
            targetPosition += followOffset;

            // Вычисляем новую позицию для ключа с использованием интерполяции
            Vector2 newPosition = Vector2.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
        }
    }
}
