using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class StoryImage
{
    public string name;
    public Image image;
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject textbox;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private List<StoryImage> storyImagesList;

    private readonly Dictionary<string, Image> _storyImages = new Dictionary<string, Image>();

    private void Awake()
    {
        InitializeStoryImages();
    }

    public void ClearUI()
    {
        for (var i = canvas.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(canvas.transform.GetChild(i).gameObject);
        }
    }

    public void UpdateText(string text)
    {
        GameObject textboxObj = Instantiate(textbox, canvas.transform, false);
        var textMesh = textboxObj.GetComponentInChildren<TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = text;
        }
    }

    public void CreateChoiceButtons(List<Choice> choices, Action<Choice> onClick)
    {
        foreach (var choice in choices)
        {
            Button button = Instantiate(buttonPrefab, canvas.transform, false);
            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text.Trim();
            button.onClick.AddListener(() => onClick(choice));
        }

        if (choices.Count == 0)
        {
            Button endButton = Instantiate(buttonPrefab, canvas.transform, false);
            endButton.GetComponentInChildren<TextMeshProUGUI>().text = "End of story. Press to quit.";
            endButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }
    }

    public void TransitionImage(string imageName)
    {
        if (_storyImages.TryGetValue(imageName, out Image newImage))
        {
            foreach (var img in _storyImages.Values)
            {
                if (img != newImage && img.gameObject.activeSelf)
                {
                    StartCoroutine(FadeOut(img));
                }
            }
            if (!newImage.gameObject.activeSelf || newImage.GetComponent<CanvasGroup>().alpha < 1)
            {
                StartCoroutine(FadeIn(newImage));
            }
        }
    }

    public void SetActiveStateForTextbox(bool isActive)
    {
        textbox.SetActive(isActive);
        foreach (Transform child in textbox.transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    private IEnumerator FadeIn(Image image)
    {
        var canvasGroup = image.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError($"CanvasGroup is null on image: {image.name}");
            yield break;
        }

        image.gameObject.SetActive(true);
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    private IEnumerator FadeOut(Image image)
    {
        var canvasGroup = image.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError($"CanvasGroup is null on image: {image.name}");
            yield break;
        }

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        image.gameObject.SetActive(false);
    }

    private void InitializeStoryImages()
    {
        _storyImages.Clear();
        foreach (var item in storyImagesList)
        {
            if (!_storyImages.ContainsKey(item.name))
            {
                _storyImages[item.name] = item.image;
                var canvasGroup = item.image.GetComponent<CanvasGroup>() ?? item.image.gameObject.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
}