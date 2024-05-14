using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Ink.Runtime;

[Serializable]
public class StoryImage
{
    public string name;
    public Image image;
}

[Serializable]
public class SoundClip
{
    public AudioClip clip;
    public float volume = 1.0f;
}

public class StoryManager : MonoBehaviour
{
    [SerializeField] private TextAsset inkJsonAsset;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject textbox;
    [SerializeField] private Button buttonPrefab;
    [SerializeField] private CRTController crtController;
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private SoundClip[] soundClips;

    [SerializeField]
    private List<StoryImage> storyImagesList = new List<StoryImage>();
    private Dictionary<string, Image> _storyImages = new Dictionary<string, Image>();

    private Story _story;
    private bool _uiLocked;

    private void Awake()
    {
        InitializeStoryImages();
        RemoveChildren();
        StartStory();
    }

    private void StartStory()
    {
        _story = new Story(inkJsonAsset.text);
        RefreshView();
    }

    private void RefreshView()
    {
        RemoveChildren();
        
        var fullText = "";
        var foundPlaySound = false;

        while (_story.canContinue)
        {
            var text = _story.Continue().Trim();
            fullText += text + (_story.canContinue ? "\n" : "");
            // Check for play_sound tag in the current set of tags
            foreach (var tag in _story.currentTags)
            {
                if (tag.StartsWith("play_sound"))
                {
                    foundPlaySound = true;
                }
            }
            
            HandleTags(_story.currentTags);
        }
        
        // If no play_sound tag is found, stop the music
        if (!foundPlaySound)
        {
            backgroundAudioSource.Stop();
        }

        foreach (Choice choice in _story.currentChoices)
        {
            Button button = CreateChoiceView(choice.text.Trim());
            button.onClick.AddListener(() => OnClickChoiceButton(choice));
        }

        if (_story.currentChoices.Count == 0)
        {
            Button choice = CreateChoiceView("End of story. Press to quit.");
            choice.onClick.AddListener(RefreshFinishView);
        }
        
        CreateContentView(fullText);
    }
    
