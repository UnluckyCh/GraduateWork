using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    public SimplePlayerController playerController;
    public GameObject complitedGameObject;
    public GameObject completedObject;

    public Vector2 targetOffset;
    public float moveSpeed = 1.5f;
    public float shrinkDuration = 1f;

    private bool triggered = false;
    private Vector2 targetPosition;
    private float shrinkProgress = 0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !triggered && playerController.Alive)
        {
            triggered = true;
            if (playerController)
            {
                playerController.BlockDamage();
                playerController.StopPlayer();
                playerController.LockPlayerActions();
                playerController.PlayJumpAnimation();
                playerController.GetComponent<Rigidbody2D>().simulated = false;
                if (completedObject)
                {
                    completedObject.SetActive(true);
                }
            }

            targetPosition = (Vector2)transform.position + targetOffset;
        }
    }

    private void Update()
    {
        if (triggered)
        {
            Vector2 direction = (targetPosition - (Vector2)playerController.transform.position).normalized;
            playerController.transform.position = Vector2.MoveTowards(playerController.transform.position, targetPosition, moveSpeed * Time.deltaTime);

            shrinkProgress += Time.deltaTime / shrinkDuration;
            shrinkProgress = Mathf.Clamp01(shrinkProgress);

            float newSize = Mathf.Lerp(0.35f, 0f, shrinkProgress * shrinkProgress);
            playerController.transform.localScale = new Vector3(newSize, newSize, 1f);

            // Определяем направление по оси X на основе масштаба
            float scaleSign = Mathf.Sign(playerController.transform.localScale.x);

            if (direction.x * scaleSign < 0) // Если направление движения и направление масштаба по X противоположны
            {
                // Персонаж повернут в противоположную сторону
                playerController.transform.localScale = new Vector3(-playerController.transform.localScale.x, playerController.transform.localScale.y, playerController.transform.localScale.z);
            }

            if (shrinkProgress >= 1f)
            {
                CompletedGame();
            }
        }
    }

    private void CompletedGame()
    {
        complitedGameObject.GetComponent<CompletedScript>().CompletedGame();
    }
}
