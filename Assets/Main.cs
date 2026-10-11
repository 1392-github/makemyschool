using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
public class Main : MonoBehaviour
{
    public Save1 save;
    public Text dateText;
    public Text moneyText;
    public Data data;
    public GameObject resetUI;
    public Text resetText;
    public Text resetph;
    public Text warningCnt;
    public Text areaDisplay;
    public InputField areaInput;
    public GameObject dialog;
    public Text dialogText;
    public InputField amountOfClassInput;
    public string[] warningColor;
    FieldInfo[] edurulefield;
    string resetCode;
    public float saveTmr = 10;
    void Start()
    {
        edurulefield = new FieldInfo[data.eduRule.Count];
        Type t = typeof(Save1);
        for (int i = 0; i < edurulefield.Length; i++)
        {
            edurulefield[i] = t.GetField(data.eduRule[i].var);
        }
        if (File.Exists(Application.persistentDataPath + "/save"))
        {
            save = JsonUtility.FromJson<Save1>(File.ReadAllText(Application.persistentDataPath + "/save"));
        }
        else
        {
            save.eduRule = new Range[data.eduRule.Count];
            for (int i = 0; i < data.eduRule.Count; i++)
            {
                save.eduRule[i] = new Range();
            }
            UpdateEduRule();
            Save();
        }
        UpdateWarning();
    }
    void Update()
    {
        if (save.reamingDay == -1)
        {
            dateText.text = $"{save.year}년 준비기간";
        }
        else
        {
            dateText.text = $"{save.year}년 ({save.reamingDay}일 남음)";
        }
        moneyText.text = $"예산 : {save.money:N0}원";
        saveTmr -= Time.deltaTime;
        if (saveTmr <= 0)
        {
            saveTmr = 10;
            Save();
        }
        UpdateAreaText();
    }
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }
    void OnApplicationQuit()
    {
        Save();
    }
    void Save()
    {
        File.WriteAllText(Application.persistentDataPath + "/save", JsonUtility.ToJson(save));
    }
    public void OpenReset()
    {
        resetCode = "";
        for (int i = 0; i < 10; i++)
        {
            resetCode += (char)Random.Range(97, 123);
        }
        resetText.text = $"정말로 폐교(초기화)하시겠습니다?\n이는 취소가 불가능합니다\n\n폐교(초기화)를 진행하려면, 아래 상자에 \"{resetCode}\"를 입력하고[Enter]을 눌러주세요";
        resetph.text = $"\"{resetCode}\" 입력";
        resetUI.SetActive(true);
    }
    public void OpenDialog(string content)
    {
        dialogText.text = content;
        dialog.SetActive(true);
        dialog.transform.SetAsLastSibling();
    }
    public void Reset(string code)
    {
        if (resetCode == code)
        {
            File.Delete(Application.persistentDataPath + "/save");
            SceneManager.LoadScene("MainScene");
        }
    }
    public void UpdateWarning()
    {
        warningCnt.text = $"경고 횟수 : <color=#{warningColor[save.warning]}>{save.warning}</color> / 5";
    }
    public void UpdateAreaText()
    {
        areaDisplay.text = $@"전체 공간 {save.area} ㎡
빈 공간 {GetEmptyArea()} ㎡
교장실 30 ㎡
교무실 60 ㎡
교실 {save.amountOfClass * 60} ㎡ (1반당 60 ㎡, 총 {save.amountOfClass}반)";
    }
    public int GetEmptyArea() => save.area - 90 - save.amountOfClass * 60;
    public void BuyArea()
    {
        int area = int.Parse(areaInput.text);
        long cost = area * 10_000_000L;
        if (save.money >= cost)
        {
            save.money -= cost;
            save.area += area;
            UpdateAreaText();
        }
        else
        {
            OpenDialog("돈이 부족합니다");
        }
    }
    public void SellArea()
    {
        int area = int.Parse(areaInput.text);
        long cost = area * 7_000_000L;
        if (GetEmptyArea() >= area)
        {
            save.area -= area;
            save.money += cost;
            UpdateAreaText();
        }
        else
        {
            OpenDialog("빈 공간이 부족합니다");
        }
    }
    public void UpdateEduRule()
    {
        for (int i = 0; i < data.eduRule.Count; i++)
        {
            //if (save.year == 2025)
            //{
            int infdetect = 0;
            do
            {
                if (data.eduRule[i].minRange.min != 0 || data.eduRule[i].minRange.max != 0)
                {
                    save.eduRule[i].min = Random.Range(data.eduRule[i].minRange.min, data.eduRule[i].minRange.max + 1);
                }
                if (data.eduRule[i].maxRange.min != 0 || data.eduRule[i].maxRange.max != 0)
                {
                    save.eduRule[i].max = Random.Range(data.eduRule[i].maxRange.min, data.eduRule[i].maxRange.max + 1);
                }
                infdetect++;
                if (infdetect >= 1000)
                {
                    Debug.Log("INFINITY LOOP DETECTED");
                    break;
                }
            } while (save.eduRule[i].min > save.eduRule[i].max);
            /*}
            else
            {
                save.eduRule[i].min = Random.Range(data.eduRule[i].range.min, data.eduRule[i].range.max + 1);
                save.eduRule[i].max = Random.Range(save.eduRule[i].min, data.eduRule[i].range.max + 1);
                if (save.eduRule[i].min > (int)edurulefield[i].GetValue(save))
                {
                    edurulefield[i].SetValue(save, save.eduRule[i].min);
                }
                if (save.eduRule[i].max < (int)edurulefield[i].GetValue(save))
                {
                    edurulefield[i].SetValue(save, save.eduRule[i].max);
                }
            }*/
        }
    }
    public void ChangeAmountOfClass(string sval)
    {
        int val = int.Parse(sval);
        if (val > 1_000_000) // 오버플로 오류 방지
        {
            OpenDialog("가능한 최대 학급 수는 1,000,000학급입니다 (근데 거기까지 갈 일이 있으려나)");
            amountOfClassInput.text = save.amountOfClass.ToString();
            return;
        }
        if (val <= 0)
        {
            OpenDialog("가능한 최소 학급 수는 1학급입니다");
            amountOfClassInput.text = save.amountOfClass.ToString();
            return;
        }
        if ((val - save.amountOfClass) * 60 <= GetEmptyArea())
        {
            save.amountOfClass = val;
        }
        else
        {
            OpenDialog($"학급 수를 {sval}학급으로 늘리는 데 빈 공간이 부족합니다. 공간 관리에서 빈 공간을 {(val - save.amountOfClass) * 60 - GetEmptyArea()} ㎡ 이상 더 구매하세요");
            amountOfClassInput.text = save.amountOfClass.ToString();
        }
    }
    public void EndPrep()
    {
        save.reamingDay = save.totalDay;
    }
}
