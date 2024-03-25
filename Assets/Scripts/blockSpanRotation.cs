using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class blockSpanRotation : MonoBehaviour
{
    private int showingBlockCount = 1;
    private Sprite[] allImages;
    private List<Tuple<int, Color>> enteredBlocks = new List<Tuple<int, Color>>();
    private List<Tuple<int, string>> enteredImageBlocks = new List<Tuple<int, string>>();
    private List<Sprite> generatedImages = new List<Sprite>();
    private List<Button> generatedBlocks = new List<Button>();

    private int currentIndex = 0;
    public TextMeshProUGUI countdownDisplay;
    public GameObject blocks;
    public GameObject imageDigitCirclePrefab;
    public GameObject Digitcircle;
    private bool isLevelCompleted = false;

    public Button blockPrefab;
    public Button repeatButton;

    public int countdownTime;

    private int initialCountdownTime = 0;

    public Slider progressBar;

    private int succesCount = 0;
    private int failCount = 0;

    public GameObject succedDialog;
    public TextMeshProUGUI levelTime;
    private int levelTimeCountdown;

    public bool isBackward;
    private bool isRepeat;
    private int startingBlockCount = 1;
    private List<Tuple<int, Color>> shownIndices = new List<Tuple<int, Color>>();
    private List<Tuple<int, string>> shownImageIndices = new List<Tuple<int, string>>();
    private List<Tuple<int, Color>> tempShownIndices = new List<Tuple<int, Color>>();
    private bool isCheckingInput = false;
    public Button deleteButton;
    public Button checkAnswerButton;
    private int level = 1;
    public GameObject elementOptions;
    public Button optionPrefab;
    private Button selectedButton;

    private Vector3 originalScale;
    private bool isColorPhase = false;
    private bool isImagePhase = false;
    private Quaternion startRotation;

    void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    void Update()
    {
        repeatButton.gameObject.SetActive(progressBar.value == 0.75f && failCount == 0);
    }

    public void Repeat()
    {
        isRepeat = true;
        CheckSuccessRate(false);
    }

    private IEnumerator CountdownToStart()
    {
        blocks.SetActive(false);
        initialCountdownTime = countdownTime;
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
        StartCoroutine(LoadBlocks());
    }

    private IEnumerator LoadBlocks()
    {
        CheckLevel();

        for (int i = 0; i < showingBlockCount; i++)
        {
            Button block = Instantiate(blockPrefab, blocks.transform);
            block.transform.localPosition = new Vector3(i * 2, 0, 0);
            block.name = i.ToString();
            block.onClick.AddListener(() => ChoosedBlocks(block));
        }
        blocks.transform.rotation = startRotation;
        blocks.SetActive(true);
        yield return new WaitForSeconds(1f);
        StartCoroutine(StartStep(startingBlockCount));
    }

    private IEnumerator ChangeColorDigitCircle(Button button, Color color, Color originalColor, int digitCircleIndex)
    {
        GameObject newImage = Digitcircle.transform.GetChild(digitCircleIndex).gameObject;
        ChangeImageColor(newImage, color);

        yield return new WaitForSeconds(1.0f);

        button.GetComponent<Image>().color = originalColor;
        ChangeImageColor(newImage, originalColor);
    }

    private IEnumerator StartStep(int showingBlockCount)
    {
        isCheckingInput = false;
        Button[] allButtons = blocks.GetComponentsInChildren<Button>();
        Color[] colors = { Color.red, Color.blue, Color.green };
        Color randomColor = (!isColorPhase) ? Color.green : colors[UnityEngine.Random.Range(0, colors.Length)];
        if (isColorPhase)
        {
            ColorElementOptions();
        }
        if (isImagePhase)
        {
            allImages = Resources.LoadAll<Sprite>("Images");
            Sprite randomImage = allImages[UnityEngine.Random.Range(0, allImages.Length)];
            for (int j = 0; j < showingBlockCount; j++)
            {
                generatedImages.Add(randomImage);
            }
            ImageElementOptions();
        }
        Dictionary<Button, Color> originalColors = new Dictionary<Button, Color>();
        CreateElementsInDigitCircle(showingBlockCount);

        // Store original colors
        foreach (Button button in allButtons)
        {
            originalColors.Add(button, button.GetComponent<Image>().color);
        }

        for (int i = 0; i < showingBlockCount; i++)
        {
            int randomIndex;

            if (isImagePhase)
            {
                do
                {
                    randomIndex = UnityEngine.Random.Range(0, allButtons.Length);
                } while (shownImageIndices.Any(tuple => tuple.Item1 == randomIndex));
                shownImageIndices.Add(new Tuple<int, string>(randomIndex, generatedImages[i].name));
            }
            else
            {
                do
                {
                    randomIndex = UnityEngine.Random.Range(0, allButtons.Length);
                } while (shownIndices.Any(tuple => tuple.Item1 == randomIndex));

                shownIndices.Add(new Tuple<int, Color>(randomIndex, randomColor));

            }

            Button currentButton = allButtons[randomIndex];
            GameObject newImage = Digitcircle.transform.GetChild(i).gameObject;
            ChangeImageColor(newImage, Color.green);
            if (isImagePhase)
            {
                ChangeButtonImage(currentButton, generatedImages[i]);
            }
            else if (isColorPhase)
            {
                ChangeButtonColor(currentButton, randomColor);
            }
            else
            {
                currentButton.GetComponent<Image>().color = randomColor;
            }
        }

        yield return new WaitForSeconds(1.0f);

        // Reset all button colors
        foreach (Button button in allButtons)
        {
            button.GetComponent<Image>().color = originalColors[button];
            button.GetComponent<Image>().sprite = null;
        }

        tempShownIndices = checkBackward();
        StartCoroutine(ReadyToStart());
    }
    private void ChangeImageColor(GameObject imageObject, Color color)
    {
        Image imageComponent = imageObject.GetComponent<Image>();

        if (imageComponent != null)
        {
            imageComponent.color = color;
        }
    }

    private IEnumerator ResetBlocksColor()
    {
        yield return new WaitForSeconds(1.0f);
        Button[] allButtons = blocks.GetComponentsInChildren<Button>();
        foreach (Button button in allButtons)
        {
            ChangeButtonColor(button, Color.white);
        }
    }


    public void ChoosedBlocks(Button block)
    {
        if ((isCheckingInput == true && isColorPhase == false && isImagePhase == false) || (isCheckingInput == true && isColorPhase == true && selectedButton.GetComponent<Image>()) || (isCheckingInput == true && isImagePhase == true && selectedButton.GetComponent<Image>()))
        {

            if (block.name == "Delete")
            {
                currentIndex = (currentIndex == 0) ? currentIndex : currentIndex - 1;
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.white);
                changeOptionsInteraction(true);
                if (currentIndex > 0)
                {
                    for (int i = 0; i < currentIndex - 1; i++)
                    {
                        Button button = blocks.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name == enteredBlocks[i].Item1.ToString());
                        // Button button = blocks.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name == enteredBlocks[i]);
                        button.interactable = false;
                    }
                }

                // ChangeButtonColor(blocks.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name == enteredBlocks[currentIndex]), Color.white);
                ChangeButtonColor(blocks.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name == enteredBlocks[currentIndex].Item1.ToString()), Color.white);
                if (isImagePhase)
                {
                    enteredImageBlocks.RemoveAt(currentIndex);
                    ChangeButtonImage(blocks.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name == enteredBlocks[currentIndex].Item1.ToString()), null);
                }
                enteredBlocks.RemoveAt(currentIndex);
            }
            else if (block.name == "CheckAnswer")
            {

                blocks.SetActive(false);

                bool isSuccess = false;
                makeButtonsVisible(false);
                if (isImagePhase)
                {
                    isSuccess = DoesImageExistInLists(enteredImageBlocks, shownImageIndices);
                }
                else
                {
                    isSuccess = DoesItemExistInLists(enteredBlocks, tempShownIndices);
                }
                CheckSuccessRate(isSuccess);

            }
            else
            {
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.green);
                currentIndex++;
                Sprite chosenImage;
                Color chosenColor;

                if (isImagePhase)
                {
                    chosenImage = selectedButton.GetComponent<Image>().sprite;
                    ChangeButtonImage(block, chosenImage);
                    enteredImageBlocks.Add(new Tuple<int, string>(Int32.Parse(block.name), chosenImage.name));
                    enteredBlocks.Add(new Tuple<int, Color>(Int32.Parse(block.name), Color.white));

                }
                else if (isColorPhase)
                {
                    ChangeButtonColor(block, selectedButton.GetComponent<Image>().color);
                    chosenColor = selectedButton.GetComponent<Image>().color;
                    enteredBlocks.Add(new Tuple<int, Color>(Int32.Parse(block.name), chosenColor));

                }
                else
                {
                    ChangeButtonColor(block, Color.green);
                    enteredBlocks.Add(new Tuple<int, Color>(Int32.Parse(block.name), Color.green));

                }
                block.interactable = false;
                if (currentIndex == tempShownIndices.Count)
                {
                    changeOptionsInteraction(false);
                }

            }
        }
    }

    private bool DoesItemExistInLists(List<Tuple<int, Color>> list1, List<Tuple<int, Color>> list2)
    {

        if (list1.Count != list2.Count)
        {
            return false;
        }

        foreach (var item in list1)
        {
            if (!list2.Contains(item))
            {
                return false;
            }
        }

        return true;
    }

    private bool DoesImageExistInLists(List<Tuple<int, string>> list1, List<Tuple<int, string>> list2)
    {

        if (list1.Count != list2.Count)
        {
            return false;
        }
        foreach (var item in list1)
        {
            if (!list2.Contains(item))
            {
                return false;
            }
        }

        return true;
    }
    bool IsListEqual(List<string> list1, List<string> list2)
    {

        if (list1.Count != list2.Count)
        {
            return false;
        }

        foreach (var item in list1)
        {
            if (!list2.Contains(item))
            {
                return false;
            }
        }

        return true;
    }
    private void ChangeButtonColor(Button button, Color color)
    {
        button.GetComponent<Image>().color = color;
    }

    private void ChangeButtonImage(Button button, Sprite image)
    {
        button.GetComponent<Image>().sprite = image;
    }


    private void CheckSuccessRate(bool isSuccess)
    {
        isLevelCompleted = true;
        levelTime.gameObject.SetActive(false);
        TextMeshProUGUI succedDialogText = succedDialog.GetComponent<TextMeshProUGUI>();

        if (progressBar.value == 0.75f && failCount == 0 && isRepeat)
        {
            failCount++;
            succedDialogText.text = "Bir şans daha";
            succedDialog.SetActive(true);
            isRepeat = false;
        }
        else
        {
            if (isSuccess)
            {
                failCount = 0;
                succesCount++;
                succedDialogText.text = "Bravo :)";
                succedDialog.SetActive(true);
                progressBar.value += 0.25f;
                if (succesCount == 4)
                {
                    level++;
                    progressBar.value = 0;
                    startingBlockCount++;
                    succesCount = 0;
                }
            }
            else
            {
                succesCount = 0;
                failCount++;
                succedDialogText.text = "Tekrar Deneyelim :(";
                progressBar.value = 0;
                succedDialog.SetActive(true);
                if (failCount == 4 && startingBlockCount > 2)
                {
                    failCount = 0;
                    level--;
                    startingBlockCount--;
                }
            }
        }

        blocks.SetActive(false);
        GameStartRules(isRepeat);
        StartCoroutine(HideSuccessDialog(2.0f, startingBlockCount));
    }

    IEnumerator HideSuccessDialog(float delay, int startingBlockCount)
    {
        yield return new WaitForSeconds(delay);
        succedDialog.SetActive(false);
        blocks.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(LoadBlocks());
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

    private void GameStartRules(bool isRepeat)
    {
        foreach (Transform child in Digitcircle.transform)
        {
            Destroy(child.gameObject);
        }
        makeButtonsVisible(false);

        foreach (Transform button in blocks.transform)
        {
            Destroy(button.gameObject);
        }
        foreach (Transform button in elementOptions.transform)
        {
            Destroy(button.gameObject);
        }
        if (!isRepeat) 
        {
            shownIndices.Clear();
            shownImageIndices.Clear();
            tempShownIndices.Clear();
        }
        enteredBlocks.Clear();
        generatedImages.Clear();
        enteredImageBlocks.Clear();
        currentIndex = 0;
    }

    private IEnumerator ReadyToStart()
    {
        blocks.SetActive(false);

        ResetDigitCircleColor();
        StartCoroutine(ActivateRotation());
        yield return new WaitForSeconds(1.5f);
        countdownDisplay.gameObject.SetActive(true);
        countdownDisplay.text = "Sıra sende...";
        yield return new WaitForSeconds(1.5f);
        countdownDisplay.gameObject.SetActive(false);
        isCheckingInput = true;
        blocks.SetActive(true);

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

    private IEnumerator LevelTimeStart()
    {
        makeButtonsVisible(true);

        levelTimeCountdown = (startingBlockCount * 3) + 2;
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
        blocks.SetActive(false);
        makeButtonsVisible(false);
        countdownDisplay.text = "Süre Doldu...";
        countdownDisplay.gameObject.SetActive(false);
        bool isSuccess = false;
        if (isImagePhase)
        {
            isSuccess = DoesImageExistInLists(enteredImageBlocks, shownImageIndices);
        }
        else
        {
            isSuccess = DoesItemExistInLists(enteredBlocks, tempShownIndices);
        }
        // bool isSuccess = IsListEqual(enteredBlocks, tempShownIndices);
        CheckSuccessRate(isSuccess);
    }

    private List<Tuple<int, Color>> checkBackward()
    {
        if (isBackward)
        {
            List<Tuple<int, Color>> reversedList = new List<Tuple<int, Color>>(shownIndices);
            reversedList.Reverse();
            return reversedList;
        }

        return new List<Tuple<int, Color>>(shownIndices);
    }

    private void makeButtonsVisible(bool isVisible)
    {
        deleteButton.gameObject.SetActive(isVisible);
        checkAnswerButton.gameObject.SetActive(isVisible);
    }

    private void changeOptionsInteraction(bool isInteractable)
    {
        Button[] allOptions = blocks.GetComponentsInChildren<Button>();

        foreach (Button option in allOptions)
        {
            option.interactable = isInteractable;
        }
    }

    private IEnumerator ActivateRotation()
{
    blocks.SetActive(true);
    float rotationDuration = 1.5f;
    float rotationTime = 0.0f;

    // Blokların rotasyonunu ayarla
    Quaternion startRotation = blocks.transform.rotation;
    Quaternion targetRotation = Quaternion.Euler(0, 0, startRotation.eulerAngles.z - 90.0f);

    // Option'ların rotasyonlarını saklamak için bir liste oluştur
    List<Transform> optionTransforms = new List<Transform>();

    // Option'ların rotasyonlarını kaydet
    foreach (Transform optionTransform in blocks.transform)
    {
        optionTransforms.Add(optionTransform);
    }

    while (rotationTime < rotationDuration)
    {
        rotationTime += Time.deltaTime;

        // Blokların rotasyonunu güncelle
        blocks.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, rotationTime / rotationDuration);

        yield return null;
    }

    // Option'ların rotasyonlarını sabit tut
    foreach (Transform optionTransform in optionTransforms)
    {
        optionTransform.rotation = Quaternion.identity;
    }
}


    private void ColorElementOptions()
    {
        elementOptions.SetActive(true);
        Debug.Log("ColorElementOptions called");
        Color[] buttonColors = { Color.blue, Color.red, Color.green };

        for (int i = 0; i < 3; i++)
        {
            Button option = Instantiate(optionPrefab, elementOptions.transform);
            option.transform.localPosition = new Vector3(i * 2, 0, 0);
            option.name = i.ToString();
            int colorIndex = i % buttonColors.Length;
            ChangeButtonColor(option, buttonColors[colorIndex]);
            option.onClick.AddListener(() => EnteredOption(option));
        }
    }

    private void ImageElementOptions()
    {
        List<Sprite> generatedOptionImages = new List<Sprite>();
        generatedOptionImages.Add(generatedImages[0]); int nonAnswerImages = 2;

        while (nonAnswerImages > 0)
        {
            Sprite randomImage;
            do
            {
                randomImage = allImages[UnityEngine.Random.Range(0, allImages.Length)];
            } while (generatedOptionImages.Contains(randomImage));

            generatedOptionImages.Add(randomImage);
            nonAnswerImages--;
        }
        Shuffle(generatedOptionImages);
        for (int i = 0; i < 3; i++)
        {
            Button option = Instantiate(optionPrefab, elementOptions.transform);
            option.transform.localPosition = new Vector3(i * 2, 0, 0);
            ChangeButtonImage(option, generatedOptionImages[i]);
            option.name = i.ToString();
            option.onClick.AddListener(() => EnteredOption(option));
        }
    }
    private void Shuffle(List<Sprite> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Sprite temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
    private void EnteredOption(Button clickedOption)
    {
        if (selectedButton != null)
        {
            ChangeButtonScale(selectedButton, originalScale);
        }

        selectedButton = clickedOption;
        originalScale = selectedButton.transform.localScale;
        ChangeButtonScale(selectedButton, originalScale * 1.2f);
    }

    private void ChangeButtonScale(Button button, Vector3 scale)
    {
        button.transform.localScale = scale;
    }

    private void CheckLevel()
    {
        GridLayoutGroup blocksGridLayoutGroup = blocks.GetComponentInChildren<GridLayoutGroup>();
        Debug.Log("Current Level: " + level);

        switch (level)
        {
            case 1:
                showingBlockCount = 4;
                blocksGridLayoutGroup.padding.left = 150;
                blocksGridLayoutGroup.padding.right = 150;
                blocksGridLayoutGroup.cellSize = new Vector2(200, 200);
                startingBlockCount = 1;
                break;

            case 2:
                showingBlockCount = 6;
                blocksGridLayoutGroup.padding.left = 50;
                blocksGridLayoutGroup.padding.right = 50;
                blocksGridLayoutGroup.cellSize = new Vector2(200, 200);
                startingBlockCount = 2;
                break;

            case 3:
                showingBlockCount = 9;
                blocksGridLayoutGroup.padding.left = 50;
                blocksGridLayoutGroup.padding.right = 50;
                blocksGridLayoutGroup.cellSize = new Vector2(200, 200);
                startingBlockCount = 3;
                isColorPhase = false;
                isImagePhase = false;
                break;

            case 4:
                showingBlockCount = 9;
                blocksGridLayoutGroup.padding.left = 50;
                blocksGridLayoutGroup.padding.right = 50;
                blocksGridLayoutGroup.cellSize = new Vector2(200, 200);
                isColorPhase = true;
                isImagePhase = false;
                startingBlockCount = 3;
                break;

            case 5:
                showingBlockCount = 9;
                blocksGridLayoutGroup.padding.left = 50;
                blocksGridLayoutGroup.padding.right = 50;
                blocksGridLayoutGroup.cellSize = new Vector2(200, 200);
                isColorPhase = false;
                isImagePhase = true;
                startingBlockCount = 3;
                break;

            case 6:
                showingBlockCount = 12;
                blocksGridLayoutGroup.padding.left = 25;
                blocksGridLayoutGroup.padding.right = 25;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                isColorPhase = false;
                isImagePhase = false;
                startingBlockCount = 4;
                break;
            case 7:
                showingBlockCount = 12;
                blocksGridLayoutGroup.padding.left = 25;
                blocksGridLayoutGroup.padding.right = 25;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                isColorPhase = true;
                isImagePhase = false;
                startingBlockCount = 4;
                break;
            case 8:
                showingBlockCount = 12;
                blocksGridLayoutGroup.padding.left = 25;
                blocksGridLayoutGroup.padding.right = 25;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                isColorPhase = false;
                isImagePhase = true;
                startingBlockCount = 4;
                break;
            case 9:
                showingBlockCount = 16;
                blocksGridLayoutGroup.padding.left = 25;
                blocksGridLayoutGroup.padding.right = 25;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                isColorPhase = false;
                isImagePhase = false;
                startingBlockCount = 5;
                break;
            case 10:
                showingBlockCount = 16;
                blocksGridLayoutGroup.padding.left = 25;
                blocksGridLayoutGroup.padding.right = 25;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                isColorPhase = true;
                isImagePhase = false;
                startingBlockCount = 5;
                break;
            case 11:
                showingBlockCount = 16;
                blocksGridLayoutGroup.padding.left = 25;
                blocksGridLayoutGroup.padding.right = 25;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                isColorPhase = false;
                isImagePhase = true;
                startingBlockCount = 5;
                break;

            case 12:
                showingBlockCount = 20;
                blocksGridLayoutGroup.padding.left = 5;
                blocksGridLayoutGroup.padding.right = 5;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                startingBlockCount = 6;
                isColorPhase = false;
                isImagePhase = false;
                break;
            case 13:
                showingBlockCount = 20;
                blocksGridLayoutGroup.padding.left = 5;
                blocksGridLayoutGroup.padding.right = 5;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                startingBlockCount = 6;
                isColorPhase = true;
                isImagePhase = false;
                break;
            case 14:
                showingBlockCount = 20;
                blocksGridLayoutGroup.padding.left = 5;
                blocksGridLayoutGroup.padding.right = 5;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                startingBlockCount = 6;
                isColorPhase = false;
                isImagePhase = true;
                break;

            case 15:
                showingBlockCount = 25;
                blocksGridLayoutGroup.padding.left = 5;
                blocksGridLayoutGroup.padding.right = 5;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                isColorPhase = false;
                isImagePhase = false;
                startingBlockCount = 7;
                break;
            case 16:
                showingBlockCount = 25;
                blocksGridLayoutGroup.padding.left = 5;
                blocksGridLayoutGroup.padding.right = 5;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                isColorPhase = true;
                isImagePhase = false;
                startingBlockCount = 7;
                break;
            case 17:
                showingBlockCount = 25;
                blocksGridLayoutGroup.padding.left = 5;
                blocksGridLayoutGroup.padding.right = 5;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                isColorPhase = false;
                isImagePhase = true;
                startingBlockCount = 7;
                break;

            default:
                break;
        }
    }
}
