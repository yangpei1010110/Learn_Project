using UnityEngine;

public class TriggerTest : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"时间:{Time.time} {gameObject.name}->碰撞了->{other.name}");
    }
}
