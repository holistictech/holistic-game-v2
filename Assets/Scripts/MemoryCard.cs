using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Card
{
    public Sprite Image { get; set; }
    public bool IsMatched { get; set; }
    public bool IsFlipped { get; set; }
    public Sprite BackSideImage { get; set; }
    public GameObject CardObject { get; set; }

    public void TurnCard()
    {
        Image imageComponent = CardObject.GetComponent<Image>();
        if (imageComponent != null)
        {
            if (IsFlipped)
            {
                imageComponent.sprite = Image;
            }
            else
            {
                imageComponent.sprite = BackSideImage;
            }
        }
        else
        {
            Debug.LogError("No Image component found on CardObject.");
        }
    }
}

public class MemoryCard : MonoBehaviour
{
    private List<Card> cards = new List<Card>();
    private List<Card> matchedCards;
    private Card previousCard;

    private int moves = 0;
    private int maxMoves = 3;

    public bool isSoundIncluded;

    private int showingImageCount = 2;

    private List<Sprite> allImages = new List<Sprite>();
    private List<Sprite> duplicatedImages = new List<Sprite>();
    private List<Sprite> generatedImages = new List<Sprite>();
    private List<string> enteredOptions = new List<string>();

    public TextMeshProUGUI countdownDisplay;
    public TextMeshProUGUI pointDisplay;
    public TextMeshProUGUI movesDisplay;
    public TextMeshProUGUI maxMovesDisplay;
    public GameObject showingImageObject;
    public GameObject options;
    public GameObject imageDigitCirclePrefab;
    public GameObject Digitcircle;
    private bool isLevelCompleted = false;

    public int countdowmTime;

    private int succesCount = 0;
    private int failCount = 0;
    private int points = 0;

    public GameObject succedDialog;
    public TextMeshProUGUI levelTime;
    private int levelTimeCountdown;

    public GameObject optionPrefab;

    private bool isCheckingInput = false;

    private GameObject cardPrefab;
    private Sprite[] images;

    private const int gridRows = 2;
    private const int gridCols = 4;
    private const float offsetX = 2f;
    private const float offsetY = 2.5f;

    private Card firstRevealed = null;
    private Card secondRevealed = null;

    public bool canReveal
    {
        get { return secondRevealed == null; }
    }

    private void Start()
    {
        StartCoroutine(CountdownToStart());
    }

    private void Update()
    {

    }

    private IEnumerator CountdownToStart()
    {
        options.SetActive(false);
        countdownDisplay.gameObject.SetActive(true);

        countdownDisplay.text = "MemoryCard oyununa hoşgeldiniz!";
        yield return new WaitForSeconds(1.0f);
        countdownDisplay.text = "hazırsanız başlayalım!";
        yield return new WaitForSeconds(1.0f);

        countdownDisplay.gameObject.SetActive(false);
        StartCoroutine(AssignImagesToCards());
    }

