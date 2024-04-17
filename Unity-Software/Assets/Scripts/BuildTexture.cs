using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class BuildTexture : MonoBehaviour
{
    // Textur-Abmessungen
    private int width = 1024;
    private int height = 1024;
    public Texture2D renderedOutput;

    private float[,] schleifIntensivitaet;

    public GameObject schleifer;
    public GameObject planeObject;
    private InputSchleifwerte schleifwerte;

    // Schleifscheibe + Abmessungen
    //TODO Schleifscheibe verschieben für den Start; Tisch mit Schleifscheibengröße 
    public GameObject schleifScheibe;
    private float schleifscheibeRadiusX;
    private float schleifscheibeRadiusY;

    // Werkstück
    public GameObject werkstueck;

    // Start is called before the first frame update
    void Start()
    {
        schleifwerte = schleifer.GetComponent<InputSchleifwerte>();
        ResetWerkstueck();
    }

    public void ResetWerkstueck()
    {
        var bounds = werkstueck.GetComponent<MeshRenderer>().bounds;

        // scale plane to object
        var werkstueckSize = bounds.max - bounds.min;
        var localSize = planeObject.transform.worldToLocalMatrix.MultiplyVector(werkstueckSize);
        var scale = planeObject.transform.localScale;
        float xScale = scale.x / 10.0f;  //10 ist der default-Scale der Fläche (von -5 zu +5)
        float zScale = scale.z / 10.0f;
        planeObject.transform.localScale = new Vector3(
            //scale.x * localSize.x / 10.0f, 
            xScale * localSize.x,
            scale.y,
            //scale.z * localSize.z / 10.0f
            zScale*localSize.z
        );

        // move plane to object
        var targetPos = bounds.center;
        targetPos.y = bounds.max.y + 0.001f;
        planeObject.transform.position = targetPos;

        // Scale changes texture coordinates -> reset everything
        ResetTiefe();
        ResetSchleifscheibe();
    }

    public void ResetSchleifscheibe()
    {
        SetSchleifscheibenRadius(GetSchleifscheibenRadius());
    }

    private float GetSchleifscheibenRadius()
    {
        // Radius der Scheibe bestimmen, funktioniert nicht, wenn die Scheibe komplett verdreht ist
        var bounds = schleifScheibe.GetComponent<MeshRenderer>().bounds;
        var diagonale = bounds.max - bounds.min;
        return Math.Max(diagonale.x, diagonale.z) / 2.0f;
    }

    /**
     * radius in world coordinates.
     */
    private void SetSchleifscheibenRadius(float radius)
    {
        // 1x1-Kreis nach World projezieren
        Vector3 dx = new Vector3(1.0f, 0.0f, 0.0f);
        Vector3 dy = new Vector3(0.0f, 0.0f, 1.0f);
        dx = planeObject.transform.localToWorldMatrix.MultiplyVector(dx);
        dy = planeObject.transform.localToWorldMatrix.MultiplyVector(dy);
        float rxInLocalPlaneCoords = radius / dx.magnitude;
        float ryInLocalPlaneCoords = radius / dy.magnitude;
        schleifscheibeRadiusX = rxInLocalPlaneCoords * width / 10.0f;
        schleifscheibeRadiusY = ryInLocalPlaneCoords * height / 10.0f;
    }

    public void ResetTiefe()
    {
        schleifIntensivitaet = new float[width, height];
        intensitaetToTexture(0, width, 0, height);
    }

    // Update is called once per frame
    
    void Update()
    {
        if (!schleifwerte.isConnected || !schleifwerte.isOn) 
            return;

        // Plane: -5 bis +5
        Vector3 pos = planeObject.transform.worldToLocalMatrix.MultiplyPoint(schleifer.transform.position);
        // y-Koordinate checken ob aufgesetzt, falls Druck nicht funktioniert 

        // Plane coordinates => Texture Coordinates
        float x = ((5.0f - pos.x) / 10.0f) * width;
        float y = ((5.0f - pos.z) / 10.0f) * height;
        
        float tiefe = 0.2f + (0*schleifwerte.pressure)* (0*schleifwerte.drehzahl); // TODO Wer für druck, drehzahl finden 
        if (schleifwerte.pressure < 0f)
        {
            tiefe = 0;
        }
        if (tiefe > 0)
        {
            schleifen(x, y, tiefe * Time.deltaTime);
        }
    }

    private void schleifen(float x, float y, float tiefe)
    {
        // Radius des Schleifers
        float rx = schleifscheibeRadiusX;
        float ry = schleifscheibeRadiusY;

        // Koordinaten des Kreises den wir zeichnen
        int xStart = (int)Math.Floor(x - rx);
        int xEnd = (int)Math.Ceiling(x + rx);
        int yStart = (int)Math.Floor(y - ry);
        int yEnd = (int)Math.Ceiling(y + ry);
        if (xStart < 0) xStart = 0;
        if (xEnd > width) xEnd = width;
        if (yStart < 0) yStart = 0;
        if (yEnd > width) yEnd = height;
        if (xStart > xEnd || yStart > yEnd)
            return;

        float rx2 = rx * rx;
        float ry2 = ry * ry;
        for (int px = xStart; px < xEnd; px++)
        {
            for (int py = yStart; py < yEnd; py++)
            {
                float d2 = (px - x) * (px - x) / rx2 + (py - y) * (py - y) / ry2;
                if (d2 <= 1) // wenn im Kreis
                    schleifIntensivitaet[px, py] += tiefe;
            }
        }

        // var watch2 = Stopwatch.StartNew();
        intensitaetToTexture(xStart, xEnd, yStart, yEnd);
        // watch2.Stop();
        // Debug.Log($"Timing (Texture): {watch2.ElapsedMilliseconds}ms");
    }

    private void intensitaetToTexture(int xStart, int xEnd, int yStart, int yEnd)
    {
        float value1 = 0.0f;
        Color color1 = Color.blue;
        float value2 = 1.0f;
        Color color2 = Color.green;
        float value3 = 2.0f;
        Color color3 = Color.red;

        var colors = new Color[xEnd - xStart];

        for (int y = yStart; y < yEnd; y++)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                float value = schleifIntensivitaet[x, y];
                if (value <= value1)
                    colors[x - xStart] = color1;
                else if (value < value2)
                    colors[x - xStart] = Color.Lerp(color1, color2, (value - value1) / (value2 - value1));
                else if (value < value3)
                    colors[x - xStart] = Color.Lerp(color2, color3, (value - value2) / (value3 - value2));
                else
                    colors[x - xStart] = color3;
            }

            renderedOutput.SetPixels(xStart, y, xEnd - xStart, 1, colors);
        }

        renderedOutput.Apply();
    }
}