using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;

public class TileManager : MonoBehaviour
{
    [SerializeField] Tile square;
    [SerializeField] Sprite faceDown;
    Sprite[] possibleSprites;
    List<Tile> listOfTiles = new();
    [SerializeField] TMP_Text instructions;
    Sprite toFind = null;

    void Start()
    {
        possibleSprites = Resources.LoadAll<Sprite>("Tile Sprites");

        for (float i = -7.5f; i<=7.5f; i+=1.5f)
        {
            for (float j = -4f; j<= 2f; j+=1.5f)
            {
                Tile newSquare = Instantiate(square);
                //newSquare.mySprite = possibleSprites[Random.Range(0, possibleSprites.Length)];
                //newSquare.name = newSquare.mySprite.name;

                listOfTiles.Add(newSquare);
                newSquare.transform.position = new(i, j);
            }
        }

        int maxObjects = listOfTiles.Count;
        for (int i = maxObjects-1; i>=PlayerPrefs.GetInt("Tiles"); i--)
        {
            int randomNumber = Random.Range(0, listOfTiles.Count);
            listOfTiles[randomNumber].gameObject.SetActive(false);
            listOfTiles.RemoveAt(randomNumber);
        }
        StartCoroutine(Reveal(10f));
    }

    IEnumerator Reveal(float time)
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
    }

    private void Update()
    {
        if (toFind != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.TryGetComponent<Tile>(out Tile clickedTile))
                    {
                        Debug.Log("clicked tile: " + clickedTile.name);
                    }
                }
            }
        }
    }
}
