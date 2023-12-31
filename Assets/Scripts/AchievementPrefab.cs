using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementPrefab : MonoBehaviour
{
    public Achievement achievement;
    public AchievementData achievementData;

    public Sprite bgSelesai;
    public Sprite bgBelumSelesai;

    public Image bgImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI valueText;

    bool start;
    public void UpdateAchievement()
    {
        if (!start)
        {
            start = true;
            Load();
            SetChild();
        }

        titleText.text = achievement.namaAchievement;
        valueText.text = achievementData.value + "/" + achievement.maxValue;

        //Cek selesai
        if (!achievementData.selesai && achievementData.value >= achievement.maxValue)
        {
            achievementData.selesai = true;

            UIManager.instance.SpawnNotifAct(achievement.namaAchievement);

            SetChild();
        }

        void SetChild()
        {
            if (achievementData.selesai)
            {
                bgImage.sprite = bgSelesai;
                titleText.color = Color.black;
                valueText.color = Color.black;
                transform.SetAsFirstSibling();
            }
            else
            {
                bgImage.sprite = bgBelumSelesai;
                titleText.color = Color.white;
                valueText.color = Color.white;
                transform.SetAsLastSibling();
            }
        }

    }

    public void Save()
    {
        string createFolder = Application.persistentDataPath + "/AchievementData";
        if (!System.IO.File.Exists(createFolder))
        {
            System.IO.Directory.CreateDirectory(createFolder);
        }

        string filePath = Application.persistentDataPath + "/AchievementData/" + achievement.name + ".jhon";
        string data = JsonUtility.ToJson(achievementData);
        System.IO.File.WriteAllText(filePath, data);
    }

    public void Load()
    {
        string filePath = Application.persistentDataPath + "/AchievementData/" + achievement.name + ".jhon";
        if (System.IO.File.Exists(filePath))
        {
            string data = System.IO.File.ReadAllText(filePath);
            achievementData = JsonUtility.FromJson<AchievementData>(data);
        }

    }
}
[System.Serializable]
public class AchievementData
{
    public bool selesai;
    public int value;
}
