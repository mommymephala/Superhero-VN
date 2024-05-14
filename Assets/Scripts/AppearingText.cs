using System.Collections;
using UnityEngine;

public class AppearingText : MonoBehaviour
{
    [SerializeField] private float timePerCharacter = 0.1f;
    private TMPro.TextMeshProUGUI textMesh;
    public string fullText;
    private float timer;
    public Coroutine typingCoroutine;

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
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
            typingCoroutine = StartCoroutine(TypeText());
        }
    }

    private IEnumerator TypeText()
    {
        int totalLength = fullText.Length;
        int currentLength = 0;

        while (currentLength < totalLength)
        {
            if (Input.GetMouseButtonDown(0))
            {
                textMesh.text = fullText;
                yield break;
            }

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