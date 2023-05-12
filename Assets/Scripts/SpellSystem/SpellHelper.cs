using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHelper : MonoBehaviour
{

    public Vector3 direction;
    public float speed;
    public Sprite vfx;
    //private SpriteRenderer renderer;
    public float lifeTime;
    private float currentLifeTime;


    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.AddComponent<SpriteRenderer>();
        //renderer = this.gameObject.GetComponent<SpriteRenderer>();
        //GetComponent<Renderer>().sprite = vfx;
        //GetComponent<Renderer>().sortingOrder = 1;
        currentLifeTime = lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position += direction * speed * Time.deltaTime;


        if (currentLifeTime <= 0)
        {
            Destroy(this.gameObject);
        }
        currentLifeTime -= Time.deltaTime;
    }
}
