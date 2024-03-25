using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;



public class ŞekilliNotaForward : MonoBehaviour
{
    public Slider progressBar;

    public TextMeshProUGUI levelTime;
    public TextMeshProUGUI playerTime;
    public TextMeshProUGUI countdownDisplay;

    public GameObject successionDialog;

    public AudioSource audioSource;

    public AudioClip[] allSounds;

    public GameObject soundButtons;

    public int countdownTime = 3;
    public int gameDuration = 120;

    public bool isBackward;


    private List<Button> playedButtons = new List<Button>();
    private List<string> playedSoundNames = new List<string>();
    private List<string> enteredSoundNames = new List<string>();

    private bool isGameActive = false;
    private bool isLevelCompleted = false;

    private int level = 1;
    private int currentIndex = 0;
    private int successCount = 0;
    private int failCount = 0;
    private int levelTimeCountdown;
    private int playerTimeCountdown;


    private void Start()
    {
        playedSoundNames.Clear();
        enteredSoundNames.Clear();
        StartCoroutine(ReadyForStart());
    }

    private void Update()
    {

    }

    private IEnumerator ReadyForStart()
    {
        progressBar.gameObject.SetActive(false);
        countdownDisplay.gameObject.SetActive(true);

        Button[] buttons = soundButtons.GetComponentsInChildren<Button>();
        allSounds = Resources.LoadAll<AudioClip>("Music");
        EnableButtons(false);

        var messages = new[] {
            new { Text = "Şekilli nota Span'e hoşgeldiniz!", Time = 1f },
            new { Text = "Yanan şekillere sırası ile basmanız gerekiyor.", Time = 1f },
            new { Text = "Hazırsan başlayalım!", Time = 1f },
            new { Text = "Şimdi sesleri dinleyelim.", Time = 1.5f },
        };

        foreach (var message in messages)
        {
            countdownDisplay.text = message.Text;
            yield return new WaitForSeconds(message.Time);
        }

        foreach (Button button in buttons)
        {
            EnteredOption(button);
            yield return new WaitForSeconds(1f);
        }

        countdownDisplay.gameObject.SetActive(false);

        currentIndex = 0;
        playedButtons.Clear();
        enteredSoundNames.Clear();
        yield return new WaitForSeconds(1.5f);

        isGameActive = true;
        StartCoroutine(CountdownTimer());

        progressBar.gameObject.SetActive(true);

        StartCoroutine(PlaySound());
    }

    private IEnumerator PlaySound()
    {
        if (allSounds == null || allSounds.Length == 0)
        {
            Debug.LogError("No sound found in the 'Sounds' folder in Resources.");
            yield return null;
        }

        Button[] buttons = soundButtons.GetComponentsInChildren<Button>();
        Button newButton;
        do
        {
            newButton = buttons[Random.Range(0, buttons.Length)];
        } while (playedButtons.Count >= 3 &&
                 playedButtons.GetRange(playedButtons.Count - 3, 3).All(b => b == playedButtons.Last()) &&
                 newButton == playedButtons.Last());

        Debug.Log("new Button is" + newButton.name);
        playedButtons.Add(newButton);
        playedSoundNames.Add(newButton.name);

        for (int i = 0; i < level; i++)
        {
            Button currentButton = playedButtons[i];
            AudioSource source = currentButton.GetComponentInChildren<AudioSource>();
            source.Play();
            StartCoroutine(ChangeColorTemporarily(currentButton));

            while (source.isPlaying)
            {
                yield return null;
            }
        }

        isLevelCompleted = false;

        countdownDisplay.gameObject.SetActive(true);
        countdownDisplay.text = (level == 1) ? "Hangi notayı duydunuz?" : "Sırası ile hangi notalar çaldı?";
        yield return new WaitForSeconds(1f);

        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        EnableButtons(true);

        countdownDisplay.gameObject.SetActive(false);

        currentIndex = 0;

        StartCoroutine(PlayerTimeStart());
    }

