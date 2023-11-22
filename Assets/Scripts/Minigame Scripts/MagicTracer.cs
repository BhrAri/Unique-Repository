using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MagicTracer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public GameObject drawingDot;
    public float checkLine;
    public float checkLineTime;
    public ParticleSystem dotParticleSystem;
    public bool leftTrailing;
    public bool rightTrailing;
    private ParticleSystem.EmissionModule emission;
    public int numberMade;
    private readonly string save = "/magicSave.json";
    public static Mesh vesh;
    public TMP_Text numberMadeText;
    private void Start()
    {
        emission = dotParticleSystem.emission;
        dotParticleSystem.Play();
        if (File.Exists(Application.persistentDataPath + save))
        {
            MagicSave magicSave = JsonUtility.FromJson<MagicSave>(File.ReadAllText(Application.persistentDataPath + save));
            numberMade += magicSave.magic;
        }
    }
    void Update()
    {
        if (Input.GetAxisRaw("Vertical") != 0)
        {
            drawingDot.transform.position += new Vector3(0f, Input.GetAxis("Vertical"), 0f) * Time.deltaTime;
            leftTrailing = true;
        }
        else { leftTrailing = false; }
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            drawingDot.transform.position += new Vector3(0f, 0f, -Input.GetAxis("Horizontal") * Time.deltaTime);
            rightTrailing = true;
        }
        else { rightTrailing = false; }
        if (leftTrailing || rightTrailing)
        {
            drawingDot.transform.rotation = Quaternion.Euler(GetXRotation(), 0f, 0f);
            emission.enabled = true;
        } else 
        {
            emission.enabled = false;
        }
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, drawingDot.transform.position + new Vector3 (-1f, 0f, 0f));
        checkLine += Time.deltaTime;
        if (checkLine >= checkLineTime)
        {
            lineRenderer.Simplify(0.01f);
            checkLine = 0f;
        }
    }
    public float GetXRotation()
    {
        if (Input.GetAxisRaw("Vertical") != 0) { return Input.GetAxisRaw("Vertical") * (90 - (Input.GetAxis("Horizontal") * 90)); }
        else { return 90 - (Input.GetAxisRaw("Horizontal") * 90); }
    }
    public void SaveSigil()
    {
        lineRenderer.BakeMesh(vesh);
    }
    public void Restart()
    {
        numberMade++;
        numberMadeText.text = "Number Made: " + numberMade;
        drawingDot.transform.position = new Vector3(11.3999996f, 4.04965734f, -2.82285714f);
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, drawingDot.transform.position + new Vector3(-1f, 0f, 0f));
        lineRenderer.Simplify(1000);
    }
    public void EndMinigame()
    {
        MagicSave magicSave = new() { magic = numberMade };
        File.WriteAllText(Application.persistentDataPath + save, JsonUtility.ToJson(magicSave));
        SceneManager.UnloadSceneAsync(4);
    }
    [System.Serializable]
    class MagicSave
    {
        public int magic;
    }
}
