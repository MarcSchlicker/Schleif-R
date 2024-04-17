using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TischHohenAnpassung : MonoBehaviour
{
    public GameObject Schleiferv2;
    public GameObject Tisch;
    private float xSchleifer;
    private float ySchleifer;
    private float zSchleifer;
    private float xTisch;
    private float zTisch;

    // Start is called before the first frame update
    void Start()
    {
        if (Schleiferv2 != null) // Sicherstellen, dass das Schleiferv2-Objekt zugewiesen wurde
        {
            Transform TischTransform = Tisch.transform;
            xTisch = TischTransform.position.x;
            zTisch = TischTransform.position.z;
        }
        else
        {
            Debug.LogError("Das GameObject 'Schleiferv2' wurde nicht zugewiesen.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        if (Input.GetButton(".x-Knopf") || Input.GetKey(KeyCode.Backspace))
        {
            Transform targetTransform = Schleiferv2.transform;
            ySchleifer = targetTransform.position.y; // Y-Wert speichern
            xSchleifer = targetTransform.position.x - 0.63f;
            zSchleifer = targetTransform.position.z + 0.38f;
            Tisch.transform.position = new Vector3(xSchleifer, ySchleifer, zSchleifer);
            //Tisch.transform.position = new Vector3(xTisch, ySchleifer, zTisch); // transform.position statt gameObject.transform.position
        }
    }
}