using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DexterityTotem : MonoBehaviour
{
    public GameObject top;
    public float topSpeed;
    public bool topHittable;
    public GameObject topDisplay;
    public float topRotated;
    public GameObject middle;
    public float middleSpeed;
    public bool middleHittable;
    public GameObject middleDisplay;
    public float middleRotated;
    public GameObject bottom;
    public float bottomSpeed;
    public bool bottomHittable;
    public GameObject bottomDisplay;
    public float bottomRotated;
    public bool lost;
    public float timeSurvived;
    public GameObject retryButton;
    public int points;
    private readonly string save = "/dexteritySave.json";
    public TMP_Text scoreText;
    void Start()
    {
        topSpeed = -30f;
        middleSpeed = 17f;
        bottomSpeed = -10f;
        if (File.Exists(Application.persistentDataPath + save))
        {
            DexteritySave dexteritySave = JsonUtility.FromJson<DexteritySave>(File.ReadAllText(Application.persistentDataPath + save));
            points = dexteritySave.dexterity;
        }
    }

    void Update()
    {
        if (!lost)
        {
            LossCheck();
            HittableCheck();
            DisplayHittables();
            if (Input.GetKeyDown(KeyCode.W) && topHittable)
            {
                topSpeed *= -1.1f;
                topRotated = 0;
            }
            if (Input.GetKeyDown(KeyCode.A) && middleHittable || Input.GetKeyDown(KeyCode.D) && middleHittable)
            {
                middleSpeed *= -1.3f;
                middleRotated = 0;
            }
            if (Input.GetKeyDown(KeyCode.S) && bottomHittable)
            {
                bottomSpeed *= -1.5f;
                bottomRotated = 0;
            }
            RotateStick(top.transform, topSpeed);
            RotateStick(middle.transform, middleSpeed);
            RotateStick(bottom.transform, bottomSpeed);
            timeSurvived += Time.deltaTime;
            scoreText.text = "Time Survived: " + timeSurvived.ToString();
        } else { retryButton.SetActive(true); }
    }
    private void RotateStick(Transform stick, float speed)
    {
        stick.Rotate(speed * Time.deltaTime * Vector3.down);
    }
    private void LossCheck()
    {
        topRotated += Time.deltaTime * topSpeed;
        middleRotated += Time.deltaTime * middleSpeed;
        bottomRotated += Time.deltaTime * bottomSpeed;
        if (topRotated > 460 ||  middleRotated > 460 ||  bottomRotated > 460 ||
            topRotated < -460 || middleRotated < -460 || bottomRotated < -460)
        {
            lost = true;
        }
    }
    private void HittableCheck()
    {
        if (top.transform.localEulerAngles.y >= 30 && top.transform.localEulerAngles.y <= 90)
        {
            topHittable = true;
        }
        else { topHittable = false; }
        if (middle.transform.localEulerAngles.y >= 270 && middle.transform.localEulerAngles.y <= 330)
        {
            middleHittable = true;
        }
        else { middleHittable = false; }
        if (bottom.transform.localEulerAngles.y >= 1 && bottom.transform.localEulerAngles.y <= 115)
        {
            bottomHittable = true;
        }
        else { bottomHittable = false; }
    }
    private void DisplayHittables()
    {
        topDisplay.SetActive(topHittable);
        topDisplay.transform.localRotation = Quaternion.Euler(0f,180f - top.transform.localEulerAngles.y,0f);
        middleDisplay.SetActive(middleHittable);
        middleDisplay.transform.localRotation = Quaternion.Euler(0f, 180f - middle.transform.localEulerAngles.y, 0f);
        bottomDisplay.SetActive(bottomHittable);
        bottomDisplay.transform.localRotation = Quaternion.Euler(30f + bottom.transform.localEulerAngles.y, 180f, 90f);
    }
    public void Retry()
    {
        retryButton.SetActive(false);
        topSpeed = -30f;
        middleSpeed = 17f;
        bottomSpeed = -10f;
        timeSurvived = 0f;
        points += Mathf.RoundToInt(timeSurvived);
        lost = false;
    }
    public void EndMinigame()
    {
        DexteritySave dexteritySave = new() { dexterity = points };
        File.WriteAllText(Application.persistentDataPath + save, JsonUtility.ToJson(dexteritySave));
        SceneManager.UnloadSceneAsync(5);
    }
    [System.Serializable]
    class DexteritySave
    {
        public int dexterity;
    }
}
