using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class forwardSpanAudioNumber : MonoBehaviour
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

    public Button repeatButton;
    public Button optionPrefab;
    public Button checkAnswerButton;
    public Button deleteButton;

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

    private Sprite[] allImages;


    void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    private void Update()
    {
        checkAnswerButton.interactable = (playedSounds.Count == enteredOptions.Count);
        repeatButton.gameObject.SetActive(progressBar.value == 0.75f && failCount == 0 && isCheckingInput && showRepeat);
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

        countdownDisplay.text = isBackward ? "BackwardSpanAudioNumber'a Hoşgeldiniz!" : "ForwardSpanAudioNumber'a Hoşgeldiniz!";
        yield return new WaitForSeconds(1);
        countdownDisplay.text = "Başlayalım...";
        yield return new WaitForSeconds(1);

        countdownDisplay.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(true);

        StartCoroutine(PlayNumberofSounds());
    }

    private IEnumerator InitializeGame()
    {
        yield return StartCoroutine(PlayNumberofSounds());

        yield return new WaitForSeconds(2.0f);

        yield return null;
    }

    private void GameStartRules(bool isRepeat)
    {
        options.SetActive(false);
        makeButtonsVisible(false);

        foreach (Transform child in options.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in Digitcircle.transform)
        {
            Destroy(child.gameObject);
        }

        if(!isRepeat) playedSounds.Clear();
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

        CreateOptions(listeningAudioCount);
        currentIndex = 0;
        isLevelCompleted = false;
        StartCoroutine(LevelTimeStart());
    }

    private void CreateOptions(int numberOfElements)
    {
        numberOfElements = Mathf.Min(numberOfElements * 2, 8);
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

        List<int> playedSoundNumbers = playedSounds.Select(sound => int.Parse(sound.name)).ToList();
        List<int> allNumbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        List<int> numbersToAdd = allNumbers.Except(playedSoundNumbers).ToList();

        // Shuffle the numbersToAdd list
        for (int i = 0; i < numbersToAdd.Count; i++)
        {
            int temp = numbersToAdd[i];
            int randomIndex = UnityEngine.Random.Range(i, numbersToAdd.Count);
            numbersToAdd[i] = numbersToAdd[randomIndex];
            numbersToAdd[randomIndex] = temp;
        }

        int addingNumber = numberOfElements - playedSoundNumbers.Count;

        if (playedSounds.Count >= 4)
        {
           addingNumber = 5;
        }

        List<int> optionsNumbers = playedSoundNumbers.Concat(numbersToAdd.Take(addingNumber)).ToList();
        optionsNumbers.Sort();

        foreach (int number in optionsNumbers)
        {
            Button option = Instantiate(optionPrefab, options.transform);
            option.name = number.ToString();
            option.GetComponentInChildren<TextMeshProUGUI>().text = number.ToString();
            option.onClick.AddListener(() => EnteredOption(option.name));
        }
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
        allSounds = Resources.LoadAll<AudioClip>("NumberSounds");
        Sprite speakerImage = Resources.Load<Sprite>("speaker");
        CreateElementsInDigitCircle(listeningAudioCount);

        System.Random rnd = new System.Random();
        allSounds = allSounds.OrderBy(x => rnd.Next()).ToArray();

        if (listeningAudioCount == 2)
        {
            allSounds = allSounds.Take(4).ToArray();
        }
        else if (listeningAudioCount == 3)
        {
            allSounds = allSounds.Take(6).ToArray();
        }
        else if (listeningAudioCount == 4)
        {
            allSounds = allSounds.Take(8).ToArray();
        }

        if (allSounds != null && allSounds.Length > 0)
        {
            int lastRandomNum = -1;

            while (playedSounds.Count < listeningAudioCount)
            {
                AudioClip randomSound;
                int randomNum;
                do
                {
                    randomNum = UnityEngine.Random.Range(0, allSounds.Length);
                    randomSound = allSounds[randomNum];
                } while (playedSounds.Contains(randomSound) || IsConsecutive(lastRandomNum, randomNum));

                lastRandomNum = randomNum;
                playedSounds.Add(randomSound);
                Debug.Log("Loaded sound name: " + playedSounds.Count + ": " + randomSound.name);
                yield return null;
            }

            Debug.Log("playedSounds Contents: " + string.Join(", ", playedSounds.Select(sound => sound.name).ToArray()));

            showingSpeakerObject.GetComponent<Image>().sprite = speakerImage;
            showingSpeakerObject.SetActive(true);

            while (currentIndex < playedSounds.Count)
            {
                AudioClip currentSound = playedSounds[currentIndex];
                audioSource.clip = currentSound;
                audioSource.Play();
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                ChangeImageColor(newImage, Color.green);
                while (audioSource.isPlaying)
                {
                    yield return null;
                }

                currentIndex++;
                yield return new WaitForSeconds(1);
            }

            showingSpeakerObject.SetActive(false);

            CheckBackward();
            StartCoroutine(ReadyToStart());
        }
        else
        {
            Debug.LogError("No audio found in the 'NumberSounds' folder in Resources.");
        }
    }

    bool IsConsecutive(int lastNum, int newNum)
    {
        if (listeningAudioCount == 2) return (Mathf.Abs(lastNum - newNum) == 1);
        else return (Mathf.Abs(lastNum - newNum) == 1 || Mathf.Abs(lastNum - newNum) == 2);
    }

    public void EnteredOption(string optionName)
    {
        if (isCheckingInput == true)
        {
            if (optionName == "Delete")
            {
                    currentIndex = (currentIndex == 0) ? currentIndex : currentIndex - 1;
                    GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                    TextMeshProUGUI textElement = newImage.GetComponentInChildren<TextMeshProUGUI>();
                    ChangeImageColor(newImage, Color.white);
                    textElement.text = "";
                    enteredOptions.RemoveAt(currentIndex);
            }
            else if (optionName == "CheckAnswer")
            {
                options.SetActive(false);
                makeButtonsVisible(false);
                List<string> playedSoundsName = new List<string>();

                for (int i = 0; i < playedSounds.Count; i++)
                {
                    playedSoundsName.Add(playedSounds[i].name);
                }
                bool isSuccess = IsListEqual(enteredOptions, playedSoundsName);
                CheckSuccessRate(isSuccess);
            }
            else
            {
                GameObject newImage = Digitcircle.transform.GetChild(currentIndex).gameObject;
                enteredOptions.Add(optionName);
                currentIndex++;
                ChangeImageColor(newImage, Color.green);
            }
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
        options.SetActive(false);
        makeButtonsVisible(false);

        countdownDisplay.text = "Süre Doldu...";
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);

        List<string> playedSoundsName = new List<string>();

        for (int i = 0; i < playedSounds.Count; i++)
        {
            playedSoundsName.Add(playedSounds[i].name);
        }

        bool isSuccess = IsListEqual(enteredOptions, playedSoundsName);
        CheckSuccessRate(isSuccess);
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

    private void CheckBackward()
    {
        if (isBackward)
        {
            playedSounds.Reverse();
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

     private void makeButtonsVisible(bool isVisible)
    {
        deleteButton.gameObject.SetActive(isVisible);
        checkAnswerButton.gameObject.SetActive(isVisible);
    }
}
