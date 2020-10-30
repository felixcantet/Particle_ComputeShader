using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ParticleDrawer : MonoBehaviour
{
    public ComputeShader compute;

    public ComputeBuffer buffer;

    public Material material;

    private Camera camera;
    public float particleLifeTime = 3.0f;
    public struct Particle
    {
        public Vector3 position;
        public Vector3 velocity;
        public float life;
    }

    public Particle[] particles;
    public int particleSpawnPerFrame = 10;
    const int workGroupNumbers = 256;
    int threadNumber;
    int kernelID;
    public void Start()
    {
        this.camera = Camera.main;
        this.kernelID = compute.FindKernel("CSMain");
        this.particles = new Particle[0];
    }

    public void OnRenderObject()
    {
        if (this.buffer != null)
        {
            this.buffer.GetData(this.particles);
            material.SetBuffer("_Buffer", this.buffer);
            material.SetPass(0);
            //Graphics.DrawProcedural(material, new Bounds(Vector3.zero, new Vector3(200, 200, 200)), MeshTopology.Points, 4); 
            Graphics.DrawProceduralNow(MeshTopology.Points, 1, this.particles.Length);
        }
    }

    public void OnDestroy()
    {
        if (this.buffer != null)
            this.buffer.Release();
    }

    unsafe int GetSizeOf<T>() where T : unmanaged
    {
        return sizeof(T);
    }


    public void Update()
    {
        var mousePos = Input.mousePosition;
        var worldMousePos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -camera.transform.position.z));
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var newParticles = new Particle[10000];
            for (int i = 0; i < newParticles.Length; ++i)
            {
                newParticles[i] = new Particle { position = worldMousePos, life = Random.Range(0.0f, 5.0f) };
            }
            List<Particle> tmp = new List<Particle>(this.particles.Length + newParticles.Length);
            tmp.AddRange(this.particles);
            tmp.AddRange(newParticles);
            this.particles = tmp.ToArray();
            this.threadNumber = Mathf.CeilToInt((float)this.particles.Length / 256);
            Debug.Log(this.particles.Length);
            if (this.buffer != null)
                this.buffer.Release();
            this.buffer = new ComputeBuffer(this.particles.Length, GetSizeOf<Particle>());
            this.buffer.SetData(this.particles);
        }
        if (this.buffer == null)
            return;
        int id = Shader.PropertyToID("buffer");
        this.compute.SetVector("mousePosition", worldMousePos);
        this.compute.SetFloat("deltaTime", Time.deltaTime);
        this.compute.SetFloat("particleSize", 0.1f);
        this.compute.SetBuffer(kernelID, id, this.buffer);
        this.compute.Dispatch(kernelID, this.threadNumber, 1, 1);
        //Debug.Log(buffer.count);
    }

}
