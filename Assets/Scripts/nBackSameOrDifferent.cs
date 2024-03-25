using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class nBackSameOrDifferent : MonoBehaviour
{
    private Sprite[] allImages;
    private int showingImageCount = 2;

    private List<Sprite> generatedImages = new List<Sprite>();
    private List<Sprite> showingImages = new List<Sprite>();
    private List<string> enteredOptions = new List<string>();
    public GameObject showingImageObject;
    private int currentIndex = 0;
    public TextMeshProUGUI countdownDisplay;
    private bool isLevelCompleted = false;
    public int countdowmTime;
    private int initialCountdownTime = 0;
    public Slider progressBar;
    private int succesCount = 0;
    private int failCount = 0;
    public GameObject succedDialog;
    public TextMeshProUGUI levelTime;
    private int levelTimeCountdown;
    private bool isCheckingInput = false;
    public GameObject options;
    private bool isSame = true;
    private int consecutiveSameCount = 0;


    private void Start()
    {
        // StartCoroutine(LevelTimeStart());
        StartCoroutine(CountdownToStart());

    }

    private void Update()
    {
        // Add any update logic you need here.
    }

    private IEnumerator LoadShowingImages()
    {
        isCheckingInput = false;
        allImages = Resources.LoadAll<Sprite>("Images");
        yield return new WaitForSeconds(1);
        isSame = IsSameOrDifferent();

        if (allImages != null && allImages.Length > 0)
        {
            for (int i = 0; i < showingImageCount; i++)
            {
                Sprite randomImage;
                if (isSame && generatedImages.Count > 0)
                {
                    randomImage = generatedImages[generatedImages.Count - 1];
                }
                else
                {

                    do
                    {
                        randomImage = allImages[Random.Range(0, allImages.Length)];
                    } while (generatedImages.Contains(randomImage));
                }

                generatedImages.Add(randomImage);
                showingImages.Add(randomImage);
                Debug.Log("Loaded image name: " + generatedImages.Count + ": " + randomImage.name);
                yield return null;
            }
            while (currentIndex < showingImages.Count)
            {

                showingImageObject.GetComponent<Image>().sprite = showingImages[currentIndex];
                currentIndex++;

                showingImageObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                showingImageObject.SetActive(false);
                yield return new WaitForSeconds(1);

            }
            showingImageObject.SetActive(false);
            StartCoroutine(ReadyToStart());
        }
        else
        {
            Debug.LogError("No images found in the 'Images' folder in Resources.");
        }
    }

    private bool IsSameOrDifferent()
    {
        if (consecutiveSameCount < 3)
        {
            consecutiveSameCount++;
            return Random.Range(0, 2) == 0;  // Return 'true' to indicate it's the same for the first three occurrences.
        }
        else
        {
            consecutiveSameCount = 0;  // Reset the counter after three consecutive occurrences.
            isSame = !isSame;  // Toggle the value for the fourth occurrence and onwards.
            return isSame;
        }
    }


    private IEnumerator ReadyToStart()
    {
        isCheckingInput = true;
        countdownDisplay.gameObject.SetActive(true);
        countdownDisplay.text = "Hazır Ol...";
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);
        options.SetActive(true);

        currentIndex = 0;
        isLevelCompleted = false;
    }


    public void EnteredOption(string optionName)
    {
        if (isCheckingInput)
        {
            if (optionName == "Aynı")
            {
                bool isSuccess = isSame ? true : false;
                checkSuccesRate(isSuccess);
            }
            if (optionName == "Farklı")
            {
                bool isSuccess = !isSame ? true : false;
                checkSuccesRate(isSuccess);
            }
        }
    }
    private void checkSuccesRate(bool isSucces)
    {
        TextMeshProUGUI succedDialogText = succedDialog.GetComponent<TextMeshProUGUI>();
        if (isSucces)
        {
            succesCount++;
            succedDialogText.text = "Bravo :)";
            succedDialog.SetActive(true);

            showingImageCount = 1;
            GameStartRules(true);
        }
        else
        {
            failCount++;
            succedDialogText.text = "Tekrar Deneyelim :(";
            progressBar.value = 0;
            succedDialog.SetActive(true);
            showingImageCount = 2;
            GameStartRules(false);
        }
        // Calculate success rate
        float successRate = (float)succesCount / (succesCount + failCount);

        // Set progressBar.value based on success rate
        progressBar.value = successRate;
        isLevelCompleted = true;
        options.SetActive(false);
        StartCoroutine(HideSuccessDialog(2.0f));
    }

    private void GameStartRules(bool isSucces)
    {
        if (!isSucces)
        {
            generatedImages.Clear();
        }
        enteredOptions.Clear();
        showingImages.Clear();
        currentIndex = 0;
    }

    IEnumerator HideSuccessDialog(float delay)
    {
        yield return new WaitForSeconds(delay);
        succedDialog.SetActive(false);
        StartCoroutine(LoadShowingImages());
    }
    private IEnumerator LevelTimeStart()
    {

        levelTimeCountdown = 90;
        levelTime.text = levelTimeCountdown.ToString();
        levelTime.gameObject.SetActive(true);
        while (levelTimeCountdown > 0)
        {
            levelTime.text = levelTimeCountdown.ToString();
            yield return new WaitForSeconds(1);
            levelTimeCountdown--;
        }
    }
    private IEnumerator CountdownToStart()
    {
        options.SetActive(false);
        initialCountdownTime = countdowmTime;
        countdownDisplay.gameObject.SetActive(true);
        while (initialCountdownTime > 0)
        {
            countdownDisplay.text = initialCountdownTime.ToString();
            yield return new WaitForSeconds(1);
            initialCountdownTime--;
        }

        countdownDisplay.text = "Başlayalım...";
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);
        StartCoroutine(LoadShowingImages());
        StartCoroutine(LevelTimeStart());

    }

}