    private void RefreshFinishView()
    {
        if (_story.currentChoices.Count == 0)
        {
            // If no choices are left, quit the application
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    private void HandleTags(List<string> tags)
    {
        // Debug.Log("Processing tags: " + String.Join(", ", tags));
        foreach (var tag in tags)
        {
            var parts = tag.Split(':');
            switch (parts[0].Trim())
            {
                case "play_sound":
                    PlaySound(parts[1].Trim(), false, true);
                    break;
                case "change_image":
                    // ChangeImage(parts[1].Trim());
                    TransitionImage(parts[1].Trim());
                    break;
                case "change_preset":
                    ChangePreset(int.Parse(parts[1].Trim()));
                    break;
                case "lerp_preset":
                    LerpToPreset();
                    break;
                case "zoom":
                    ApplyZoom(parts[1].Trim());
                    break;
                case "hide_textbox":
                    HideTextbox();
                    break;
                case "activate_textbox":
                    ActivateTextbox();
                    break;
                default:
                    Debug.LogError("Unhandled tag: " + parts[0].Trim());
                    break;
            }
        }
    }
    
    private void TransitionImage(string imageName)
    {
        if (_storyImages.ContainsKey(imageName))
        {
            var newImage = _storyImages[imageName];
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
        else
        {
            Debug.LogError("Image not found in dictionary: " + imageName);
        }
    }

    private IEnumerator FadeIn(Image image) 
    {
        LockUI(true);
        image.gameObject.SetActive(true);
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();
        while (canvasGroup.alpha < 1) 
        {
            canvasGroup.alpha += Time.deltaTime / 1f; // Adjust fade duration as needed
            yield return null;
        }
        
        LockUI(false);
    }

    private IEnumerator FadeOut(Image image)
    {
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();
        while (canvasGroup.alpha > 0) 
        {
            canvasGroup.alpha -= Time.deltaTime / 1f; // Adjust fade duration as needed
            yield return null;
        }
        
        image.gameObject.SetActive(false); // Deactivate after fading out
    }

    private void LockUI(bool shouldLock)
    {
        _uiLocked = shouldLock;
        foreach (var button in FindObjectsOfType<Button>())
        {
            button.interactable = !shouldLock;
        }
    }
    
    private void ActivateTextbox()
    {
        textbox.SetActive(true);
        foreach (Transform child in textbox.transform)
        {
            child.gameObject.SetActive(true);  // Ensures all children are also active
        }
    }

    private void HideTextbox()
    {
        textbox.SetActive(false);
    }
    
    private void InitializeStoryImages() 
    {
        _storyImages = new Dictionary<string, Image>();
        foreach (StoryImage item in storyImagesList) 
        {
            if (!_storyImages.ContainsKey(item.name)) 
            {
                _storyImages.Add(item.name, item.image);
                CanvasGroup canvasGroup = item.image.GetComponent<CanvasGroup>();
                if (canvasGroup == null) 
                {
                    canvasGroup = item.image.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false; // Make sure the image doesn't block clicks when invisible
            }
        }
    }
    
    private void ChangePreset(int presetIndex)
    {
        if (crtController != null)
        {
            crtController.ChangeCRTSetting(presetIndex);
        }
    }

    private void LerpToPreset()
    {
        if (crtController != null)
        {
            crtController.LerpToNextPreset();
        }
    }

    private void ApplyZoom(string zoomLevels)
    {
        var levels = zoomLevels.Split(',');
        if (levels.Length >= 3)
        {
            var startZoom = float.Parse(levels[0].Trim());
            var endZoom = float.Parse(levels[1].Trim());
            var duration = float.Parse(levels[2].Trim());
            crtController.SetZoom(startZoom, endZoom, duration);
        }
    }

    private void ChangeImage(string imageName)
    {
        if (_storyImages.ContainsKey(imageName))
        {
            // If the image is found in the dictionary, simply activate it and deactivate others.
            foreach (Image img in _storyImages.Values)
            {
                img.gameObject.SetActive(false); // First, disable all images.
            }
            
            _storyImages[imageName].gameObject.SetActive(true); // Then enable the requested image.
        }
        else
        {
            Debug.LogError("Image not found in dictionary: " + imageName);
        }
    }

    private void PlaySound(string soundName, bool isUISound = false, bool loop = false)
    {
        AudioSource source = isUISound ? uiAudioSource : backgroundAudioSource;
        SoundClip soundClip = Array.Find(soundClips, s => s.clip.name.Equals(soundName, StringComparison.OrdinalIgnoreCase));

        if (soundClip != null && soundClip.clip != null)
        {
            // Stop previous music if this is background music and not a UI sound
            if (!isUISound && source.isPlaying)
            {
                source.Stop();
            }

            source.clip = soundClip.clip;
            source.volume = soundClip.volume;
            source.loop = loop; // Set whether this track should loop
            source.Play();
        }
        else
        {
            Debug.LogError("Sound not found: " + soundName);
        }
    }

    private void OnClickChoiceButton(Choice choice)
    {
        if (!_uiLocked)
        {
            PlayButtonClickSound();
            _story.ChooseChoiceIndex(choice.index);
            RefreshView();
        }
    }
    
    private void PlayButtonClickSound()
    {
        if (uiAudioSource != null && buttonClickSound != null)
        {
            uiAudioSource.PlayOneShot(buttonClickSound); // Play a sound assigned to buttonClickSound
        }
        else
        {
            Debug.LogError("AudioSource or buttonClickSound is not assigned.");
        }
    }
    
    private void CreateContentView(string text)
    {
        GameObject textboxObj = Instantiate(textbox, canvas.transform, false);
        var appearingText = textboxObj.GetComponentInChildren<AppearingText>();
        
        if (appearingText != null)
        {
            appearingText.ShowText(text); // Use the ShowText method to display the text with the typewriter effect
        }
        
        else
        {
            Debug.LogError("AppearingText component is not found on the textbox GameObject.");
            
            // Fallback to direct text assignment if AppearingText component is missing
            var textMesh = textboxObj.GetComponentInChildren<TextMeshProUGUI>();
            if (textMesh != null)
            {
                textMesh.text = text;
            }
        }
    }

    private Button CreateChoiceView(string text)
    {
        Button button = Instantiate(buttonPrefab, canvas.transform, false);
        var choiceText = button.GetComponentInChildren<TextMeshProUGUI>();
        choiceText.text = text;
        return button;
    }

    private void RemoveChildren()
    {
        for (var i = canvas.transform.childCount - 1; i >= 0; --i)
        {
            Destroy(canvas.transform.GetChild(i).gameObject);
        }
    }
}