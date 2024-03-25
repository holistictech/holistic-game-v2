using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class nBackSelectBefore : MonoBehaviour
{
    private int showingBlockCount = 2;
    private List<Button> generatedBlocks = new List<Button>();
    private List<string> enteredBlocks = new List<string>();
    private int currentIndex = 0;
    public TextMeshProUGUI countdownDisplay;
    public GameObject blocks;
    public GameObject imageDigitCirclePrefab;
    public GameObject Digitcircle;
    private bool isLevelCompleted = false;

    public Button blockPrefab;

    public int countdowmTime;

    private int initialCountdownTime = 0;

    public Slider progressBar;
    private int succesCount = 0;

    private int failCount = 0;

    public GameObject succedDialog;
    public TextMeshProUGUI levelTime;
    private int levelTimeCountdown;

    public bool isBackward;
    private int startingBlockCount = 1;
    private List<string> shownIndices = new List<string>();
    private List<string> tempShownIndices = new List<string>();
    private bool isCheckingInput = false;
    public Button deleteButton;
    public Button checkAnswerButton;

    public float blocksWidth = 800f;
    public float blocksHeight = 1200f;
    public float buttonWidth = 150f;
    public float buttonHeight = 150f;
    public float distanceBetweenButtons = 10f;

    private List<Sprite> usedImages = new List<Sprite>();

    void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator CountdownToStart()
    {
        blocks.SetActive(false);
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
        StartCoroutine(LoadBlocks());
        StartCoroutine(LevelTimeStart());

    }

    private IEnumerator LoadBlocks()
    {
        changeOptionsInteraction(false);
        yield return new WaitForSeconds(1);

        blocks.SetActive(true);

        // Load all images from the "Images" folder
        Sprite[] allImages = Resources.LoadAll<Sprite>("Images");

        // Shuffle the array of images
        ShuffleArray(allImages);


        for (int i = 0; i < showingBlockCount; i++)
        {
            float randomX;
            float randomY;
            bool isOverlap = false;
            float spacing = 50.0f;

            Sprite randomImage;
            ShuffleButtonPositions();
            // Get a unique image that has not been used before
            do
            {
                randomImage = allImages[Random.Range(0, allImages.Length)];
            } while (usedImages.Contains(randomImage));

            usedImages.Add(randomImage);

            do
            {
                randomX = Random.Range(-blocksWidth / 2 + buttonWidth / 2 + spacing, blocksWidth / 2 - buttonWidth / 2 - spacing);
                randomY = Random.Range(-blocksHeight / 2 + buttonHeight / 2 + spacing, blocksHeight / 2 - buttonHeight / 2 - spacing);
                isOverlap = generatedBlocks.Any(block => CheckOverlap(randomX, randomY, buttonWidth, buttonHeight, block.transform.localPosition.x, block.transform.localPosition.y, buttonWidth, buttonHeight));
            } while (isOverlap);

            Button block = Instantiate(blockPrefab, blocks.transform);

            // Set the image on the button
            changeOptionsInteraction(false);

            block.image.sprite = randomImage;

            block.transform.localPosition = new Vector3(randomX, randomY, 0);

            block.name = randomImage.name;

            // Store the button in a local variable before using it in lambda
            Button currentButton = block;

            // Add the button to the generatedBlocks list
            generatedBlocks.Add(currentButton);

            blocks.SetActive(true);

            Debug.Log("Loaded image name: " + currentButton.name);
            currentButton.onClick.AddListener(() => ChoosedBlocks(currentButton));

            yield return new WaitForSeconds(1); // Introduce a 1-second delay between button creations
        }

        Button lastItem = generatedBlocks[generatedBlocks.Count - 2];
        shownIndices.Add(lastItem.name);
        tempShownIndices = checkBackward();
        StartCoroutine(ReadyToStart());
        // StartCoroutine(StartStep(startingBlockCount));
    }

    private void ShuffleButtonPositions()
    {
        for (int i = 0; i < generatedBlocks.Count; i++)
        {
            float randomX;
            float randomY;
            bool isOverlap;
            float spacing = 50.0f;

            do
            {
                randomX = Random.Range(-blocksWidth / 2 + buttonWidth / 2 + spacing, blocksWidth / 2 - buttonWidth / 2 - spacing);
                randomY = Random.Range(-blocksHeight / 2 + buttonHeight / 2 + spacing, blocksHeight / 2 - buttonHeight / 2 - spacing);
                isOverlap = generatedBlocks.Any(block => CheckOverlap(randomX, randomY, buttonWidth, buttonHeight, block.transform.localPosition.x, block.transform.localPosition.y, buttonWidth, buttonHeight));
            } while (isOverlap);

            // Update the position of the button in the list
            generatedBlocks[i].transform.localPosition = new Vector3(randomX, randomY, 0);
        }
    }

    void ShuffleArray<T>(T[] array)
    {
        // Function to shuffle an array
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }


    private bool CheckOverlap(float x1, float y1, float width1, float height1, float x2, float y2, float width2, float height2)
    {
        return (x1 < x2 + width2 &&
                x1 + width1 > x2 &&
                y1 < y2 + height2 &&
                y1 + height1 > y2);
    }



    private IEnumerator StartStep(int showingBlockCount)
    {

        isCheckingInput = false;
        Button[] allButtons = blocks.GetComponentsInChildren<Button>();
        // CreateElementsInDigitCircle(showingBlockCount);
        changeOptionsInteraction(false);
        for (int i = 0; i < showingBlockCount; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, allButtons.Length);
            } while (shownIndices.Contains(randomIndex.ToString()));

            shownIndices.Add(randomIndex.ToString());

            Button currentButton = allButtons[randomIndex];
            Color originalColor = currentButton.GetComponent<Image>().color;

            currentButton.GetComponent<Image>().color = Color.green;

            // Save the index or do something with it
            Debug.Log("Showing button at index: " + randomIndex);
            GameObject newImage = Digitcircle.transform.GetChild(i).gameObject;
            ChangeImageColor(newImage, Color.green);
            yield return new WaitForSeconds(1.0f);

            currentButton.GetComponent<Image>().color = originalColor;
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

    public void ChoosedBlocks(Button block)
    {
        if (isCheckingInput == true)
        {
            enteredBlocks.Add(block.name);
            blocks.SetActive(false);
            currentIndex++;
            ChangeButtonColor(block, Color.green);
            bool isSuccess = IsListEqual(enteredBlocks, tempShownIndices);
            checkSuccesRate(isSuccess);
        }
    }




    private void ChangeButtonColor(Button button, Color color)
    {
        button.GetComponent<Image>().color = color;
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

    private void checkSuccesRate(bool isSucces)
    {
        isLevelCompleted = true;
        TextMeshProUGUI succedDialogText = succedDialog.GetComponent<TextMeshProUGUI>();
        if (isSucces)
        {
            succesCount++;
            succedDialogText.text = "Bravo :)";
            succedDialog.SetActive(true);
            showingBlockCount = 1;

            if (succesCount == 4)
            {
                // progressBar.value = 0;
                // // startingBlockCount++;
                // showingBlockCount = 1;

            }

        }
        else
        {
            generatedBlocks.Clear();
            failCount++;
            succedDialogText.text = "Tekrar Deneyelim :(";
            showingBlockCount = 2;
            succedDialog.SetActive(true);
            if (failCount == 4)
            {
                // startingBlockCount--;
            }
            blocks.SetActive(false);

        }
        // Calculate success rate
        float successRate = (float)succesCount / (succesCount + failCount);

        // Set progressBar.value based on success rate
        progressBar.value = successRate;
        GameStartRules(isSucces);
        StartCoroutine(HideSuccessDialog(2.0f));
    }

    IEnumerator HideSuccessDialog(float delay)
    {
        yield return new WaitForSeconds(delay);
        succedDialog.SetActive(false);
        blocks.SetActive(false);

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

    private void GameStartRules(bool isSuccess)
    {
        makeButtonsVisible(false);

        Button[] allButtons = blocks.GetComponentsInChildren<Button>();

        if (isSuccess)
        {
            foreach (Button button in allButtons)
            {
                button.GetComponent<Image>().color = Color.white;
                button.interactable = true;
            }
        }
        else
        {
            List<Button> buttonsToDestroy = new List<Button>();
            usedImages.Clear();
            foreach (Button button in allButtons)
            {
                buttonsToDestroy.Add(button);
            }

            foreach (Button button in buttonsToDestroy)
            {
                Destroy(button.gameObject);
            }
        }

        shownIndices.Clear();
        tempShownIndices.Clear();
        enteredBlocks.Clear();
        currentIndex = 0;
    }


    private IEnumerator ReadyToStart()
    {
        countdownDisplay.gameObject.SetActive(true);
        countdownDisplay.text = "Sıra Sende";
        yield return new WaitForSeconds(1);
        changeOptionsInteraction(true);
        countdownDisplay.gameObject.SetActive(false);
        blocks.SetActive(true);
        isCheckingInput = true;

        currentIndex = 0;
        isLevelCompleted = false;
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

    private List<string> checkBackward()
    {
        if (isBackward)
        {
            List<string> reversedList = new List<string>(shownIndices);
            reversedList.Reverse();
            return reversedList;
        }

        return shownIndices;
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

}
