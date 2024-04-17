using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activateSound : MonoBehaviour
{

    [HideInInspector]
    private InputSchleifwerte schleifwerte;

    public AudioSource schleiferSound;

    // Start is called before the first frame update
    void Start()
    {
        schleifwerte = GetComponent<InputSchleifwerte>();
        schleifwerte.onStart.AddListener(onStart);
        schleifwerte.onStop.AddListener(onStop);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void onStart()
    {
        //Debug.Log("Start");
        schleiferSound.Play();
    }

    private void onStop()
    {
        //Debug.Log("Stop");
        schleiferSound.Stop();
    }
}
