using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlyPrep : MonoBehaviour
{
    Selectable target;
    Main main;
    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<Selectable>();
        main = Camera.main.GetComponent<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        target.interactable = main.save.reamingDay == -1;
    }
}
