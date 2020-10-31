using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


public enum EffectType
{
    GRAB,
    VORTEX,
    HEIGHT,
    VORTEX2,
    SIN,
    INVERT
        
}

public class AnimatedTexture : MonoBehaviour
{
    public ComputeShader computeShader;

    public ComputeBuffer buffer;
    public ComputeBuffer argBuffer;
    public Texture texture;

    public Material material;

    int fillKernel;
    int computeKernel;
    int vortexKernel;
    int heightKernel;
    int vortex2Kernel;
    int sinKernel;
    int invertKernel;
    public Particle[] particles;

    Vector3 prevMousePos;
    Vector3 mousePos;

    [Range(0.1f, 20.0f)]
    public float radius = 0.5f;
    [Range(0.01f, 10f)]
    public float effectStrength;

    public List<EffectType> kernelList;

    [System.Serializable]
    public struct Particle
    {
        public Vector4 color;
        public Vector3 position;
        public Vector3 initialPosition;
        public Vector3 velocity;
        public Vector3 targetPosition;
        public float interpolationFactor;
        public bool interpolationGrow;
        public bool move;
        public Vector3 startMovePosition;
        public Vector4 initialColor;
    }

    private void Start()
    {
        InitializeDatas();
    }
    public void InitializeDatas()
    {
        buffer = new ComputeBuffer(texture.width * texture.height, Marshal.SizeOf(typeof(Particle)), ComputeBufferType.Append);
        buffer.SetCounterValue(0);

        fillKernel = computeShader.FindKernel("FillBuffer");
        computeKernel = computeShader.FindKernel("SimulateGrab");
        vortexKernel = computeShader.FindKernel("SimulateVortex");
        heightKernel = computeShader.FindKernel("SimulateHeight");
        vortex2Kernel = computeShader.FindKernel("SimulateVortex2");
        invertKernel = computeShader.FindKernel("SimulateInvert");
        sinKernel = computeShader.FindKernel("SimulateSin");
        computeShader.SetTexture(fillKernel, "Texture", texture);
        computeShader.SetBuffer(fillKernel, "_Buffer", buffer);
        computeShader.SetFloat("_DeltaTime", Time.deltaTime);
        

        argBuffer = new ComputeBuffer(4, Marshal.SizeOf(typeof(int)), ComputeBufferType.IndirectArguments);
        var args = new int[] { 0, 1, 0, 0 };
        argBuffer.SetData(args);

        // Fill Buffer
        computeShader.Dispatch(fillKernel, 128, 128, 1);
        computeShader.SetBuffer(computeKernel, "_ReadBuffer", buffer);
        computeShader.SetBuffer(vortexKernel, "_ReadBuffer", buffer);
        computeShader.SetBuffer(heightKernel, "_ReadBuffer", buffer);
        computeShader.SetBuffer(vortex2Kernel, "_ReadBuffer", buffer);
        computeShader.SetBuffer(invertKernel, "_ReadBuffer", buffer);
        computeShader.SetBuffer(sinKernel, "_ReadBuffer", buffer);
        //List<Particle> data = new List<Particle>();

        //for(int i = 0; i < texture.width; i++)
        //{
        //    for(int j = 0; j < texture.height; j++)
        //    {

        //    }
        //}

        ComputeBuffer.CopyCount(buffer, argBuffer, 0);
        argBuffer.GetData(args);

        // buffer.GetData(particles);
        Debug.Log("vertex count " + args[0]);
        Debug.Log("instance count " + args[1]);
        Debug.Log("start vertex " + args[2]);
        Debug.Log("start instance " + args[3]);
        this.prevMousePos = Input.mousePosition;
    }

    private void OnRenderObject()
    {
        if (this.buffer == null)
            return;
        material.SetPass(0);
        // this.buffer.GetData(particles);
        this.material.SetBuffer("_Buffer", this.buffer);
        Graphics.DrawProceduralIndirectNow(MeshTopology.Points, argBuffer, 0);
        // Graphics.DrawProceduralNow(MeshTopology.Points, 1, this.texture.width * this.texture.height);
    }

    public void OnDestroy()
    {
        if (this.buffer != null)
        {
            this.buffer.Dispose();
        }
        if (this.argBuffer != null)
            this.argBuffer.Dispose();
    }

    bool GetMousePos(out Vector3 pos)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane p = new Plane(Vector3.back, Vector3.zero);
        float dist = 0;
        pos = new Vector3();
        if (p.Raycast(ray, out dist))
        {
            pos = ray.GetPoint(dist);
            return true;
        }

        return true;

    }

    int GetKernel(EffectType effect)
    {
        if (effect == EffectType.GRAB)
            return this.computeKernel;
        if (effect == EffectType.VORTEX)
            return this.vortexKernel;
        else if (effect == EffectType.HEIGHT)
            return this.heightKernel;
        else if (effect == EffectType.VORTEX2)
            return this.vortex2Kernel;
        else if (effect == EffectType.SIN)
            return this.sinKernel;
        else
            return this.invertKernel;
    }

    private void Update()
    {
        // Fill Shader Parameters
        this.computeShader.SetFloat("_DeltaTime", Time.deltaTime);
        this.computeShader.SetBool("_Clicked", Input.GetKey(KeyCode.Mouse0));
        this.computeShader.SetFloat("_MaxSpeed", 1000);
        computeShader.SetFloat("_EffectStrength", this.effectStrength);
        
        Vector3 mousePosition;
        if (GetMousePos(out mousePosition))
        {
            this.prevMousePos = this.mousePos;
            this.mousePos = mousePosition;
            this.computeShader.SetVector("_MousePos", mousePos);
            this.computeShader.SetVector("_PrevMousePos", prevMousePos);
        }
        this.computeShader.SetFloat("_Radius", this.radius);

        foreach (var item in kernelList)
        {
            // Execute each kernel on the list
            this.computeShader.Dispatch(GetKernel(item), (1024 * 1024) / 256, 1, 1);
        }
    }

}
