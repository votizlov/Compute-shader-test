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
    [SerializeField] private int numOfAgents = 3;
    [SerializeField] private float moveSpeed = 0.01f;
    [SerializeField] private int sensorSize = 10;
    [SerializeField] private float steeringSpeed = 0.01f;
    [SerializeField] private bool isCircleInit = true;
    [SerializeField] private int height = 108;
    [SerializeField] private int width = 192;

    [SerializeField] private int startX = 960;
    [SerializeField] private int startY = 540;
    [SerializeField] private float[] colors;
    private float[] agentsData = new []{10f,10f,0f,20f,20f,0f,30f,30f,0f};

    private RenderTexture _renderTexture;
    private ComputeBuffer computeBuffer;
    void Start()
    {
        Application.targetFrameRate = 60;

        if(isCircleInit){CircleInit();}
        
        Agent a = new Agent();
        Debug.Log(System.Runtime.InteropServices.Marshal.SizeOf(a));
        
        _renderTexture = new RenderTexture(width,height,24);
        _renderTexture.filterMode = FilterMode.Point;
        _renderTexture.enableRandomWrite = true;
        _renderTexture.Create();
        _rawImage.texture = _renderTexture;
        
        _shader.SetTexture(0,"Result",_renderTexture);
        //_shader.SetTexture(1,"Result",_renderTexture);
        _shader.SetTexture(1,"Result",_renderTexture);
        _shader.SetBool("isTarget",false);
        _shader.SetInt("sensorSize",sensorSize);
        _shader.SetFloat("steeringSpeed",steeringSpeed);
        _shader.SetFloat("moveSpeed",moveSpeed);
        _shader.SetInt("width", _renderTexture.width);
        _shader.SetInt("height", _renderTexture.height);
        _shader.SetInt("numOfAgents",numOfAgents);
        computeBuffer = new ComputeBuffer(numOfAgents,12);
        computeBuffer.SetData(agentsData);
        ComputeBuffer colorsBuffer = new ComputeBuffer(colors.Length,16);
        colorsBuffer.SetData(colors);
        _shader.SetBuffer(0,"colors",colorsBuffer);
        _shader.SetBuffer(0,"agents",computeBuffer);
    }

    void Update(){
        if(Input.GetMouseButtonDown(0)){
            var t = Input.mousePosition;
            _shader.SetInt("tgtX",(int)t.x); 
            _shader.SetInt("tgtY",(int)t.y);
            _shader.SetBool("isTarget",true);
        }
        if(Input.GetMouseButtonUp(0)){
            _shader.SetBool("isTarget",false);
        }

        _shader.SetFloat("deltaTime",Time.fixedDeltaTime);        
        _shader.Dispatch(0,_renderTexture.width/8,_renderTexture.height/8,1);
        //_shader.Dispatch(1,_renderTexture.width/8,_renderTexture.height/8,1);
        _shader.Dispatch(1,_renderTexture.width/8,_renderTexture.height/8,1);
    }
/*
    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
        {
            //UpdateShader();
        }
    }*/

    void CircleInit(){
        Debug.Log(numOfAgents);
        agentsData = new float[numOfAgents*3];
        float t = 0;
        float step = 360/numOfAgents + 0.01f;
        for(int i = 0;i<agentsData.Length;i+=3){
            agentsData[i]=startX;
            agentsData[i+1]=startY;
            agentsData[i+2]=t;
            t+=step;
            if(t>6.284){t=0;}
        }
    }

    struct Agent
    {
        private float angle;
        private float posx;
        private float posy;
    }
    
}
