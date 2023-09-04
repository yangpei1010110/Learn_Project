using System;
using SandBox;
using SandBox.Elements.Gas;
using SandBox.Elements.Liquid;
using SandBox.Elements.Solid;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Void = SandBox.Elements.Void.Void;

public class ElementButtonInit : MonoBehaviour
{
    public GameObject TempButton;
    public int        buttonHeight = 50;

    private GameObject[] children = Array.Empty<GameObject>();

    void Start()
    {
        var elements = new[]
        {
            typeof(Void),
            typeof(Sand),
            typeof(Water),
            typeof(Smoke),
        };
        children = new GameObject[elements.Length];
        // set content height
        var height = elements.Length * buttonHeight;
        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(0, height);

        // generate buttons
        for (int i = 0; i < elements.Length; i++)
        {
            children[i] = Instantiate(TempButton, transform);
            var buttonRect = children[i].GetComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(0, buttonHeight);
            buttonRect.anchoredPosition = new Vector2(0, -i * buttonHeight);
            children[i].GetComponentInChildren<TextMeshProUGUI>().SetText(elements[i].Name);
            var elementType = elements[i];
            children[i].GetComponent<Button>().onClick.AddListener(() =>
            {
                SandBoxWorld.Selected = elementType;
            });
        }
    }
}