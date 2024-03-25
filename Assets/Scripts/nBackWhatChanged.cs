using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class nBackWhatChanged : MonoBehaviour
{
    private Sprite[] allImages;
    private int showingImageCount = 2;
    private List<Sprite> generatedImages = new List<Sprite>();
    private List<Sprite> showingImages = new List<Sprite>();
    private List<string> enteredOptions = new List<string>();
    public GameObject showingImageObject;
    public GameObject cloneImages;
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
    private bool isColor = true;
    private bool isCount = false;
    private Color newColor = Color.white;
    public bool isThreeOption;
    private bool multiple = false;

    private void Start()
    {
        // StartCoroutine(LevelTimeStart());
        if (isThreeOption)
        {
            multiple = Random.Range(0, 2) == 0;
        }
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

        if (isThreeOption)
        {
            SetRandomCondition();
        }
        else
        {
            isColor = IsSameOrDifferent();
        }
        if (allImages != null && allImages.Length > 0)
        {
            for (int i = 0; i < showingImageCount; i++)
            {
                Sprite randomImage;
                if ((isColor || isCount) && generatedImages.Count > 0)
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
                newColor = isColor ? new Color(Random.value, Random.value, Random.value) : newColor;
                ChangeImageColor(cloneImages, newColor);
                cloneImages.GetComponent<Image>().sprite = showingImages[currentIndex];
                ChangeImageColor(showingImageObject, newColor);
                showingImageObject.GetComponent<Image>().sprite = showingImages[currentIndex];
                if (isCount)
                {
                    multiple = !multiple;

                }
                if (isThreeOption)
                {
                    if (multiple)
                    {

                        cloneImages.SetActive(true);
                    }
                    else
                    {
                        cloneImages.SetActive(false);
                    }
                }
                showingImageObject.SetActive(true);
                currentIndex++;

                yield return new WaitForSeconds(1);
            }
            cloneImages.SetActive(false);
            showingImageObject.SetActive(false);
            StartCoroutine(ReadyToStart());
        }
        else
        {
            Debug.LogError("No images found in the 'Images' folder in Resources.");
        }
    }



    private void ChangeImageColor(GameObject imageObject, Color color)
    {
        Image imageComponent = imageObject.GetComponent<Image>();

        if (imageComponent != null)
        {
            imageComponent.color = color;
        }
    }


    private bool IsSameOrDifferent()
    {
        return Random.Range(0, 2) == 0;
    }

    private void SetRandomCondition()
    {
        int randomValue = Random.Range(0, 3);
        switch (randomValue)
        {
            case 0:
                isCount = true;
                isColor = false;
                break;
            case 1:
                isColor = true;
                isCount = false;
                break;
            case 2:
                isColor = false;
                isCount = false;
                break;

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
            if (optionName == "Renk")
            {
                bool isSuccess = isColor ? true : false;
                checkSuccesRate(isSuccess);
            }
            if (optionName == "Şekil")
            {
                bool isSuccess = (!isColor && !isCount) ? true : false;
                checkSuccesRate(isSuccess);
            }
            if (optionName == "Sayı")
            {
                bool isSuccess = isCount ? true : false;
                checkSuccesRate(isSuccess);
            }
        }
    }
    private void checkSuccesRate(bool isSucces)
    {
        levelTime.gameObject.SetActive(false);
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
            if (isLevelCompleted)
            {
                yield break; // Coroutine'i sonlandır
            }
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
