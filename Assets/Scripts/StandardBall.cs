using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBall : BallBase
{
    [SerializeField]
    private Material highlightMaterial;
    private Material defaultMaterial;
    private Color defaultColor;

    private void Start()
    {
        this.defaultMaterial = this.GetComponent<Renderer>().material;
        this.defaultColor = this.GetComponent<Renderer>().material.color;
    }

    public void OnTriggerEnter(Collider trigger)
    {
        if (trigger.CompareTag("Hole"))//Add a point and destroy the ball if it falls in a hole
        {
            this.AddPoint();
            Destroy(this);
        }

        if (trigger.CompareTag("OutOfBounds"))//If the ball goes out of bounds, reset its position
        {
            this.transform.position = new Vector3(0, 0.2188f, 0);
            this.GetComponent<Rigidbody>().velocity = Vector3.zero;//Remove position changes (displacement)
            this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;//Remove angular movement (rotation)
        }
    }

    public void Highlight(bool highlighted)
    {
        if (highlighted)
        {            
            this.GetComponent<Renderer>().material = this.highlightMaterial;
            this.GetComponent<Renderer>().material.SetColor("baseColor", this.defaultColor*.5f);//Set the base color as the normal color of the ball
            //This "SetColor()" uses a reference to the BaseColor property of the shader called BaseColor, this was set modifying the Reference to a color in Unity Editor's shader editor
        }
        else
        {
            this.GetComponent<Renderer>().material = this.defaultMaterial;
        }
    }

}