    private int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++)
        {
            int tmp = newArray[i];
            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    public void CardRevealed(Card card)
    {
        Debug.Log("CardRevealed: " + card.Image.name);

        if (firstRevealed == null)
        {
            firstRevealed = card;
            card.IsFlipped = true;
            card.TurnCard();
        }
        else
        {
            moves++;
            movesDisplay.text = "Yapılan hamle: " + moves.ToString();
            secondRevealed = card;
            card.IsFlipped = true;
            card.TurnCard();
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        if (firstRevealed.Image.name == secondRevealed.Image.name)
        {
            Debug.Log("Matched");
            firstRevealed.IsMatched = true;
            secondRevealed.IsMatched = true;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);

            firstRevealed.IsFlipped = false;
            secondRevealed.IsFlipped = false;
            firstRevealed.TurnCard();
            secondRevealed.TurnCard();
        }

        firstRevealed = null;
        secondRevealed = null;

        if (cards.All(card => card.IsMatched) && moves <= maxMoves)
        {
            Debug.Log("Level completed!");
            CheckSuccessRate(true);
        }
    }

    private IEnumerator AssignImagesToCards()
    {
        pointDisplay.gameObject.SetActive(true);
        movesDisplay.gameObject.SetActive(true);
        maxMovesDisplay.gameObject.SetActive(true);

        isCheckingInput = false;

        allImages = Resources.LoadAll<Sprite>("Images").ToList();

        while (generatedImages.Count < showingImageCount)
        {
            Sprite randomImage;
            do
            {
                randomImage = allImages[Random.Range(0, allImages.Count)];
            } while (generatedImages.Contains(randomImage));

            generatedImages.Add(randomImage);
            Debug.Log("Loaded image name: " + generatedImages.Count + ": " + randomImage.name);
            yield return null;
        }

        duplicatedImages = generatedImages.Concat(generatedImages).ToList();
        ShuffleList(duplicatedImages);

        maxMoves = (int)(duplicatedImages.Count * 1.5);
        maxMovesDisplay.text = "Hamle hakkı: " + maxMoves.ToString();

        yield return new WaitForSeconds(1);
        StartCoroutine(ReadyToStart());
    }

    private IEnumerator ReadyToStart()
    {
        isCheckingInput = true;
        countdownDisplay.gameObject.SetActive(true);
        pointDisplay.gameObject.SetActive(true);
        pointDisplay.text = points.ToString() + " Puan";
        movesDisplay.gameObject.SetActive(true);
        movesDisplay.text = "Yapılan hamle: " +moves.ToString();
        maxMovesDisplay.gameObject.SetActive(true);
        maxMovesDisplay.text = "Hamle hakkı: " + maxMoves.ToString();

        countdownDisplay.text = "Doğru kartları eşleştirelim";
        yield return new WaitForSeconds(1.0f);
        countdownDisplay.gameObject.SetActive(false);
        options.SetActive(true);

        CreateOptions(showingImageCount);
        isLevelCompleted = false;
        StartCoroutine(LevelTimeStart());
    }

    private void CreateOptions(int numberOfElements)
    {
        numberOfElements = numberOfElements * 2;
        GridLayoutGroup optionsGridLayoutGroup = options.GetComponentInChildren<GridLayoutGroup>();

        if (numberOfElements <= 4)
        {
            options.transform.localPosition = new Vector2(0, 0);
            optionsGridLayoutGroup.cellSize = new Vector2(200, 200);
            optionsGridLayoutGroup.spacing = new Vector2(150, 150);
        }
        if (numberOfElements >= 6)
        {
            options.transform.localPosition = new Vector2(0, 0);
            optionsGridLayoutGroup.cellSize = new Vector2(150, 150);
            optionsGridLayoutGroup.spacing = new Vector2(100, 100);
        }
        if (numberOfElements >= 8)
        {
            options.transform.localPosition = new Vector2(0, 0);
            optionsGridLayoutGroup.cellSize = new Vector2(150, 150);
            optionsGridLayoutGroup.spacing = new Vector2(50, 50);
        }
        if (numberOfElements >= 10)
        {
            options.transform.localPosition = new Vector2(0, 0);
            optionsGridLayoutGroup.cellSize = new Vector2(100, 100);
            optionsGridLayoutGroup.spacing = new Vector2(50, 50);
        }
        if (numberOfElements >= 12)
        {
            options.transform.localPosition = new Vector2(0, 0);
            optionsGridLayoutGroup.cellSize = new Vector2(100, 100);
            optionsGridLayoutGroup.spacing = new Vector2(25, 25);
        }

        for (int i = 0; i < duplicatedImages.Count; i++)
        {
            Sprite randomImage = duplicatedImages[i];

            GameObject cardObject = Instantiate(optionPrefab, options.transform);
            cardObject.transform.localPosition = new Vector3(i * 2, 0, 0);
            cardObject.GetComponent<Image>().sprite = randomImage;
            cardObject.name = randomImage.name;

            Card card = new Card
            {
                Image = randomImage,
                IsFlipped = false,
                BackSideImage = Resources.Load<Sprite>("CardBack"),
                CardObject = cardObject
            };

            cardObject.GetComponent<Button>().onClick.AddListener(() => CardRevealed(card));

            cards.Add(card);
            card.TurnCard();
        }
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

    private void AddPoints()
    {
        points += 10;
        pointDisplay.text = points.ToString() + " Puan";
    }

    private void CheckSuccessRate(bool isSucces)
    {
        levelTime.gameObject.SetActive(false);
        TextMeshProUGUI succedDialogText = succedDialog.GetComponent<TextMeshProUGUI>();

            if (isSucces)
            {
                AddPoints();

                succedDialogText.text = "Bravo :)";
                succedDialog.SetActive(true);

                showingImageCount++;
            }
            else
            {
                maxMoves = 0;
                points = 0;
                pointDisplay.text = points.ToString() + " Puan";
                failCount++;
                succedDialogText.text = "Tekrar Deneyelim :(";
                succedDialog.SetActive(true);

                if (failCount == 4 && showingImageCount > 2)
                {
                    failCount = 0;
                    showingImageCount--;
                }
            }

        pointDisplay.gameObject.SetActive(false);
        movesDisplay.gameObject.SetActive(false);
        maxMovesDisplay.gameObject.SetActive(false);

        isLevelCompleted = true;
        options.SetActive(false);
        GameStartRules();
        StartCoroutine(HideSuccessDialog(2.0f));
    }

    private void GameStartRules()
    {
        foreach (Transform child in options.transform)
        {
            Destroy(child.gameObject);
        }

        cards.Clear();
        generatedImages.Clear();
        duplicatedImages.Clear();
        moves = 0;
        maxMoves = 3;
    }

    IEnumerator HideSuccessDialog(float delay)
    {
        yield return new WaitForSeconds(delay);
        succedDialog.SetActive(false);
        StartCoroutine(AssignImagesToCards());
    }

    private IEnumerator LevelTimeStart()
    {
        levelTimeCountdown = (duplicatedImages.Count * 4);
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

        countdownDisplay.text = "Süre Doldu...";
        yield return new WaitForSeconds(1);
        countdownDisplay.gameObject.SetActive(false);

        CheckSuccessRate(false);
    }
}
