using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputSchleifwerte : MonoBehaviour
{
    public bool isConnected = false;
    public bool isOn = false;
    public float pressure = 0.0f;
    public int drehzahl = 0;

    //public AudioSource schleiferSound;

    [HideInInspector]
    public UnityEvent onStart = new UnityEvent();
    [HideInInspector]
    public UnityEvent onStop = new UnityEvent();



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnConnectionEvent(bool isConnected)
    {
        this.isConnected = isConnected;
    }

    void OnMessageArrived(string message)
    {
        //Debug.Log("New message: " + message);
        if (message == "AN")
        {
            if (!isOn) onStart.Invoke();
            isOn = true;
        }
        else if (message == "AUS")
        {
            if (isOn) onStop.Invoke();
            isOn = false;
        }
        else if (message.StartsWith("Druck: "))
        {
            string part2 = message.Substring(7);
            int p = part2.IndexOf(' ');
            pressure = float.Parse(part2.Substring(0, p));
        }
        else if (message.StartsWith("Drehzahl: "))
        {
            drehzahl = int.Parse(message.Substring(10));
        }
        else
        {
            Debug.Log("Unknown message: " + message);
        }
        //Debug.Log("STATUS: connected=" + isConnected + " on=" + isOn + " pressure=" + pressure + " drehzahl=" + drehzahl);
    }

    /*private void onStart()
    {
        schleiferSound.Play();
    }

    private void onStop()
    {
        schleiferSound.Stop();
    }*/
}
