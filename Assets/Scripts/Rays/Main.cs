using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public int number_of_spheres = 4;
    public int number_of_lights = 6;

    public ComputeShader computeShader;
    public RenderTexture renderTexture;

    public float speed = 5.0f;
    public float SecondsPerFrame = 5.0f;
    public int width = 16 * 40;
    public int height = 9 * 40;

    private int maxObjects = 100;

    private int mainIndex;
    private int updateIndex;
    private bool ready = false;

    public Vector3 backgroundColour = new(0.0f, 0.0f, 0.0f);
    public Vector3 eye = new(0,0,-1);
    public Vector3 eyeDir = new(0, 0, -1);
    public Vector3 eyeUp = new(0, 1, 0);
    public Vector3 eyeDir2 = new(0, 0, 0);
    public Vector2 eyeAngle = new(0, 0);
    private Vector2 mouseWas = new(0, 0);
    private bool mouseSet = false;
    public float FOV = 60;


    private float secondsCounter;
    private bool rendering = false;
    // Start is called before the first frame update
    void Start()
    {
        eyeDir = new(0, 0, 0);
        mainIndex = computeShader.FindKernel("Main");
        updateIndex = computeShader.FindKernel("Update");
        Setup();
        ready = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time - secondsCounter > SecondsPerFrame && !rendering && ready)
        {
            rendering = true;

            SetupRender();
            RenderImage();

            secondsCounter = Time.time;
            rendering = false;
        }
    }

    private float zVal = 0;
    void Update()
    {
        if (Input.GetKey("z")) FOV += 0.5f;
        if (Input.GetKey("x")) FOV -= 0.5f;
        if (Input.GetKey("r")) Setup();

        Vector2 mouseAt = new(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (!mouseSet) { mouseWas = mouseAt; mouseSet = true; } //previous mouse pos
        if (mouseWas != mouseAt)
        {
            float turnSensitivity = 5.0f;
            eyeAngle += mouseAt * Time.deltaTime * turnSensitivity;
            eyeAngle.x %= (2*Mathf.PI);
            if (eyeAngle.y > Mathf.PI / 2.0f) eyeAngle.y = Mathf.PI / 2.0f;
            if (eyeAngle.y < -Mathf.PI / 2.0f) eyeAngle.y = -Mathf.PI / 2.0f;
            eyeDir = new(Mathf.Sin(eyeAngle.x) * Mathf.Cos(eyeAngle.y),
                         Mathf.Sin(eyeAngle.y),
                         Mathf.Cos(eyeAngle.x) * Mathf.Cos(eyeAngle.y));
            //eyeDir *= Mathf.PI;
        }
        //Vector3 left = Vector3.Cross(eyeDir, Vector3.up).normalized;
        //Vector3 right = -left;


        //Vector3 move = new(0, 0, 0);
        if (Input.GetKey("w"))
        {
            //move += eyeDir * speed * Time.deltaTime;
            eye.z -= speed * Time.deltaTime;
        }
        if (Input.GetKey("a"))
        {
            //move += left * speed * Time.deltaTime;
            eye.x -= speed * Time.deltaTime;
        }
        if (Input.GetKey("s"))
        {
            //move -= eyeDir * speed * Time.deltaTime;
            eye.z += speed * Time.deltaTime;
        }
        if (Input.GetKey("d"))
        {
            //move -= left * speed * Time.deltaTime;
            eye.x += speed * Time.deltaTime;
        }
        if (Input.GetKey("q"))
        {
            eye.y -= speed * Time.deltaTime;
        }
        if (Input.GetKey("e"))
        {
            eye.y += speed * Time.deltaTime;
        }
        //eye += move;

        //backgroundColour[0] += Time.deltaTime*2;
        //if (backgroundColour[0] > 1) backgroundColour[0] = 0;
    }
    /*
     * // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !mousePressed && ready)
        {
            mousePressed = true;
            mousePos = getMousePos(); //Input.mousePosition;
        }

        if (Input.GetKeyDown("space"))
        {
            queenPOV = !queenPOV;
        }
        if (Input.GetKeyDown("q"))
        { //consider this a round 'win' 
            sceneManager.GetComponent<SceneChanger>().ChangeScene("GameSettings");
        }
    }*/

    //To be run once before running the shader.
    void Setup()
    {
        ComputeBuffer spheresBuffer = SetupSpheres(number_of_spheres);
        ComputeBuffer lightsBuffer = SetupLights(number_of_lights);
        secondsCounter = Time.time;

        renderTexture = new RenderTexture(width, height, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.filterMode = FilterMode.Point;
        renderTexture.Create();

        //Set child's material to rendertexture for rendering, updates automatically
        transform.GetComponentInChildren<MeshRenderer>().material.mainTexture = renderTexture;

        computeShader.SetBuffer(mainIndex, "spheres", spheresBuffer);
        computeShader.SetBuffer(mainIndex, "lights", lightsBuffer);

        computeShader.SetBuffer(updateIndex, "spheres", spheresBuffer);
        computeShader.SetBuffer(updateIndex, "lights", lightsBuffer);

        computeShader.SetTexture(mainIndex, "Result", renderTexture);
        computeShader.SetTexture(updateIndex, "Result", renderTexture);
        computeShader.SetInt("width", width);
        computeShader.SetInt("height", height);

        secondsCounter = 0;
    }

    ComputeBuffer SetupSpheres(int numSpheres)
    {
        Sphere[] spheres = new Sphere[numSpheres];
        for (int i = 0; i < numSpheres; i++)
        {
            Sphere s = new Sphere();
            s.pos = new(-4f+0.5f*i, -0.5f*i, -4-(3.0f*i));
            s.radius = 0.5f + i*2.0f;
            //s.colour = new(0.1f, 0.7f, 0.2f);
            s.ambient = new(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            s.diffuse = new(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            s.shininess = Random.Range(0.0f, 1.0f);
            s.specular = new(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            s.reflective = new(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            s.transmissive = new(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            s.refraction = Random.Range(0.0f, 2.0f);
            spheres[i] = s;
        }
        return GetBuffer(ref spheres, numSpheres, Globals.SphereSize);
    }
    //public float cutoff = 0.7f;
    ComputeBuffer SetupLights(int numLights)
    {
        //numLights = 1;
        Light[] lights = new Light[numLights];
        for (int i = 0; i < numLights; i++)
        {
            Light l = new Light();
            l.type = i % Globals.LightTypes;
            Debug.Log("created type: " + l.type +"(i = "+i);
            l.pos = new(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)); // l.pos = new(0.2f + 0.5f * i, -6+2*i, -2 - i);
            l.intensity = 2/(float)numLights;// 0.4f + Random.Range(0.0f, 6.0f);
            l.colour = new(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            //l.colour = new((0.5f + (0.99f * i)) % 1, (0.8f + (0.49f * i)) % 1, (0.2f + (0.2f * i)) % 1);
            l.dir = new(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            l.cutoff = Random.Range(0.5f, 1.0f); // 0.7f; // cutoff;
            lights[i] = l;
        }
        return GetBuffer(ref lights, numLights, Globals.LightSize);
    }

    ComputeBuffer GetBuffer<T>(ref T[] data, int dataLength, int dataSize)
    {
        ComputeBuffer buffer = new ComputeBuffer(dataLength, dataSize);
        buffer.SetData(data);
        return buffer;
    }


    //To be run before the shader, updating any values needed per render
    void SetupRender()
    {
        computeShader.SetVector("colour", backgroundColour);
        computeShader.SetVector("eye", eye);
        computeShader.SetVector("eyeDir", eyeDir);
        computeShader.SetVector("eyeAngle", eyeAngle);
        computeShader.SetVector("rand", eyeDir2);
        computeShader.SetFloat("FOV", FOV);
        computeShader.SetFloat("deltaTime", Time.deltaTime);
    }
    void RenderImage()
    {
        computeShader.Dispatch(mainIndex, width / 8, height / 8, 1);
        //computeShader.Dispatch(updateIndex, maxObjects / 8, 1, 1);
    }


}
