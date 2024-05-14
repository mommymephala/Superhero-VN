using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class StoryManager : MonoBehaviour
{
    [SerializeField] private TextAsset inkJsonAsset;
    [SerializeField] private CRTController crtController;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private UIManager uiManager;

    private Story _story;
    private bool _uiLocked;

    private void Awake()
    {
        InitializeStory();
    }

    private void InitializeStory()
    {
        _story = new Story(inkJsonAsset.text);
        uiManager.ClearUI();
        RefreshView();
    }

    private void RefreshView()
    {
        uiManager.ClearUI();
        var fullText = "";

        while (_story.canContinue)
        {
            var text = _story.Continue().Trim();
            fullText += text + (_story.canContinue ? "\n" : "");
            HandleTags(_story.currentTags);
        }
        
        uiManager.CreateChoiceButtons(_story.currentChoices, OnClickChoiceButton);
        uiManager.UpdateText(fullText);
    }

    private void HandleTags(List<string> tags)
    {
        foreach (var tag in tags)
        {
            var parts = tag.Split(':');
            switch (parts[0].Trim())
            {
                case "play_sound":
                    soundManager.PlaySound(parts[1].Trim());
                    break;
                case "change_image":
                    uiManager.TransitionImage(parts[1].Trim());
                    break;
                case "change_preset":
                    crtController?.ChangeCRTSetting(int.Parse(parts[1].Trim()));
                    break;
                case "lerp_preset":
                    crtController?.LerpToNextPreset();
                    break;
                case "zoom":
                    ApplyZoom(parts[1].Trim());
                    break;
                case "hide_textbox":
                    uiManager.SetActiveStateForTextbox(false);
                    break;
                case "activate_textbox":
                    uiManager.SetActiveStateForTextbox(true);
                    break;
                default:
                    Debug.LogError("Unhandled tag: " + parts[0].Trim());
                    break;
            }
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
            crtController?.SetZoom(startZoom, endZoom, duration);
        }
    }

    private void OnClickChoiceButton(Choice choice)
    {
        if (!_uiLocked)
        {
            soundManager.PlayButtonClickSound();
            _story.ChooseChoiceIndex(choice.index);
            RefreshView();
        }
    }
}