using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Random = UnityEngine.Random;

public class corsiSpan : MonoBehaviour
{
    private int showingBlockCount = 9;
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
    private int startingBlockCount = 2;
    private List<string> shownIndices = new List<string>();
    private List<string> tempShownIndices = new List<string>();
    private bool isCheckingInput = false;
    public Button deleteButton;
    public Button checkAnswerButton;
    public Button repeatButton;
    public bool isMoving;
    public float moveInterval = 1f;
    private float moveForce = 10f;

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
        CheckSuccessRate(false);
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
        if (isMoving)
        {
            foreach (Transform child in blocks.transform)
            {
                Destroy(child.gameObject);
            }
            StartCoroutine(LoadBlocks());
        }
        else
        {
            blocks.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            StartCoroutine(StartStep(startingBlockCount));
        }
    }

    private IEnumerator LoadBlocks()
    {
        yield return new WaitForSeconds(1);


        float spacing = isMoving ? 50.0f : 0.0f;
        float blocksWidth = 800f;
        float blocksHeight = 1200f;
        float buttonWidth = 150f;
        float buttonHeight = 150f;
        float distanceBetweenButtons = 10f;

        for (int i = 0; i < showingBlockCount; i++)
        {
            float randomX = 0;
            float randomY = 0;
            bool isOverlap;

            if (isMoving)
            {
                do
                {
                    randomX = Random.Range(-blocksWidth / 2 + buttonWidth / 2 + spacing, blocksWidth / 2 - buttonWidth / 2 - spacing);
                    randomY = Random.Range(-blocksHeight / 2 + buttonHeight / 2 + spacing, blocksHeight / 2 - buttonHeight / 2 - spacing);

                    isOverlap = generatedBlocks.Any(block => CheckOverlap(randomX, randomY, buttonWidth, buttonHeight, block.transform.localPosition.x, block.transform.localPosition.y, buttonWidth, buttonHeight));
                } while (isOverlap);
            }

            Button block = Instantiate(blockPrefab, blocks.transform);

            if (isMoving)
            {
                TextMeshProUGUI buttonText = block.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = i.ToString();
            }
            block.transform.localPosition = new Vector3(randomX, randomY, 0);

            block.name = i.ToString();
            generatedBlocks.Add(block);
            if (isMoving)
            {
                StartCoroutine(MoveBlocksContinuously());
            }
            Debug.Log("Loaded image name: " + block.name);
            block.onClick.AddListener(() => ChoosedBlocks(block));


            yield return null;
        }

        blocks.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(StartStep(startingBlockCount));
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
        CreateElementsInDigitCircle(showingBlockCount);

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
                        Button button = blocks.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name == enteredBlocks[i]);
                        button.interactable = false;
                    }
                }

                ChangeButtonColor(blocks.GetComponentsInChildren<Button>().FirstOrDefault(b => b.name == enteredBlocks[currentIndex]), Color.white);
                enteredBlocks.RemoveAt(currentIndex);
            }
            else if (block.name == "CheckAnswer")
            {

                blocks.SetActive(false);
                bool isSuccess = IsListEqual(enteredBlocks, tempShownIndices);
                CheckSuccessRate(isSuccess);
                makeButtonsVisible(false);

            }
            else
            {
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.green);
                enteredBlocks.Add(block.name);
                currentIndex++;
                ChangeButtonColor(block, Color.green);
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
        isLevelCompleted = true;
        levelTime.gameObject.SetActive(false);
        TextMeshProUGUI succedDialogText = succedDialog.GetComponent<TextMeshProUGUI>();
        if (isSucces)
        {
            failCount = 0;
            succesCount++;
            succedDialogText.text = "Bravo :)";
            succedDialog.SetActive(true);
            progressBar.value += 0.25f;
            if (succesCount == 4)
            {
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
                startingBlockCount--;
            }
        }
        blocks.SetActive(false);
        GameStartRules();
        StartCoroutine(HideSuccessDialog(2.0f, startingBlockCount));
    }

    IEnumerator HideSuccessDialog(float delay, int startingBlockCount)
    {
        yield return new WaitForSeconds(delay);
        succedDialog.SetActive(false);
        blocks.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        if (isMoving)
        {
            StartCoroutine(LoadBlocks());
        }
        else
        {
            StartCoroutine(StartStep(startingBlockCount));
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

    private void GameStartRules()
    {
        foreach (Transform child in Digitcircle.transform)
        {
            Destroy(child.gameObject);
        }
        makeButtonsVisible(false);


        if (isMoving)
        {
            foreach (Transform child in blocks.transform)
            {
                Destroy(child.gameObject);
            }
            generatedBlocks.Clear();
        }
        else
        {
            Button[] allButtons = blocks.GetComponentsInChildren<Button>();
            foreach (Button button in allButtons)
            {
                button.GetComponent<Image>().color = Color.white;
                button.interactable = true;
            }
        }
        shownIndices.Clear();
        enteredBlocks.Clear();
        currentIndex = 0;
    }

    private IEnumerator ReadyToStart()
    {
        countdownDisplay.gameObject.SetActive(true);

        ResetDigitCircleColor();

        if (isBackward)
        {
            StartCoroutine(ActivateRotation());
            yield return new WaitForSeconds(1.5f);
        }
        countdownDisplay.text = "Hazır Ol...";
        yield return new WaitForSeconds(1);
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
                yield break; // Coroutine'i sonlandır
            }
        }
        levelTime.gameObject.SetActive(false);
        blocks.SetActive(false);
        makeButtonsVisible(false);
        countdownDisplay.gameObject.SetActive(true);
        countdownDisplay.text = "Süre Doldu...";
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);

        bool isSuccess = IsListEqual(enteredBlocks, tempShownIndices);
        CheckSuccessRate(isSuccess);
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

        IEnumerator MoveBlocksContinuously()
    {
        while (true) // Sonsuz döngü, sürekli hareket için
        {
            MoveBlocksRandomly();
            yield return null; // Coroutine'in bir sonraki frame'de devam etmesine izin ver
        }
    }

    void MoveBlocksRandomly()
    {
        List<Vector2> blockPositions = new List<Vector2>();

        foreach (Transform child in blocks.transform)
        {
            blockPositions.Add(child.position);
        }

        foreach (Transform child in blocks.transform)
        {
            Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
            CircleCollider2D circleCollider = child.GetComponent<CircleCollider2D>();

            if (rb != null && circleCollider != null)
            {
                Vector2 forceDirection = CalculateAverageDirection(child.position, blockPositions);
                Vector2 randomForce = forceDirection * (moveForce);

                // Check for collisions with other blocks
                AvoidBlockCollisions(child, circleCollider, blockPositions);

                // Check for collisions with other objects
                AvoidObjectCollisions(child, circleCollider, blockPositions);

                rb.velocity = randomForce;
            }
        }
    }

    void AvoidObjectCollisions(Transform currentBlock, CircleCollider2D currentCollider, List<Vector2> blockPositions)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(currentBlock.position, currentCollider.radius * 2f);

        foreach (Collider2D collider in colliders)
        {
            if (collider.transform != currentBlock)
            {
                // Change direction if colliding with other objects
                Vector2 avoidDirection = ((Vector2)currentBlock.position - (Vector2)collider.transform.position).normalized;
                currentBlock.GetComponent<Rigidbody2D>().velocity = avoidDirection * (moveForce * 2.0f);
            }
        }
    }

    void AvoidBlockCollisions(Transform currentBlock, CircleCollider2D currentCollider, List<Vector2> blockPositions)
    {
        foreach (Vector2 position in blockPositions)
        {
            if (position != (Vector2)currentBlock.position)
            {
                float distance = Vector2.Distance(currentBlock.position, position);
                float minDistance = currentCollider.radius + currentCollider.radius; // İki çember arasındaki minimum mesafe

                if (distance < minDistance)
                {
                    // Eğer bloklar birbirine çok yaklaştıysa, farklı bir yöne doğru hareket etsin
                    Vector2 avoidDirection = ((Vector2)currentBlock.position - position).normalized;
                    currentBlock.GetComponent<Rigidbody2D>().velocity = avoidDirection * (moveForce * 2.0f); // Farklı bir yöne doğru hızlandırma
                }
            }
        }
    }
    Vector2 CalculateAverageDirection(Vector3 currentPosition, List<Vector2> blockPositions)
    {
        Vector2 averageDirection = Vector2.zero;

        foreach (Vector2 position in blockPositions)
        {
            averageDirection += ((Vector2)currentPosition - position).normalized;
        }

        averageDirection /= blockPositions.Count;

        return averageDirection.normalized;
    }

    void ClampBlockPosition(Transform block, CircleCollider2D circleCollider)
    {
        // Blokların içinde bulunduğu game object'in boyutları
        RectTransform blocksRectTransform = blocks.GetComponent<RectTransform>();
        float blocksWidth = blocksRectTransform.rect.width;
        float blocksHeight = blocksRectTransform.rect.height;

        // Sınırları belirle
        float minX = blocks.transform.position.x - blocksWidth / 2f + circleCollider.radius;
        float maxX = blocks.transform.position.x + blocksWidth / 2f - circleCollider.radius;
        float minY = blocks.transform.position.y - blocksHeight / 2f + circleCollider.radius;
        float maxY = blocks.transform.position.y + blocksHeight / 2f - circleCollider.radius;

        // Düğmenin pozisyonunu sınırla
        block.position = new Vector2(
            Mathf.Clamp(block.position.x, minX + circleCollider.radius, maxX - circleCollider.radius),
            Mathf.Clamp(block.position.y, minY + circleCollider.radius, maxY - circleCollider.radius)
        );
    }


}
