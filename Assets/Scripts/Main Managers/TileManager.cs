using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TileManager : MonoBehaviour
{

#region Setup

    public static TileManager instance;
    Scene thisScene;

    [SerializeField] Tile tilePrefab;
    [SerializeField] Sprite faceDown;

    Sprite[] possibleSprites;
    List<Tile> listOfTiles = new();

    [SerializeField] TMP_Text instructions;
    [SerializeField] Image toFind;

    [SerializeField] TMP_Text mistakesLeftText;
    int _health;
    int health
    {
        get { return _health; }
        set
        {
            _health = value;
            mistakesLeftText.text = Translator.inst.Translate("Mistakes Left", new() { ("Num", value.ToString()) });
        } 
    }

    [SerializeField] TMP_Text minigameCountText;
    int _minigames;
    int minigames
    {
        get { return _minigames; }
        set
        {
            _minigames = value;
            if (_minigames < listOfTiles.Count)
                minigameCountText.text = Translator.inst.Translate("Next Minigame", new() { ("Num", value.ToString()) });
            else
                minigameCountText.text = $"{Translator.inst.Translate("No More Minigames")}";
        }
    }

    int score = 0;
    int maxScore = 0;
    [SerializeField] TMP_Text endText;
    [SerializeField] Button doneButton;
    bool cheating = false;

    private void Awake()
    {
        instance = this;
        possibleSprites = Resources.LoadAll<Sprite>("Tile Sprites");
        thisScene = this.gameObject.scene;
    }

    void Start()
    {
        for (float i = -7.5f; i<=7.5f; i+=1.5f)
        {
            for (float j = -4f; j<= 2f; j+=1.5f)
            {
                Tile newSquare = Instantiate(tilePrefab);
                newSquare.mySprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
                newSquare.name = newSquare.mySprite.name;
                newSquare.transform.position = new(i, j);
                listOfTiles.Add(newSquare);
            }
        }
        int maxObjects = listOfTiles.Count;
        for (int i = maxObjects-1; i>=PlayerPrefs.GetInt("Tiles"); i--)
        {
            int randomNumber = Random.Range(0, listOfTiles.Count);
            listOfTiles[randomNumber].gameObject.SetActive(false);
            listOfTiles.RemoveAt(randomNumber);
        }
        health = 5;

        minigames = PlayerPrefs.GetInt("Minigame");
        score = (minigames * 10) + (listOfTiles.Count * 100);
        maxScore = score;
        doneButton.gameObject.SetActive(false);
        endText.gameObject.SetActive(false);
        StartCoroutine(RevealTiles(10f));
    }

    #endregion

#region Gameplay

    IEnumerator RevealTiles(float time)
    {
        foreach (Tile tile in listOfTiles)
            tile.spriteRenderer.sprite = tile.mySprite;

        float elapsedTime = time;
        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            instructions.text = $"{Translator.inst.Translate($"Revealing Tiles", new() { ("Num", $"{elapsedTime:F1}")})}";
            yield return null;
        }
        foreach (Tile tile in listOfTiles)
            tile.spriteRenderer.sprite = faceDown;
        NextTile();
    }

    private void Update()
    {
        toFind.gameObject.SetActive(toFind.sprite != null);
        if (toFind.sprite != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.TryGetComponent<Tile>(out Tile clickedTile))
                    {
                        //Debug.Log($"clicked {clickedTile.name}");
                        StartCoroutine(AnalyzeTile(clickedTile));
                    }
                }
            }
        }
        else
        {
        }

        if (Application.isEditor && Input.GetKeyDown(KeyCode.Space))
        {
            cheating = !cheating;
            foreach (Tile tile in listOfTiles)
                tile.spriteRenderer.sprite = (cheating) ? tile.mySprite : faceDown;
        }
    }

    void NextTile()
    {
        Tile randomTile = listOfTiles[Random.Range(0, listOfTiles.Count)];
        toFind.sprite = randomTile.mySprite;
        instructions.text = Translator.inst.Translate("Find This Tile");
    }

    IEnumerator AnalyzeTile(Tile tile)
    {
        if (tile.mySprite == toFind.sprite)
        {
            listOfTiles.Remove(tile);
            tile.gameObject.SetActive(false);
            toFind.sprite = null;

            instructions.text = Translator.inst.Translate("Correct Tile");
            minigames--;
            yield return new WaitForSeconds(0.7f);

            if (listOfTiles.Count == 0)
            {
                instructions.text = Translator.inst.Translate("Victory");
                GameDone();
                if (score > PlayerPrefs.GetInt("High Score"))
                    PlayerPrefs.SetInt("High Score", score);
            }
            else if (minigames == 0)
            {
                minigames = PlayerPrefs.GetInt("Minigame");
                instructions.text = Translator.inst.Translate("Time for Minigame");

                List<string> minigameNames = MinigameManager.inst.GetMinigames();
                string nextMinigame = minigameNames[Random.Range(0, minigameNames.Count)];

                yield return new WaitForSeconds(0.7f);
                MinigameManager.inst.LoadMinigame(nextMinigame);
                yield return new WaitForSeconds(0.7f);

                while (thisScene != SceneManager.GetActiveScene())
                    yield return null;

                switch (MinigameManager.inst.grade)
                {
                    case MinigameGrade.Amazing:
                        instructions.text = Translator.inst.Translate("Second Glance");
                        yield return new WaitForSeconds(0.7f);
                        yield return RevealTiles(4f); break;
                    case MinigameGrade.Good:
                        instructions.text = Translator.inst.Translate("Second Glance");
                        score -= 3;
                        yield return new WaitForSeconds(0.7f);
                        yield return RevealTiles(2f); break;
                    case MinigameGrade.Barely:
                        instructions.text = Translator.inst.Translate("Second Glance");
                        score -= 6;
                        yield return new WaitForSeconds(0.7f);
                        yield return RevealTiles(0.25f); break;
                    case MinigameGrade.Failed:
                        instructions.text = Translator.inst.Translate("Failed Glance");
                        score -= 10;
                        yield return new WaitForSeconds(0.7f);
                        NextTile(); break;
                }
            }
            else
            {
                NextTile();
            }
        }
        else
        {
            health--;
            score -= 10;
            if (health == 0)
            {
                toFind.sprite = null;
                yield return new WaitForSeconds(0.7f);
                instructions.text = Translator.inst.Translate("Lost");
                GameDone();
            }
            else
            {
                instructions.text = Translator.inst.Translate("Wrong Tile");
            }
        }
    }

    void GameDone()
    {
        doneButton.gameObject.SetActive(true);
        endText.gameObject.SetActive(true);
        endText.text = Translator.inst.Translate("End Text", new() { ("Score", score.ToString()), ("BestScore", maxScore.ToString()) });
    }

    #endregion

}
