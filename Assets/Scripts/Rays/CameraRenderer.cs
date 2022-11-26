using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    public RenderTexture renderTexture;
    public bool drawn = false;
    void Start()
    {
        
    }
    /*private void OnGUI()
    {    
        if (Event.current.type.Equals(EventType.Repaint))
        {
            //Graphics.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTexture);
        }
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
