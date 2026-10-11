using UnityEngine;
using UnityEngine.UI;

public class EduRuleDisplay : MonoBehaviour
{
    public int ruleID;
    Main main;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        main = GameObject.Find("Main Camera").GetComponent<Main>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"({main.save.eduRule[ruleID].min:#} ~ {main.save.eduRule[ruleID].max:#})";
    }
}
