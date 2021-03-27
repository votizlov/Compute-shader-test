using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AntsController : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _shader;

    [SerializeField] private RawImage _rawImage;
    [SerializeField] private int numOfAgents = 1;
    private float[] agentsData = new []{10f,10f,0f};

    private RenderTexture _renderTexture;
    private ComputeBuffer computeBuffer;
    void Start()
    {
        Application.targetFrameRate = 60;
        Agent a = new Agent();
        Debug.Log(System.Runtime.InteropServices.Marshal.SizeOf(a));
        _renderTexture = new RenderTexture(192,108,24);
        _renderTexture.enableRandomWrite = true;
        _renderTexture.Create();
        _rawImage.texture = _renderTexture;
        
        _shader.SetTexture(0,"Result",_renderTexture);
        _shader.SetInt("width", _renderTexture.width);
        _shader.SetInt("height", _renderTexture.height);
        _shader.SetInt("numOfAgents",numOfAgents);
        computeBuffer = new ComputeBuffer(numOfAgents,12);
        computeBuffer.SetData(agentsData);
        _shader.SetBuffer(0,"agents",computeBuffer);
    }

    void Update()
    {
        _shader.SetFloat("deltaTime",Time.deltaTime);
        
        _shader.Dispatch(0,_renderTexture.width/8,_renderTexture.height/8,1);
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
        {
            //UpdateShader();
        }
    }

    struct Agent
    {
        private float angle;
        private float posx;
        private float posy;
    }
    
}
