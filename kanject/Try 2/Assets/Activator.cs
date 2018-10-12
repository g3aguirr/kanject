using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activator : MonoBehaviour {
    public KeyCode key;
    bool active = false;
    GameObject note;
    SpriteRenderer sr;
    Color old;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    // Use this for initialization
    void Start () {
        old = sr.color;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(key))
            StartCoroutine(Pressed());
        if (Input.GetKeyDown(key) && active)
        {
            Destroy(note);
            StartCoroutine(Pressed());
        }
	}

     void OnTriggerEnter2D(Collider2D col)
    {
        active = true;
        if (col.gameObject.tag == "Note")
            note = col.gameObject;
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
