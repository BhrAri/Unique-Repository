using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GrowYourBones : MonoBehaviour
{
    public GameObject bone;
    public Camera boneCamera;
    public float boneScore;
    public List<int> boneScores;
    public int TotalScore;
    public float boneHoldingTimeVertical;
    public float boneHoldingTimeHorizontal;
    private bool boneBroke;
    private bool boneHelping;
    private bool boneSubmitted;
    public GameObject boneHelp;
    public GameObject boneEndScreen;
    public TMP_Text boneScoreText;
    public TMP_Text boneHelpButtonText;
    public Material boneMaterial;
    public string save = "/hpSave.json";
    void Start()
    {
        //reset stuff just in case
        bone.transform.localScale = Vector3.one;
        boneCamera.transform.position = new Vector3(-0.05f, 0.36f, -2f);
        boneBroke = false;
        if (File.Exists(Application.persistentDataPath + save))
        {
            TotalBoneScore score = JsonUtility.FromJson<TotalBoneScore>(File.ReadAllText(Application.persistentDataPath + save));
            boneScores.Add(score.total);
        }
    }
    void Update()
    {
        //one if statement for both ensures only one direction can be input at a time
        if (!boneHelping || !boneSubmitted)
        {
            if (!boneBroke)
            {
                if (Input.GetAxis("Horizontal") > 0.3f || Input.GetAxis("Horizontal") < -0.3f)
                {
                    bone.transform.localScale += (new Vector3(0f, 0f, Input.GetAxis("Horizontal") * Time.deltaTime));
                    boneHoldingTimeHorizontal += Time.deltaTime;
                    if (boneHoldingTimeVertical > 0)
                    {
                        boneHoldingTimeVertical -= Time.deltaTime / 2f;
                    }
                    if (boneHoldingTimeHorizontal > 5)
                    {
                        boneBroke = true;
                    }
                }
                else if (Input.GetAxis("Vertical") > 0.1f || Input.GetAxis("Vertical") < -0.1f)
                {
                    bone.transform.localScale += new Vector3(0f, Input.GetAxis("Vertical") * Time.deltaTime, 0f);
                    boneHoldingTimeVertical += Time.deltaTime;
                    if (boneHoldingTimeHorizontal > 0)
                    {
                        boneHoldingTimeHorizontal -= Time.deltaTime / 2f;
                    }
                    if (boneHoldingTimeVertical > 5)
                    {
                        boneBroke = true;
                    }
                }
                else
                {
                    if (boneHoldingTimeHorizontal > 0)
                    {
                        boneHoldingTimeHorizontal -= Time.deltaTime / 2f;
                    }
                    if (boneHoldingTimeVertical > 0)
                    {
                        boneHoldingTimeVertical -= Time.deltaTime / 2f;
                    }
                }
                boneCamera.transform.position = new Vector3(-0.05f, (bone.transform.localScale.y - 1f) * 0.3f, -bone.transform.localScale.y - 2f);
            }
            else
            {
                BoneSubmit();
            }
            BoneColoration();
        }
    }
    public void BoneRestart()
    {
        BoneScoreGet();
        boneScores.Add(Mathf.RoundToInt(boneScore));
        bone.transform.localScale = Vector3.one;
        boneCamera.transform.position = new Vector3(-0.05f, 0.36f, -2f);
        boneHoldingTimeHorizontal = 0;
        boneHoldingTimeVertical = 0;
        boneBroke = false;
        boneSubmitted = false;
        boneEndScreen.SetActive(false);
    }
    public void BoneExit()
    {
        boneScores.Add(Mathf.RoundToInt(boneScore));
        int totalScore = 0;
        foreach (int score in boneScores) 
        {
            totalScore += score;
        }
        TotalBoneScore Score = new()
        {
            total = totalScore
        };
        File.WriteAllText(Application.persistentDataPath + save, JsonUtility.ToJson(Score));
        SceneManager.UnloadSceneAsync(2); //this should be fine because this scene shouldn't be open unless the main scene is open too
    }
    public void BoneHelp()
    {
        if (boneHelp.activeInHierarchy)
        {
            boneHelp.SetActive(false);
            boneHelpButtonText.text = "Help";
            boneHelping = false;
        }
        else
        {
            boneHelping = true;
            boneHelp.SetActive(true);
            boneHelpButtonText.text = "Close";
        }
    }
    public void BoneScoreGet()
    {
        if (bone.transform.localScale.z > bone.transform.localScale.y)
        {
            boneScore = (bone.transform.localScale.y / bone.transform.localScale.z) * (bone.transform.localScale.y + bone.transform.localScale.z);
        }
        else if (bone.transform.localScale.z != bone.transform.localScale.y)
        {
            boneScore = (bone.transform.localScale.z / bone.transform.localScale.y) * (bone.transform.localScale.y + bone.transform.localScale.z);
        }
        else
        {
            if (bone.transform.localScale.y != 1)
            {
                boneScore = 100f * (bone.transform.localScale.y + bone.transform.localScale.z);
            }
            else
            {
                //cheater cheater
                boneScore = 0;
            }
        }
        if (boneBroke)
        {
            boneScore /= 2;
        }
    }
    public void BoneSubmit()
    {
        if (!boneSubmitted && !boneHelping)
        {
            boneSubmitted = true;
            boneEndScreen.SetActive(true);
            BoneScoreGet();
            if (boneScore > 30f)
            {
                boneScoreText.text = Mathf.RoundToInt(boneScore).ToString() + "\nWhat a lovely bone you've grown";
            }
            if (boneScore <= 0f)
            {
                boneScoreText.text = "Bone small \nBrain small";
            }
            else
            {
                boneScoreText.text = Mathf.RoundToInt(boneScore).ToString();
            }
        }
    }
    [System.Serializable]
    class TotalBoneScore
    {
        public int total;
    }
    private void BoneColoration()
    {
        float a = boneHoldingTimeHorizontal;
        float b = boneHoldingTimeVertical;
        if (b > 5 || b < 0)
        {
            b = Mathf.RoundToInt(b);
        }
        boneMaterial.SetFloat("_BumpScale", Mathf.RoundToInt(a));
        boneMaterial.color = new Color(1,1-(b/5),1-(b/5),1);
    }
}
