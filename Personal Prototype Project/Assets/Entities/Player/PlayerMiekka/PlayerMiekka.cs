using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMiekka : MonoBehaviour
{
    public Vector3 startRotation;
    public Vector2 localPosition;
    public float threshold;

    public float rotation;
    public float speed = 10f;
    public float swingSpeed = 360f;

    // Start is called before the first frame update
    void OnGUI()
    {
        GUI.Label(new Rect(10, 60, 150, 30), "Rotation = " + rotation);
    }
    void Start()
    {
        threshold = GetComponentInParent<PlayerController>().threshold;
        localPosition = GetComponentInParent<PlayerController>().localPosition;
        startRotation = GetComponentInParent<PlayerController>().startRotation;
        transform.localPosition = localPosition;
        transform.eulerAngles = startRotation;

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(speed * Time.deltaTime * Vector2.right);
        transform.Rotate(swingSpeed * Time.deltaTime * Vector3.back);

        rotation = transform.rotation.z;


        if (rotation < threshold)
        {
            Destroy(gameObject);
            //Debug.Log("Threshold works");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyManager>().enemyHealth -= 100;
        }
    }
}