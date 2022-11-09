using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class PlatformButton : MonoBehaviour
{
    public MyDictionary.Platform referenceAsset;

    private void Start()
    {
        Image image = gameObject.transform.GetComponent<Image>();
        image.sprite = null;
        //Clone the material so we don't modify the original
        image.material = new Material(image.material);
        image.material.mainTexture = AssetPreview.GetAssetPreview(LevelEditor.instance.getPlatformObject(referenceAsset));

        //Use own name on text element in child
        TMP_Text text = GetComponentInChildren<TMP_Text>();
        text.text = name;
    }

    public void onClick()
    {
        LevelEditor.instance.buttonClicked(referenceAsset);
    }
}