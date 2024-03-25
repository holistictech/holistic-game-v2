using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class ForwardSpawnImages : MonoBehaviour
{
    private Sprite[] allImages;
    private int showingImageCount = 4;

    private List<Sprite> generatedImages = new List<Sprite>();
    private List<string> enteredOptions = new List<string>();
    public GameObject showingImageObject;
    private int currentIndex = 0;
    public TextMeshProUGUI countdownDisplay;
    public GameObject options;
    public GameObject imageDigitCirclePrefab;
    public GameObject Digitcircle;
    private bool isLevelCompleted = false;

    public Button optionPrefab;

    public int countdowmTime;

    public Slider progressBar;
    private int succesCount = 0;

    private int failCount = 0;

    public GameObject succedDialog;
    public TextMeshProUGUI levelTime;
    private int levelTimeCountdown;

    public bool isBackward;

    public Button deleteButton;
    public Button repeatButton;
    public Button checkAnswerButton;

    private bool isCheckingInput = false;
    private bool isRepeat = false;
    private bool isRepeated = false;
    private bool showRepeat = true;

    private void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    private void Update()
    {
        checkAnswerButton.interactable = (generatedImages.Count == enteredOptions.Count);
        repeatButton.gameObject.SetActive(progressBar.value == 0.75f && failCount == 0 && showRepeat);
    }

    public void Repeat()
    {
        isRepeat = true;
        showRepeat = false;
        levelTime.gameObject.SetActive(false);
        CheckSuccessRate(false);
    }

    private IEnumerator LoadShowingImages()
    {
        isCheckingInput = false;
        allImages = Resources.LoadAll<Sprite>("Images");
        yield return new WaitForSeconds(1);
        CreateElementsInDigitCircle(showingImageCount);

        if (allImages != null && allImages.Length > 0)
        {
            while (generatedImages.Count < showingImageCount)
            {
                Sprite randomImage;
                do
                {
                    randomImage = allImages[Random.Range(0, allImages.Length)];
                } while (generatedImages.Contains(randomImage));

                generatedImages.Add(randomImage);
                Debug.Log("Loaded image name: " + generatedImages.Count + ": " + randomImage.name);
                yield return null;
            }

            while (currentIndex < generatedImages.Count)
            {
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.green);
                showingImageObject.GetComponent<Image>().sprite = generatedImages[currentIndex];
                currentIndex++;

                showingImageObject.SetActive(true);
                yield return new WaitForSeconds(0.5f);
                showingImageObject.SetActive(false);
                yield return new WaitForSeconds(1.0f);
            }

            showingImageObject.SetActive(false);
            checkBackward();
            StartCoroutine(ReadyToStart());
        }
        else
        {
            Debug.LogError("No images found in the 'Images' folder in Resources.");
        }
    }

    private IEnumerator ReadyToStart()
    {
        isCheckingInput = true;
        countdownDisplay.gameObject.SetActive(true);
        ResetDigitCircleColor();

        if (isBackward)
        {
            StartCoroutine(activateRotation());
            yield return new WaitForSeconds(1.5f);
        }

        countdownDisplay.text = "Sıra Sende..";
        yield return new WaitForSeconds(1.0f);
        countdownDisplay.gameObject.SetActive(false);
        options.SetActive(true);

        CreateOptions(generatedImages.Count);
        currentIndex = 0;
        isLevelCompleted = false;
        StartCoroutine(LevelTimeStart());
    }

     private void ResetDigitCircleColor()
    {
        GameObject[] circles = Digitcircle.GetComponentsInChildren<Transform>(true)
            .Where(t => t != Digitcircle.transform)
            .Select(t => t.gameObject)
            .ToArray();
        foreach (GameObject circle in circles)
        {
            ChangeImageColor(circle, Color.white);
        }
    }

    private void CreateElementsInDigitCircle(int numberOfElements)
    {
        for (int i = 0; i < numberOfElements; i++)
        {
            GameObject newImage = Instantiate(imageDigitCirclePrefab, Digitcircle.transform);

            newImage.transform.localPosition = new Vector3(i * 2, 0, 0);

            newImage.name = "ImageElement" + i;
        }
    }

    private void CreateOptions(int numberOfElements)
    {
        Debug.Log("Number of elements: " + numberOfElements);
        numberOfElements = Mathf.Min(numberOfElements * 2, 9);

        if (showingImageCount >= 4)
        {
            numberOfElements = 9;
        }

        Debug.Log("New Number of elements: " + numberOfElements);
        GridLayoutGroup optionsGridLayoutGroup = options.GetComponentInChildren<GridLayoutGroup>();

        if (numberOfElements <= 4)
        {
            optionsGridLayoutGroup.cellSize = new Vector2(200, 200);
            optionsGridLayoutGroup.spacing = new Vector2(100, 100);
        }
        if (numberOfElements >= 6)
        {
            optionsGridLayoutGroup.spacing = new Vector2(50, 50);
        }
        if (numberOfElements >= 8)
        {
            optionsGridLayoutGroup.cellSize = new Vector2(150, 150);
        }

        Sprite[] generatedOptionImages = SetOptions(numberOfElements);
        Debug.Log("Generated option image number: " + generatedOptionImages.Length);

        for (int i = 0; i < numberOfElements; i++)
        {
            Button option = Instantiate(optionPrefab, options.transform);
            option.transform.localPosition = new Vector3(i * 2, 0, 0);
            option.GetComponent<Image>().sprite = generatedOptionImages[i];
            option.name = generatedOptionImages[i].name;
            option.onClick.AddListener(() => EnteredOption(option.name));
        }
    }

    private Sprite[] SetOptions(int numberOfElements)
    {
        List<Sprite> generatedOptionImages = new List<Sprite>(generatedImages);
        int nonAnswerImages = numberOfElements - generatedImages.Count;

        while (nonAnswerImages > 0)
        {
            Sprite randomImage;
            do
            {
                randomImage = allImages[Random.Range(0, allImages.Length)];
            } while (generatedOptionImages.Contains(randomImage));

            generatedOptionImages.Add(randomImage);
            nonAnswerImages--;
        }

        ShuffleList(generatedOptionImages);
        return generatedOptionImages.ToArray();
    }

    private void ShuffleList(List<Sprite> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Sprite temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public void EnteredOption(string optionName)
    {
        if (isCheckingInput)
        {
            if (optionName == "Delete")
            {
                currentIndex = (currentIndex == 0) ? currentIndex : currentIndex - 1;
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;

                newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.white);
                enteredOptions.RemoveAt(currentIndex);
                changeOptionsInteraction(true);
            }
            else if (optionName == "CheckAnswer")
            {
                options.SetActive(false);
                makeButtonsVisible(false);
                List<string> generatedImagesName = generatedImages.Select(option => option.name).ToList();
                bool isSuccess = IsListEqual(enteredOptions, generatedImagesName);
                CheckSuccessRate(isSuccess);
            }
            else
            {
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                enteredOptions.Add(optionName);
                currentIndex++;
                ChangeImageColor(newImage, Color.green);

                if (currentIndex == generatedImages.Count)
                {
                    changeOptionsInteraction(false);
                }
            }
        }
    }

    private void changeOptionsInteraction(bool isInteractable)
    {
        Button[] allOptions = options.GetComponentsInChildren<Button>();

        foreach (Button option in allOptions)
        {
            option.interactable = isInteractable;
        }
    }

    bool IsListEqual(List<string> list1, List<string> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }

        for (int i = 0; i < list1.Count; i++)
        {
            if (list1[i] != list2[i])
            {
                return false;
            }
        }
        return true;
    }

    private void CheckSuccessRate(bool isSucces)
    {
        levelTime.gameObject.SetActive(false);
        TextMeshProUGUI succedDialogText = succedDialog.GetComponent<TextMeshProUGUI>();

        if (progressBar.value == 0.75f && failCount == 0 && isRepeat)
        {
            failCount++;
            succedDialogText.text = "Bir şans daha";
            succedDialog.SetActive(true);
            isRepeat = false;
        } else
        {
            if (isSucces)
            {
                if (failCount == 0 || isRepeated) succesCount++;
                succedDialogText.text = "Bravo :)";
                succedDialog.SetActive(true);
                if (failCount == 0 || isRepeated) progressBar.value += 0.25f;

                if (failCount == 1) {
                isRepeated = true;
                }

                if (isRepeated)
                {
                isRepeated = false;
                failCount = 0;
                }

                if (succesCount == 4)
                {
                    progressBar.value = 0;
                    succesCount = 0;
                    failCount = 0;
                    showingImageCount++;
                    showRepeat = true;
                }
            }
            else
            {
                succesCount = 0;
                failCount++;
                succedDialogText.text = "Tekrar Deneyelim :(";
                progressBar.value = 0;
                succedDialog.SetActive(true);
                if (failCount == 4 && showingImageCount > 2)
                {
                    failCount = 0;
                    showingImageCount--;
                    showRepeat = true;
                }
            }
        }

        isLevelCompleted = true;
        options.SetActive(false);
        GameStartRules(isRepeat);
        StartCoroutine(HideSuccessDialog(2.0f));
    }

    private void GameStartRules(bool isRepeat)
    {
        makeButtonsVisible(false);

        foreach (Transform child in options.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in Digitcircle.transform)
        {
            Destroy(child.gameObject);
        }

        if(!isRepeat) generatedImages.Clear();

        enteredOptions.Clear();
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
        makeButtonsVisible(true);

        levelTimeCountdown = (showingImageCount * 3) + 2;
        levelTime.text = levelTimeCountdown.ToString();
        levelTime.gameObject.SetActive(true);
        while (levelTimeCountdown > 0)
        {
            levelTime.text = levelTimeCountdown.ToString();
            yield return new WaitForSeconds(1);
            levelTimeCountdown--;

            if (isLevelCompleted)
            {
                yield break;
            }
        }
        levelTime.gameObject.SetActive(false);
        countdownDisplay.gameObject.SetActive(true);
        options.SetActive(false);
        makeButtonsVisible(false);

        countdownDisplay.text = "Süre Doldu...";
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);

        List<string> generatedImagesName = generatedImages.Select(option => option.name).ToList();
        bool isSuccess = IsListEqual(enteredOptions, generatedImagesName);
        CheckSuccessRate(isSuccess);
    }

    private IEnumerator CountdownToStart()
    {
        options.SetActive(false);
        countdownDisplay.gameObject.SetActive(true);

        countdownDisplay.text = isBackward ? "BackwardSpanImage'a Hoşgeldiniz!" : "ForwardSpanAudioImage'a Hoşgeldiniz!";
        yield return new WaitForSeconds(1.0f);
        countdownDisplay.text = "Başlayalım...";
        yield return new WaitForSeconds(1.0f);

        countdownDisplay.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);
        StartCoroutine(LoadShowingImages());
    }

    private void ChangeImageColor(GameObject imageObject, Color color)
    {
        Image imageComponent = imageObject.GetComponent<Image>();

        if (imageComponent != null)
        {
            imageComponent.color = color;
        }
    }
    private void checkBackward()
    {
        if (isBackward)
        {
            generatedImages.Reverse();
        }
    }

    private IEnumerator activateRotation()
    {
        float rotationDuration = 1.0f;
        float rotationTime = 0.0f;
        Quaternion startRotation = Digitcircle.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, startRotation.eulerAngles.z + 180.0f);

        while (rotationTime < rotationDuration)
        {
            rotationTime += Time.deltaTime;
            Digitcircle.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationTime / rotationDuration);
            yield return null;
        }

        Digitcircle.transform.rotation = startRotation;
    }

    private void makeButtonsVisible(bool isVisible)
    {
        deleteButton.gameObject.SetActive(isVisible);
        checkAnswerButton.gameObject.SetActive(isVisible);
    }
}
