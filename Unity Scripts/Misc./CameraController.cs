using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform player;
    [SerializeField] private float aheadDistance;
    [SerializeField] private float speed;

    private float lookAhead;
    
    void Update()
    {
        transform.position = new Vector3(player.position.x + lookAhead, player.position.y + 2, transform.position.z);
        lookAhead = Mathf.Lerp(lookAhead, (aheadDistance * player.localScale.x), Time.deltaTime * speed);
    }
}
