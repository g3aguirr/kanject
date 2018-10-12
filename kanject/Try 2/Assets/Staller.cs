using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staller : MonoBehaviour
{
    public KeyCode key;
    bool active = false;
    GameObject note;
    SpriteRenderer sr;
    Color old;
    Note notescript;
    Rigidbody2D rb;
    public bool destroyed; //Demo checks if sign is destroyed
    private void Awake()
    {
        destroyed = false;
        sr = GetComponent<SpriteRenderer>();
    }
    // Use this for initialization
    void Start()
    {
        old = sr.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePosition;

            destroyed = true;
            //Destroy(note, 2f);
            StartCoroutine(Pressed());
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        active = true;
        if (col.gameObject.tag == "Note")
        {
            note = col.gameObject;
            rb = note.GetComponent<Rigidbody2D>();
            notescript = note.GetComponent<Note>();
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        active = false;
    }

    IEnumerator Pressed()
    {

        sr.color = new Color(0, 0, 0);
        yield return new WaitForSeconds(0.2f);
        sr.color = old;

    }

}
