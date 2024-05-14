using System.Collections;
using UnityEngine;

public class AppearingText : MonoBehaviour
{
    [SerializeField] private float timePerCharacter = 0.1f;
    private TMPro.TextMeshProUGUI textMesh;
    private string fullText;
    private float timer;

    private void Awake()
    {
        textMesh = GetComponent<TMPro.TextMeshProUGUI>();
    }

    public void ShowText(string text)
    {
        if (textMesh != null)
        {
            fullText = text;
            textMesh.text = "";
            timer = 0;
            StartCoroutine(TypeText());
        }
    }


    private IEnumerator TypeText()
    {
        int totalLength = fullText.Length;
        int currentLength = 0;

        while (currentLength < totalLength)
        {
            timer += Time.deltaTime;
            if (timer > timePerCharacter)
            {
                timer -= timePerCharacter;
                currentLength++;
                textMesh.text = fullText.Substring(0, currentLength);
            }
            yield return null;
        }
    }
}