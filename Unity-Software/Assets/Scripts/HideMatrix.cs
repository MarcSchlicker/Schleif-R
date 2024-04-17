using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HideMatrix : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public UnityEngine.UI.Toggle checkbox;

    void Start()
    {
        Hiding();
    }

    // Update is called once per frame

   public void Hiding()
   {
   if (MeshRenderer != null && checkbox != null)
        {
            Color objectColor = MeshRenderer.sharedMaterial.color;
            if (checkbox.isOn)
            {
                objectColor.a = 1f;
            }
            else
            {
                objectColor.a = 0f;
            }
            MeshRenderer.sharedMaterial.color = objectColor;
        }

         
    }
}
