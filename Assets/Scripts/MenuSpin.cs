using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuSpin : MonoBehaviour
{
    public Sprite[] clubs;
    public float speed;
    public float minSpeed;
    public float maxSpeed;

    // Start is called before the first frame update
    void Start()
    {
        int number = Random.Range(0, clubs.Length);
        GetComponent<Image>().sprite = clubs[number];
        speed = Random.Range(minSpeed, maxSpeed);
    }

	private void Update()
	{
        transform.localRotation = Quaternion.Euler(0f, 0f, speed) * transform.localRotation;
	}

}
