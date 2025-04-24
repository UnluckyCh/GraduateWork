using UnityEngine;

public class BreakingBridge : MonoBehaviour
{
    [System.Serializable]
    private class PieceSettings
    {
        public Transform pieceTransform;
        public Vector2 offset;
        public float rotationSpeed;
        public float targetRotation;
        public bool useLocalOffset = true;
    }

    [SerializeField] private BoxCollider2D _bridgeCollider;
    [SerializeField] private PieceSettings[] _pieces;
    [SerializeField] private float _flyDuration = 1.5f;

    private bool _isBroken = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isBroken)
            return;

        if (other.CompareTag("Boulder") &&
            GravityController.Instance.CurrentGravity == GravityDirection.Up)
        {
            BreakBridge();
        }
    }

    private void BreakBridge()
    {
        _isBroken = true;
        if (_bridgeCollider != null)
            _bridgeCollider.enabled = false;

        foreach (var piece in _pieces)
        {
            StartCoroutine(FlyAndRotatePiece(piece));
        }
    }

    private System.Collections.IEnumerator FlyAndRotatePiece(PieceSettings piece)
    {
        Vector3 startPos = piece.pieceTransform.position;
        Quaternion startRot = piece.pieceTransform.rotation;

        Vector3 targetPos = piece.useLocalOffset
            ? startPos + (Vector3)(piece.pieceTransform.parent.rotation * piece.offset)
            : startPos + (Vector3)piece.offset;

        float elapsed = 0f;

        while (elapsed < _flyDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _flyDuration;

            piece.pieceTransform.position = Vector3.Lerp(startPos, targetPos, t);
            piece.pieceTransform.rotation = Quaternion.Lerp(
                startRot,
                Quaternion.Euler(0f, 0f, piece.targetRotation),
                t
            );

            yield return null;
        }

        piece.pieceTransform.position = targetPos;
        piece.pieceTransform.rotation = Quaternion.Euler(0f, 0f, piece.targetRotation);
    }
}
