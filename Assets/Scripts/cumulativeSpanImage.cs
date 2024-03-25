using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class CumulativeSpanImages : MonoBehaviour
{
    public Slider progressBar;

    public TextMeshProUGUI levelTime;
    public TextMeshProUGUI countdownDisplay;

    public Button optionPrefab;
    public Button deleteButton;
    public Button checkAnswerButton;

    public GameObject showingImageObject;
    public GameObject successionDialog;
    public GameObject options;
    public GameObject imageDigitCirclePrefab;
    public GameObject Digitcircle;

    public int countdownTime = 1;

    public bool isBackward;


    private List<Sprite> shownImages = new List<Sprite>();
    private List<Sprite> optionImages = new List<Sprite>();
    private List<string> enteredOptions = new List<string>();
    private List<string> shownImagesNames = new List<string>();

    private Sprite[] allImages;

    private bool isLevelCompleted = false;

    private int level = 1;
    private int initialCountdownTime = 0;
    private int currentIndex = 0;
    private int successCount = 0;
    private int failCount = 0;
    private int levelTimeCountdown;


    private void Start() {
        StartCoroutine(ReadyForStart());
    }

    private void Update() {
        checkAnswerButton.interactable = (enteredOptions.Count == shownImages.Count);
    }

    private IEnumerator ReadyForStart() {
        Debug.Log(level);
        progressBar.gameObject.SetActive(false);
        makeButtonsVisible(false);
        initialCountdownTime = countdownTime;
        countdownDisplay.gameObject.SetActive(true);

        var messages = new[] {
            new { Text = !isBackward?  "Kümülatif Forward Span'e hoşgeldiniz!": "Kümülatif Backward Span'e hoşgeldiniz!", Time = 2.5f },
            new { Text = "Önünüze gelecek resimleri kümülatif bir şekilde seçmeniz gerekiyor.", Time = 2.5f },
            new { Text = "Hazırsan başlayalım!", Time = 2.5f },
        };

        foreach (var message in messages) {
            countdownDisplay.text = message.Text;
            yield return new WaitForSeconds(message.Time);
        }

        while (initialCountdownTime > 0) {
            countdownDisplay.text = initialCountdownTime.ToString();
            yield return new WaitForSeconds(1);
            initialCountdownTime--;
        }

        countdownDisplay.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);

        StartCoroutine(ShowImage());
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

    private IEnumerator ShowImage() {
        allImages = Resources.LoadAll<Sprite>("Images");

        if (allImages == null || allImages.Length == 0) {
            Debug.LogError("No images found in the 'Images' folder in Resources.");
            yield return null;
        }

        if (level > 9)
        {
            level = 12;
        }

        CreateElementsInDigitCircle(level);

        yield return new WaitForSeconds(1.0f);

        GameObject circleToHighlight;

        if (level == 4 && successCount == 0) {
            for (int i = 0; i < 4; i++) {
                Sprite newImage = SelectUniqueRandomImage();
                if (newImage == null) {
                    Debug.LogError("No more unique images available.");
                    yield break;
                }

                showingImageObject.GetComponent<Image>().sprite = newImage;
                showingImageObject.SetActive(true);

                circleToHighlight = Digitcircle.transform.GetChild(i).gameObject;
                StartCoroutine(ChangeColorTemporarily(circleToHighlight));

                shownImages.Add(newImage);
                shownImagesNames.Add(newImage.name);
                Debug.Log("Added image: " + newImage.name + " as, " + shownImages.Count + ".");

                yield return new WaitForSeconds(1.5f);
                showingImageObject.SetActive(false);
            }
        } else {
            Sprite newImage = SelectUniqueRandomImage();
            if (newImage == null)
            {
                Debug.LogError("No more unique images available.");
                yield break;
            }

            showingImageObject.GetComponent<Image>().sprite = newImage;
            showingImageObject.SetActive(true);

            circleToHighlight = Digitcircle.transform.GetChild(level-1).gameObject;
            if (circleToHighlight != null) {
                StartCoroutine(ChangeColorTemporarily(circleToHighlight));
            }

            shownImages.Add(newImage);
            shownImagesNames.Add(newImage.name);
            Debug.Log("Added image: " + newImage.name + " as, " + shownImages.Count + ".");

            yield return new WaitForSeconds(0.5f);

            showingImageObject.SetActive(false);
        }

        StartCoroutine(AskQuestionAndStartGame());
    }

    private Sprite SelectUniqueRandomImage() {
        List<Sprite> availableImages = allImages.Except(shownImages).ToList();

        if (availableImages.Count == 0) {
            return null;
        }

        return availableImages[Random.Range(0, availableImages.Count)];
    }

    private IEnumerator AskQuestionAndStartGame() {
        countdownDisplay.gameObject.SetActive(true);
        countdownDisplay.text = (level == 1) ? "Hangi resim gösterildi?" : "Hangi resim eklendi?";

        if (isBackward && level != 1)
        {
            StartCoroutine(ActivateRotation());
        }
        yield return new WaitForSeconds(1.5f);

        countdownDisplay.gameObject.SetActive(false);

        makeButtonsVisible(true);

        CreateOptions();

        currentIndex = 0;

        isLevelCompleted = false;

        StartCoroutine(LevelTimeStart());
    }

    private void CreateElementsInDigitCircle(int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject newImage = Instantiate(imageDigitCirclePrefab, Digitcircle.transform);

            newImage.transform.localPosition = new Vector3(i * 2, 0, 0);

            newImage.name = "ImageElement" + i;
        }
    }

    private void CreateOptions() {
        SetOptions();

        foreach (Transform child in options.transform) {
            Destroy(child.gameObject);
        }

        int optionCount = (level == 12) ? 12 : optionImages.Count;

        int rows = (optionCount == 12) ? 3 : 3;
        int cols = (optionCount == 12) ? 4 : 3;

        for (int i = 0; i < optionCount; i++) {
            Button option = Instantiate(optionPrefab, options.transform);

            int row = i / cols;
            int col = i % cols;
            option.transform.localPosition = new Vector3(col * 2, row * 2, 0);


            option.GetComponent<Image>().sprite = optionImages[i];
            option.name = optionImages[i].name;

            int index = i;
            option.onClick.AddListener(() => EnteredOption(optionImages[index].name));
        }
    }

    private void SetOptions() {
        optionImages.Clear();

        foreach (var shownImage in shownImages) {
            optionImages.Add(shownImage);
        }

        var availableImages = allImages.Except(optionImages).ToList();

        while (optionImages.Count < 9 && availableImages.Count > 0) {
            int randomIndex = Random.Range(0, availableImages.Count);
            Sprite randomImage = availableImages[randomIndex];
            optionImages.Add(randomImage);
            availableImages.RemoveAt(randomIndex);
        }

        ShuffleList(optionImages);
    }

    private void ShuffleList(List<Sprite> list) {
        for (int i = list.Count - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);

            Sprite temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private IEnumerator LevelTimeStart() {
        levelTimeCountdown = (shownImages.Count * 3) + 2;
        levelTime.text = levelTimeCountdown.ToString();
        levelTime.gameObject.SetActive(true);

        while (levelTimeCountdown > 0) {
            levelTime.text = levelTimeCountdown.ToString();
            yield return new WaitForSeconds(1);
            levelTimeCountdown--;

            if (isLevelCompleted) {
                yield break;
            }
        }

        levelTime.gameObject.SetActive(false);

        countdownDisplay.text = "Süre doldu...";
        countdownDisplay.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        countdownDisplay.gameObject.SetActive(false);

        EnteredOption("checkAnswer");
    }

    public void EnteredOption(string optionName) {
        if (optionName == "delete") {
            currentIndex = (currentIndex == 0) ? currentIndex : currentIndex - 1;
            GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;

            ChangeImageColor(newImage, Color.white);
            enteredOptions.RemoveAt(currentIndex);
            changeOptionsInteraction(true);
        } else if (optionName == "checkAnswer") {
            makeButtonsVisible(false);

            bool isSuccess = IsListEqual(enteredOptions, shownImagesNames);
            checkSuccess(isSuccess);
        }
        else {
            GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;

            enteredOptions.Add(optionName);
            currentIndex++;
            ChangeImageColor(newImage, Color.green);

            if (currentIndex == shownImages.Count) {
                changeOptionsInteraction(false);
            }
        }
    }

    private void changeOptionsInteraction(bool isInteractable) {
        Button[] allOptions = options.GetComponentsInChildren<Button>();

        foreach (Button option in allOptions) {
            option.interactable = isInteractable;
        }
    }

    bool IsListEqual(List<string> list1, List<string> list2) {
        if (list1.Count != list2.Count) {
            return false;
        }

        if (isBackward)
        {
            list2.Reverse();
        }

        for (int i = 0; i < list1.Count; i++) {

            if (list1[i] != list2[i]) {
                return false;
            }
        }

        if (isBackward)
        {
            list2.Reverse();
        }

        return true;
    }

    private void checkSuccess(bool isSucces) {
        levelTime.gameObject.SetActive(false);
        TextMeshProUGUI successionDialogText = successionDialog.GetComponent<TextMeshProUGUI>();

        if (isSucces) {
            level++;
            failCount = 0;
            successCount++;

            successionDialogText.text = "Bravo :)";
            successionDialog.SetActive(true);

            progressBar.value += 0.1f;
        } else {
            if (level > 4) {
                progressBar.value = 0.4f;
                level = 4;

                successionDialogText.text = "Bilemediniz, 4. seviyeden devam edelim..";
            }
            else
            {
                level = 1;
                progressBar.value = 0;

                successionDialogText.text = "Bilemediniz, baştan başlayalım..";
            }

            successCount = 0;
            shownImages.Clear();
            shownImagesNames.Clear();
            failCount++;

            successionDialog.SetActive(true);
        }

        isLevelCompleted = true;

        ReadyNextLevel();
        StartCoroutine(ShowSuccessDialog(2.0f));
    }

    private void ReadyNextLevel() {
        makeButtonsVisible(false);

        ResetDigitCircleColor();

        foreach (Transform child in Digitcircle.transform) {
            Destroy(child.gameObject);
        }

        foreach (Transform child in options.transform) {
            Destroy(child.gameObject);
        }

        enteredOptions.Clear();
        currentIndex = 0;
    }

    IEnumerator ShowSuccessDialog(float delay) {
        yield return new WaitForSeconds(delay);

        successionDialog.SetActive(false);

        StartCoroutine(ShowImage());
    }

    private void ChangeImageColor(GameObject imageObject, Color color) {
        Image imageComponent = imageObject.GetComponent<Image>();

        if (imageComponent != null) {
            imageComponent.color = color;
        }
    }

    private IEnumerator ChangeColorTemporarily(GameObject gameObject)
    {
        ChangeImageColor(gameObject, Color.green);
        yield return new WaitForSeconds(1f);
        ChangeImageColor(gameObject, Color.white);
    }

    private void makeButtonsVisible(bool isVisible) {
        options.SetActive(isVisible);
        deleteButton.gameObject.SetActive(isVisible);
        checkAnswerButton.gameObject.SetActive(isVisible);
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
}
