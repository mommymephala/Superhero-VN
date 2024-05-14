using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public struct StoryImage
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
    private bool _isTextFullyDisplayed;
    private Coroutine _textCoroutine;

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

    public void UpdateText(string text, List<Choice> choices, Action<Choice> onClickChoice, Action onComplete)
    {
        if (_textCoroutine != null)
        {
            StopCoroutine(_textCoroutine);
        }

        _textCoroutine = StartCoroutine(DisplayTextCoroutine(text, choices, onClickChoice, onComplete));
    }

    private IEnumerator DisplayTextCoroutine(string text, List<Choice> choices, Action<Choice> onClickChoice, Action onComplete)
    {
        _isTextFullyDisplayed = false;

        GameObject textboxObj = Instantiate(textbox, canvas.transform, false);
        var appearingText = textboxObj.GetComponentInChildren<AppearingText>();

        if (appearingText != null)
        {
            appearingText.ShowText(text);
        }
        else
        {
            var textMesh = textboxObj.GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = text;
            }
        }

        while (!IsTextFullyDisplayed())
        {
            if (Input.GetMouseButtonDown(0))
            {
                SkipTextAnimation(textboxObj);
                break;
            }
            yield return null;
        }

        _isTextFullyDisplayed = true;
        onComplete?.Invoke();

        // Ensure choices are displayed after the text box
        if (choices.Count > 0)
        {
            CreateChoiceButtons(choices, onClickChoice);
        }
    }

    private bool IsTextFullyDisplayed()
    {
        var appearingText = FindObjectOfType<AppearingText>();
        return appearingText == null || appearingText.GetComponent<TextMeshProUGUI>().text == appearingText.fullText;
    }

    private void SkipTextAnimation(GameObject textboxObj)
    {
        var appearingText = textboxObj.GetComponentInChildren<AppearingText>();
        if (appearingText != null)
        {
            StopCoroutine(appearingText.GetComponent<AppearingText>().typingCoroutine);
            appearingText.GetComponent<TextMeshProUGUI>().text = appearingText.fullText;
        }
    }

    public void CreateChoiceButtons(List<Choice> choices, Action<Choice> onClick)
    {
        // Clear any existing choice buttons
        foreach (Transform child in canvas.transform)
        {
            if (child.GetComponent<Button>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        // Create new choice buttons
        foreach (var choice in choices)
        {
            Button button = Instantiate(buttonPrefab, canvas.transform, false);
            button.GetComponentInChildren<TextMeshProUGUI>().text = choice.text.Trim();
            button.onClick.AddListener(() => onClick(choice));
        }
    }

    public void TransitionImage(string imageName)
    {
        if (_storyImages.TryGetValue(imageName, out Image newImage))
        {
            foreach (Image img in _storyImages.Values)
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
    
    public void CreateQuitButton()
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

    public void SetActiveStateForTextbox(bool isActive)
    {
        textbox.SetActive(isActive);
        foreach (Transform child in textbox.transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }

    private static IEnumerator FadeIn(Image image)
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

    private static IEnumerator FadeOut(Image image)
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

    public void InitializeStoryImages()
    {
        _storyImages.Clear();
        foreach (StoryImage item in storyImagesList)
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