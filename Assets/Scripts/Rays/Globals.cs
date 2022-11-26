using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Ray
{
    Vector3 pos;
    Vector3 angle;
    Vector3 colour;
};

/*public struct Sphere
{
    public Vector3 pos;
    public float radius;
    public Vector3 colour;
}; //7 floats 
*/
public struct Sphere
{
    public Vector3 pos;
    public float radius;

    public Vector3 ambient;
    public Vector3 diffuse;
    public Vector3 specular;
    public float shininess;

    public Vector3 reflective;
    public Vector3 transmissive;
    public float refraction;
}; //21 floats

public struct Light
{
    public int type; //{ambient, point, directional, spotlight}
    public Vector3 pos;
    public float intensity; //brightness
    public Vector3 colour;
    public Vector3 dir; //for directional/spotlight
    public float cutoff; //for spotlight cone size
};

public class Globals : MonoBehaviour
{
    public const uint RaySize = sizeof(float)*3 *3;
    public const int SphereSize = sizeof(float) * 21; //7
    public const int LightSize = sizeof(float)*11 + sizeof(int);
    public const int LightTypes = 4; //realistically 4
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        //Debug.Log("Ray: " + RaySize);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
