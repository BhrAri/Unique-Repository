using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEditor.UI;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class LIFT : MonoBehaviour
{
    public TextMeshProUGUI liftCounter;
    public TextMeshProUGUI feelingItText;
    public Scrollbar liftProgress;
    private float weightWeight = 1;
    private int timesLifted = 0;
    private float strength;
    public float weightLighteningValue;
    [SerializeField]private float feelingIt = 1;
    private Rigidbody weight;
    private readonly string save = "/strengthSave.json";
    
    void Start()
    {
        weight = GetComponent<Rigidbody>();
        if (File.Exists(Application.persistentDataPath + save))
        {
            StrengthSave strengthSave = JsonUtility.FromJson<StrengthSave>(Application.persistentDataPath + save);
            strength = strengthSave.strength;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            weight.AddForce((Vector3.up * weightLighteningValue + new Vector3 (0f, strength, 0f) * feelingIt), ForceMode.Impulse);
            feelingIt += 0.13f;
        }
        liftProgress.value = (transform.position.y - 0.5f) / 0.9f;
        if (feelingIt > 1)
        {
            feelingIt -= Time.deltaTime;
        }
        if (feelingIt > 2)
        {
            feelingItText.enabled = true;
            feelingItText.text = "FEELING IT! x " + feelingIt;
        } else
        {
            feelingItText.enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        weight.transform.position = new (0f, 0.5f, 1.2f);
        weight.mass += 0.5f;
        timesLifted++;
        weightWeight ++;
        liftCounter.text = "Lifted: " + timesLifted + "\nWeight:" + weightWeight;
        strength += 0.1f;
        weight.velocity = Vector3.zero;
    }
    public void EndMinigame()
    {
        StrengthSave strengthSave = new() { strength = strength };
        File.WriteAllText(Application.persistentDataPath + save, JsonUtility.ToJson(strengthSave));
        SceneManager.UnloadSceneAsync(3);
    }
    [System.Serializable]
    class StrengthSave
    {
        public float strength;
    }
}
