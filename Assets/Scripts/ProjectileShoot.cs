using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileShoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            GetComponent<AudioSource>().Play();
            Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        }
    }
}