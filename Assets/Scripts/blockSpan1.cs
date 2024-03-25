using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class blockSpan1 : MonoBehaviour
{
    private int showingBlockCount = 2;
    private Sprite[] allImages;
    private List<Button> generatedBlocks = new List<Button>();
    private List<Tuple<int, Color>> enteredBlocks = new List<Tuple<int, Color>>();
    private List<Tuple<int, string>> enteredImageBlocks = new List<Tuple<int, string>>();
    private List<Sprite> generatedImages = new List<Sprite>();

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
    private int startingBlockCount = 2;
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

    private IEnumerator StartStep(int startingBlockCount)
    {
        isCheckingInput = false;
        Button[] allButtons = blocks.GetComponentsInChildren<Button>();
        CreateElementsInDigitCircle(startingBlockCount);

        Color[] colors = { Color.red, Color.blue, Color.green };
        Color randomColor = (level <= 3) ? colors[2] : colors[UnityEngine.Random.Range(0, colors.Length)];

        List<int> blockIndexes = CreateNeighborBlocks();
        allImages = Resources.LoadAll<Sprite>("Images");

        if ((level >= 6 && level <= 8) || (level >= 13 && level <= 17))
        {
             for (int j = 0; j < showingBlockCount; j++)
            {
                Sprite randomImage;
                do
                {
                randomImage = allImages[UnityEngine.Random.Range(0, allImages.Length)];
                } while (generatedImages.Contains(randomImage));

                generatedImages.Add(randomImage);
            }
        }

        List<Coroutine> colorChangeCoroutines = new List<Coroutine>();

        for (int i = 0; i < blockIndexes.Count; i++)
        {
            int randomIndex = blockIndexes[i];

            if ((level >= 6 && level <= 8) || (level >= 13 && level <= 17))
            {
                shownImageIndices.Add(new Tuple<int, string>(randomIndex, generatedImages[i].name));
            }
            else {
                shownIndices.Add(new Tuple<int, Color>(randomIndex, randomColor));
            }

            Button currentButton = allButtons[randomIndex];

            if ((level >= 6 && level <= 8) || (level >= 13 && level <= 17))
            {
                ChangeButtonImage(currentButton, generatedImages[i]);
            }
            else
            {
                ChangeButtonColor(currentButton, randomColor);
            }

            Color originalColor = currentButton.GetComponent<Image>().color;

            Coroutine digitCircleColorChangeCoroutine = StartCoroutine(ChangeColorDigitCircle(currentButton, Color.green, originalColor, i));

            colorChangeCoroutines.Add(digitCircleColorChangeCoroutine);
        }

        Coroutine resetColor = StartCoroutine(ResetBlocksColor());

        foreach (Coroutine coroutine in colorChangeCoroutines)
        {
            yield return coroutine;
        }

        tempShownIndices = checkBackward();
        StartCoroutine(ReadyToStart());
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

    private List<int> CreateNeighborBlocks()
    {
        List<int> result = new List<int>();

        if (level == 1)
        {
            int firstRandomNumber;
            int secondRandomNumber;

            do
            {
                firstRandomNumber = UnityEngine.Random.Range(0, 4);
                secondRandomNumber = UnityEngine.Random.Range(0, 4);

            } while (firstRandomNumber == secondRandomNumber);

            result.Add(firstRandomNumber);
            result.Add(secondRandomNumber);
        }
        if (level == 2)
        {
            int firstRandomNumber;
            int secondRandomNumber;

            do
            {
                firstRandomNumber = UnityEngine.Random.Range(0, 6);
                secondRandomNumber = UnityEngine.Random.Range(0, 6);

            } while (firstRandomNumber == secondRandomNumber);

            result.Add(firstRandomNumber);
            result.Add(secondRandomNumber);
        }
        else if (level == 3 || level == 6)
        {
            int randomIndex = UnityEngine.Random.Range(0, 8);
            result.Add(randomIndex);

            int secondIndex = GetNeighborIndex(randomIndex);
            result.Add(secondIndex);

            int thirdIndex = GetNeighborIndex(secondIndex);
            result.Add(thirdIndex);
        }
        else if (level == 4 || level == 7)
        {
            int randomIndex = UnityEngine.Random.Range(0, 8);
            result.Add(randomIndex);

            int secondIndex = GetNeighborIndex(randomIndex);
            result.Add(secondIndex);

            int thirdIndex = GetSeparateIndex(result);
            result.Add(thirdIndex);
        }
        else if (level == 5 || level == 8)
        {
            int randomIndex = UnityEngine.Random.Range(0, 8);
            result.Add(randomIndex);

            int secondIndex = GetSeparateIndex(result);
            result.Add(secondIndex);

            int thirdIndex = GetSeparateIndex(result);
            result.Add(thirdIndex);
        }

        return result;
    }

    private int GetNeighborIndex(int currentIndex)
    {
        int[] possibleNeighbors = GetPossibleNeighbors(currentIndex);
        return possibleNeighbors[UnityEngine.Random.Range(0, possibleNeighbors.Length)];
    }

    private int GetSeparateIndex(List<int> existingIndices)
    {
        int[] possibleIndices = Enumerable.Range(0, 8).Except(existingIndices).ToArray();
        return possibleIndices[UnityEngine.Random.Range(0, possibleIndices.Length)];
    }

    private int[] GetPossibleNeighbors(int currentIndex)
    {
        int row = currentIndex / 3;
        int col = currentIndex % 3;

        List<int> neighbors = new List<int>();

        if (row > 0)
            neighbors.Add(currentIndex - 3);

        if (row < 2)
            neighbors.Add(currentIndex + 3);

        if (col > 0)
            neighbors.Add(currentIndex - 1);

        if (col < 2)
            neighbors.Add(currentIndex + 1);

        return neighbors.ToArray();
    }

    private void ChangeImageColor(GameObject imageObject, Color color)
    {
        Image imageComponent = imageObject.GetComponent<Image>();

        if (imageComponent != null)
        {
            imageComponent.color = color;
        }
    }

    public void ChoosedBlocks(Button block)
    {
        if (isCheckingInput == true)
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
                        button.interactable = false;
                    }
                }

                ChangeButtonColor(blocks.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name == enteredBlocks[currentIndex].Item1.ToString()), Color.white);
                enteredBlocks.RemoveAt(currentIndex);
            }
            else if (block.name == "CheckAnswer")
            {
                blocks.SetActive(false);
                bool isSuccess;

                if ((level >= 6 && level <= 8) || (level >= 13 && level <= 17))
                {
                    isSuccess = DoesImageExistInLists(enteredImageBlocks, shownImageIndices);
                }
                else
                {
                    isSuccess = DoesItemExistInLists(enteredBlocks, tempShownIndices);
                }

                CheckSuccessRate(isSuccess);
                makeButtonsVisible(false);
            }
            else
            {
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.green);
                Color chosenColor;
                Sprite chosenImage;

                if (level == 1 || level == 2 || level == 3 || level == 4 || level == 5)
                {
                    chosenColor = Color.green;
                } else
                {
                    chosenColor = block.GetComponent<Image>().color;
                }

                chosenImage = block.GetComponent<Image>().sprite;

                if ((level >= 6 && level <= 8) || (level >= 13 && level <= 17))
                {
                    enteredImageBlocks.Add(new Tuple<int, string>(Int32.Parse(block.name), chosenImage.name));
                }
                else
                {
                    enteredBlocks.Add(new Tuple<int, Color>(Int32.Parse(block.name), chosenColor));
                }

                currentIndex++;
                ChangeButtonColor(block, chosenColor);
                block.interactable = false;

                if (currentIndex == tempShownIndices.Count)
                {
                    changeOptionsInteraction(false);
                }

            }
        }
    }

    private void ChangeButtonColor(Button button, Color color)
    {
        button.GetComponent<Image>().color = color;
    }

    private void ChangeButtonImage(Button button, Sprite image)
    {
        button.GetComponent<Image>().sprite = image;
    }

    private bool DoesItemExistInLists(List<Tuple<int, Color>> list1, List<Tuple<int, Color>> list2)
    {
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
        foreach (var item in list1)
        {
            if (!list2.Contains(item))
            {
                return false;
            }
        }

        return true;
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
        } else
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
        if(!isRepeat) shownIndices.Clear();
        enteredBlocks.Clear();
        currentIndex = 0;
    }

    private IEnumerator ReadyToStart()
    {
        blocks.SetActive(false);
        countdownDisplay.gameObject.SetActive(true);

        ResetDigitCircleColor();

        if (isBackward)
        {
            StartCoroutine(ActivateRotation());
            yield return new WaitForSeconds(1.5f);
        }

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
        if (level == 3 || level == 4 || level == 5) ColorElementOptions();
        else if (level == 6 || level == 7 || level == 8) ImageElementOptions();
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
        countdownDisplay.text = "Süre Doldu...";
        yield return new WaitForSeconds(1);
        blocks.SetActive(false);
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);

        CheckSuccessRate(false);
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
        float rotationDuration = 1.5f;
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

    private void ColorElementOptions()
    {
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
        for (int i = 0; i < 3; i++)
        {
            Button option = Instantiate(optionPrefab, elementOptions.transform);
            option.transform.localPosition = new Vector3(i * 2, 0, 0);
            ChangeButtonImage(option, generatedImages[i]);
            option.name = i.ToString();
            option.onClick.AddListener(() => EnteredOption(option));
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
                startingBlockCount = 2;
                break;

            case 2:
                showingBlockCount = 6;
                blocksGridLayoutGroup.padding.left = 50;
                blocksGridLayoutGroup.padding.right = 50;
                blocksGridLayoutGroup.cellSize = new Vector2(200, 200);
                startingBlockCount = 2;
                break;

            case 3: // komşu ve renk soruları
            case 4:
            case 5:
            case 6: // resim soruları
            case 7:
            case 8:
                showingBlockCount = 9;
                blocksGridLayoutGroup.padding.left = 50;
                blocksGridLayoutGroup.padding.right = 50;
                blocksGridLayoutGroup.cellSize = new Vector2(200, 200);
                startingBlockCount = 3;
                break;

            case 9: // komşu ve renk soruları
            case 10:
            case 11:
            case 12:
            case 13: // resim soruları
            case 14:
            case 15:
            case 16:
            case 17:
                showingBlockCount = 12;
                blocksGridLayoutGroup.padding.left = 25;
                blocksGridLayoutGroup.padding.right = 25;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                startingBlockCount = 4;
                break;

            case 18:
                showingBlockCount = 16;
                blocksGridLayoutGroup.padding.left = 25;
                blocksGridLayoutGroup.padding.right = 25;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                startingBlockCount = 5;
                break;

            case 19:
                showingBlockCount = 20;
                blocksGridLayoutGroup.padding.left = 5;
                blocksGridLayoutGroup.padding.right = 5;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                startingBlockCount = 6;
                break;

            case 20:
                showingBlockCount = 25;
                blocksGridLayoutGroup.padding.left = 5;
                blocksGridLayoutGroup.padding.right = 5;
                blocksGridLayoutGroup.cellSize = new Vector2(150, 150);
                startingBlockCount = 7;
                break;

            default:
                break;
        }
    }
}
