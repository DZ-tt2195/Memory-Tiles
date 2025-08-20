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
        set { _health = value;
            mistakesLeftText.text = $"{Translator.inst.GetText("Mistakes Left")} {value}"; }
    }

    [SerializeField] TMP_Text minigameCountText;
    int _minigames;
    int minigames
    {
        get { return _minigames; }
        set
        { _minigames = value;
            minigameCountText.text = $"{Translator.inst.GetText("Next Minigame")} ";
            if (_minigames < listOfTiles.Count)
                minigameCountText.text += $"{value} {Translator.inst.GetText("Tiles")}";
            else
                minigameCountText.text += $"{Translator.inst.GetText("N/A")}";
        }
    }

    int score = 0;
    bool cheating = false;

    private void Awake()
    {
        instance = this;
        possibleSprites = Resources.LoadAll<Sprite>("Tile Sprites");
        thisScene = gameObject.scene;
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

        score = minigames * 10 + listOfTiles.Count * 100;

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
            instructions.text = $"{Translator.inst.GetText($"Revealing Tiles")} {elapsedTime:F1}{Translator.inst.GetText($"Sec")}";
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
        instructions.text = Translator.inst.GetText("Find This Tile");
    }

    IEnumerator AnalyzeTile(Tile tile)
    {
        if (tile.mySprite == toFind.sprite)
        {
            listOfTiles.Remove(tile);
            tile.gameObject.SetActive(false);
            toFind.sprite = null;

            instructions.text = Translator.inst.GetText("Correct Tile");
            minigames--;
            yield return new WaitForSeconds(0.7f);

            if (listOfTiles.Count == 0)
            {
                instructions.text = Translator.inst.GetText("Victory");
            }
            else if (minigames == 0)
            {
                minigames = PlayerPrefs.GetInt("Minigame");
                instructions.text = Translator.inst.GetText("Time for Minigame");

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
                        instructions.text = Translator.inst.GetText("Second Glance");
                        yield return new WaitForSeconds(0.7f);
                        yield return RevealTiles(4f); break;
                    case MinigameGrade.Good:
                        instructions.text = Translator.inst.GetText("Second Glance");
                        score -= 3;
                        yield return new WaitForSeconds(0.7f);
                        yield return RevealTiles(2f); break;
                    case MinigameGrade.Barely:
                        instructions.text = Translator.inst.GetText("Second Glance");
                        score -= 6;
                        yield return new WaitForSeconds(0.7f);
                        yield return RevealTiles(0.25f); break;
                    case MinigameGrade.Failed:
                        instructions.text = Translator.inst.GetText("Failed Glance");
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
                instructions.text = Translator.inst.GetText("Lost");
            }
            else
            {
                instructions.text = Translator.inst.GetText("Wrong Tile");
            }
        }
    }

    #endregion

}
