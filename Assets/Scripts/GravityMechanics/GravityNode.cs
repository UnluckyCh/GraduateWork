using UnityEngine;

public class GravityNode : MonoBehaviour
{
    public GravityNode Up;
    public GravityNode Down;
    public GravityNode Left;
    public GravityNode Right;

    public GravityNode GetNextNode(GravityDirection gravityDirection)
    {
        return gravityDirection switch
        {
            GravityDirection.Up => Up,
            GravityDirection.Down => Down,
            GravityDirection.Left => Left,
            GravityDirection.Right => Right,
            _ => null,
        };
    }
}
