using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ChooseWhatYouHear : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] allSounds;

    public TextMeshProUGUI countdownDisplay;
    public TextMeshProUGUI levelTime;
    public TextMeshProUGUI[] choices;

    public Slider progressBar;

    public GameObject options;
    public GameObject succedDialog;
    public GameObject Digitcircle;
    public GameObject imageDigitCirclePrefab;
    public GameObject showingSpeakerObject;

    public Button optionPrefab;
    public Button deleteButton;
    public Button checkAnswerButton;
    public Button repeatButton;

    public int countdownTime = 3;

    public bool isBackward;


    private List<AudioClip> playedSounds = new List<AudioClip>();

    private int levelTimeCountdown;
    private int listeningAudioCount = 2;
    private int currentIndex = 0;
    private int succesCount = 0;
    private int failCount = 0;
    private int level = 2;

    private bool isLevelCompleted = false;
    private bool isCheckingInput = false;
    private bool isRepeat = false;
    private bool isRepeated = false;
    private bool showRepeat = true;

    private List<string> enteredOptions = new List<string>();

    private string[] soundNames;

    private Sprite[] allImages;


    void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    private void Update()
    {
        checkAnswerButton.interactable = (playedSounds.Count == enteredOptions.Count);
        repeatButton.gameObject.SetActive(progressBar.value == 0.75f && failCount == 0 && showRepeat);
    }

    public void Repeat()
    {
        isRepeat = true;
        showRepeat = false;
        levelTime.gameObject.SetActive(false);
        CheckSuccessRate(false);
    }

    private IEnumerator CountdownToStart()
    {
        options.SetActive(false);
        countdownDisplay.gameObject.SetActive(true);

        countdownDisplay.text = isBackward ? "BackwardSpanAudio'a Hoşgeldiniz!" : "ForwardSpanAudio'a Hoşgeldiniz!";
        yield return new WaitForSeconds(1);
        countdownDisplay.text = "Başlayalım...";
        yield return new WaitForSeconds(1);

        countdownDisplay.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);

        StartCoroutine(PlayNumberofSounds());
    }

    private IEnumerator InitializeGame()
    {
        StartCoroutine(PlayNumberofSounds());

        yield return new WaitForSeconds(1);

        ShuffleArray(soundNames);

        ShuffleList(playedSounds);

        yield return new WaitForSeconds(1);

        StartCoroutine(ReadyToStart());

        yield return null;
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
        if (!isRepeat) playedSounds.Clear();
        enteredOptions.Clear();
        currentIndex = 0;
    }

    private IEnumerator ReadyToStart()
    {
        isCheckingInput = true;
        countdownDisplay.gameObject.SetActive(true);
        ResetDigitCircleColor();

        if (isBackward)
        {
            StartCoroutine(ActivateRotation());
            yield return new WaitForSeconds(1.5f);
        }

        countdownDisplay.text = "Sıra sende..";

        yield return new WaitForSeconds(1);

        countdownDisplay.gameObject.SetActive(false);
        options.SetActive(true);

        CreateOptions(playedSounds.Count);
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

    private IEnumerator PlayNumberofSounds()
    {
        isCheckingInput = false;
        allSounds = Resources.LoadAll<AudioClip>("Sounds");
        Sprite speakerImage = Resources.Load<Sprite>("speaker");
        CreateElementsInDigitCircle(listeningAudioCount);

        if (allSounds != null && allSounds.Length > 0)
        {
            while (playedSounds.Count < listeningAudioCount)
            {
                AudioClip randomSound;
                do
                {
                    randomSound = allSounds[Random.Range(0, allSounds.Length)];
                } while (playedSounds.Contains(randomSound));

                playedSounds.Add(randomSound);
                Debug.Log("Loaded sound name: " + playedSounds.Count + ": " + randomSound.name);
                yield return null;
            }

            showingSpeakerObject.GetComponent<Image>().sprite = speakerImage;
            showingSpeakerObject.SetActive(true);

            while (currentIndex < playedSounds.Count)
            {
                AudioClip currentSound = playedSounds[currentIndex];
                audioSource.clip = currentSound;
                audioSource.Play();
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.green);
                yield return new WaitForSeconds(3);
                audioSource.Stop();

                currentIndex++;
                yield return new WaitForSeconds(1);
            }

            showingSpeakerObject.SetActive(false);

            if (isBackward)
            {
                playedSounds.Reverse();
            }

            StartCoroutine(ReadyToStart());
        }
        else
        {
            Debug.LogError("No sounds found in the 'Sounds' folder in Resources.");
        }
    }

    private void CreateOptions(int numberOfElements)
    {
        allImages = Resources.LoadAll<Sprite>("Animals");
        numberOfElements = Mathf.Min(numberOfElements * 2, 8);
        GridLayoutGroup optionsGridLayoutGroup = options.GetComponentInChildren<GridLayoutGroup>();

        if (numberOfElements <= 4)
        {
            optionsGridLayoutGroup.cellSize = new Vector2(200, 200);
            optionsGridLayoutGroup.spacing = new Vector2(100, 100);
        }
        else if (numberOfElements >= 6)
        {
            optionsGridLayoutGroup.spacing = new Vector2(50, 50);
        }
        else if (numberOfElements >= 8)
        {
            optionsGridLayoutGroup.cellSize = new Vector2(150, 150);
        }

        Sprite[] generatedOptionImages = SetOptions(numberOfElements);

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
        List<Sprite> generatedOptionImages = new List<Sprite>();
        int nonAnswerImages = numberOfElements - playedSounds.Count;

        List<string> imagesOfPlayedSounds = new List<string>();

        foreach (AudioClip audio in playedSounds)
        {
            Sprite playedSoundImage = allImages.FirstOrDefault(image => image.name == audio.name);

            if (playedSoundImage != null)
            {
                generatedOptionImages.Add(playedSoundImage);
            }
        }

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

        ShuffleList1(generatedOptionImages);

        return generatedOptionImages.ToArray();
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
                List<string> playedSoundsName = playedSounds.Select(option => option.name).ToList();
                bool isSuccess = IsListEqual(enteredOptions, playedSoundsName);
                CheckSuccessRate(isSuccess);
            }
            else
            {
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                enteredOptions.Add(optionName);
                currentIndex++;
                ChangeImageColor(newImage, Color.green);

                if (currentIndex == playedSounds.Count)
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

    private void CheckSuccessRate(bool isSuccess)
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
        if (isSuccess)
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
                listeningAudioCount++;
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
            if (failCount == 4 && listeningAudioCount > 2)
            {
                failCount = 0;
                listeningAudioCount--;
                showRepeat = true;
            }
        }
        }

        isLevelCompleted = true;
        options.SetActive(false);

        GameStartRules(isRepeat);
        StartCoroutine(HideSuccessDialog(2.0f));
    }

    IEnumerator HideSuccessDialog(float delay)
    {
        yield return new WaitForSeconds(delay);
        succedDialog.SetActive(false);
        StartCoroutine(InitializeGame());
    }

    private IEnumerator LevelTimeStart()
    {
        makeButtonsVisible(true);
        levelTimeCountdown = (listeningAudioCount * 3) + 2;
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
        makeButtonsVisible(false);
        countdownDisplay.text = "Süre Doldu...";
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);

        List<string> playedSoundsName = playedSounds.Select(option => option.name).ToList();
        bool isSuccess = IsListEqual(enteredOptions, playedSoundsName);
        CheckSuccessRate(isSuccess);
    }

    private bool IsPreviousSoundIndex(int[] previousIndices, int currentIndex)
    {
        for (int i = 0; i < previousIndices.Length; i++)
        {
            if (previousIndices[i] == currentIndex)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator WaitAndPlayNextSound(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

    void ShuffleArray(string[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    void ShuffleList(List<AudioClip> array)
    {
        for (int i = array.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            AudioClip temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    void ShuffleList1(List<Sprite> array)
    {
        for (int i = array.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Sprite temp = array[i];
            array[i] = array[j];
            array[j] = temp;
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

    private void CreateElementsInDigitCircle(int numberOfElements)
    {
        for (int i = 0; i < numberOfElements; i++)
        {
            GameObject newImage = Instantiate(imageDigitCirclePrefab, Digitcircle.transform);

            newImage.transform.localPosition = new Vector3(i * 2, 0, 0);

            newImage.name = "ImageElement" + i;
        }
    }

    private void makeButtonsVisible(bool isVisible)
    {
        deleteButton.gameObject.SetActive(isVisible);
        checkAnswerButton.gameObject.SetActive(isVisible);
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