    private IEnumerator PlayerTimeStart()
    {
        playerTimeCountdown = (playedButtons.Count * 3) + 2;
        playerTime.text = playerTimeCountdown.ToString();
        playerTime.gameObject.SetActive(true);

        while (playerTimeCountdown > 0)
        {
            playerTime.text = playerTimeCountdown.ToString();
            yield return new WaitForSeconds(1);
            playerTimeCountdown--;

            if (isLevelCompleted)
            {
                yield break;
            }
        }

        checkSuccess(false);
    }

    public void EnteredOption(Button button)
    {
        if (!button.interactable && isGameActive) return;

        AudioSource source = button.GetComponentInChildren<AudioSource>();
        source.Play();

        enteredSoundNames.Add(button.name);
        currentIndex++;

        StartCoroutine(ChangeColorTemporarily(button));

        if (isGameActive) {
            if (playedSoundNames[currentIndex - 1] != button.name)
            {
                checkSuccess(false);
            }
            else if (currentIndex == playedSoundNames.Count)
            {
                bool isSuccess = IsListEqual(playedSoundNames, enteredSoundNames);
                checkSuccess(isSuccess);
            }
        }
    }

    bool IsListEqual(List<string> list1, List<string> list2) {
        List<string> list2Copy = new List<string>(list2);

        if (isBackward)
        {
            list2Copy.Reverse();
        }

        for (int i = 0; i < list1.Count; i++) {
            if (list1[i] != list2Copy[i]) {
                return false;
            }
        }

        return true;
    }

    private void checkSuccess(bool isSuccess)
    {
        playerTime.gameObject.SetActive(false);
        TextMeshProUGUI successionDialogText = successionDialog.GetComponent<TextMeshProUGUI>();

        if (isSuccess)
        {
            level++;
            failCount = 0;
            successCount++;

            successionDialogText.text = "Bravo :)";
            successionDialog.SetActive(true);

        }
        else
        {
            isGameActive = true;
            successCount = 0;
            progressBar.value = 1;
            level = 1;
            levelTimeCountdown = gameDuration;
            failCount++;
            playedButtons.Clear();
            playedSoundNames.Clear();
            enteredSoundNames.Clear();

            successionDialogText.text = "Bilemediniz, baştan başlayalım..";
            successionDialog.SetActive(true);
        }

        isLevelCompleted = true;

        ReadyNextLevel();
        StartCoroutine(ShowSuccessDialog(2.0f));
    }

    private void ReadyNextLevel()
    {
        EnableButtons(false);
        enteredSoundNames.Clear();
        currentIndex = 0;
    }

    IEnumerator ShowSuccessDialog(float delay)
    {
        yield return new WaitForSeconds(delay);

        successionDialog.SetActive(false);

        StartCoroutine(PlaySound());
    }

    private IEnumerator CountdownTimer()
    {
        levelTimeCountdown = gameDuration;
        levelTime.text = levelTimeCountdown.ToString();
        levelTime.gameObject.SetActive(true);
        progressBar.maxValue = 1;
        progressBar.value = 1;

        while (levelTimeCountdown > 0 && isGameActive)
        {
            yield return new WaitForSeconds(1f);
            levelTimeCountdown--;
            progressBar.value = (float)levelTimeCountdown / gameDuration;
            levelTime.text = levelTimeCountdown.ToString();
        }

        levelTime.gameObject.SetActive(false);

        if (levelTimeCountdown <= 0)
        {
            countdownDisplay.text = "Oyun bitti!";
            countdownDisplay.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            countdownDisplay.gameObject.SetActive(false);
            yield return null;
        }
    }

    private void ChangeButtonColor(Button button, Color color)
    {
        button.GetComponent<Image>().color = color;
    }

    private IEnumerator ChangeColorTemporarily(Button button)
    {
        ChangeButtonColor(button, Color.green);
        yield return new WaitForSeconds(0.5f);
        ChangeButtonColor(button, Color.white);
    }

    private void EnableButtons(bool isEnabled)
    {
        Button[] buttons = soundButtons.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.interactable = isEnabled;
        }
    }
}
