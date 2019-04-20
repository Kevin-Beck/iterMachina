using UnityEngine;

public class CameraMover : MonoBehaviour
{
    Transform t;
    [SerializeField] float speed = 3f;
    Vector3 startPos;

    private void Start()
    {
        t = transform;
        startPos = transform.position;
    }
    private void Update()
    {
        float xSpeed = speed;
        float zSpeed = speed;

        Vector3 curVector = t.position - startPos;
        if (Input.GetKey("d"))
            t.Translate(Vector3.right * speed * Time.deltaTime);            
        if (Input.GetKey("a"))
            t.Translate(Vector3.left * speed * Time.deltaTime); 
        if (Input.GetKey("w"))
            t.Translate(Vector3.forward * speed * Time.deltaTime);
        if (Input.GetKey("s"))
            t.Translate(Vector3.back * speed * Time.deltaTime);
    }
}
