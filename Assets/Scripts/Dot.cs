using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{

    public GridManager gridManager;
    public Color[] colors;
    public int row;
    public int col;
    SpriteRenderer spriteRenderer;
    public Color dotColor;
    public bool isSelected;
    //static bool SelectActive;
    //Color activeColor;

    // Use this for initialization
    void Start()
    {
        // Get the dot's sprite
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;

        gridManager = GameObject.Find("Grid").GetComponent<GridManager>();

        // Randomize the color
        RandomizeColor();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RandomizeColor()
    {
        int rand = Random.Range(0, colors.Length);
        spriteRenderer.color = colors[rand];
        dotColor = colors[rand];
    }

    public int GetRow()
    {
        return row;
    }

    public int GetCol()
    {
        return col;
    }

    private void OnMouseDown()
    {
        gridManager.selectActive = true;
        //SelectActive = true;
        isSelected = true;
        gridManager.SelectDot(this.transform.gameObject);
    }

    // ISSUE ARISES AFTER SECOND DESELECTION, CONFUSION WITH FINAL TWO ELEMENTS OF ARRAY

    private void OnMouseEnter()
    {
        if (gridManager.selectActive)
        {
            if (!isSelected)
            {
                isSelected = true;
                gridManager.SelectDot(this.transform.gameObject);
            }
            else if (isSelected && gridManager.IsPreviousDot(this.transform.gameObject))
            {
                isSelected = false;
                gridManager.DeselectLastDot();
            }
            else if (isSelected && gridManager.IsSquare(this.transform.gameObject))
            {
                gridManager.SelectDot(this.transform.gameObject);
                gridManager.square = true;
            }
        }
    }

    private void OnMouseUp()
    {
        gridManager.selectActive = false;
        gridManager.RemoveDots();
        Deselected();
    }

    public void Selected()
    {
        isSelected = true;
        Color lit = spriteRenderer.color;
        lit.a = 1.0f;
        spriteRenderer.color = lit;
    }

    public void Deselected()
    {
        isSelected = false;
        Color unlit = spriteRenderer.color;
        unlit.a = 0.5f;
        spriteRenderer.color = unlit;
    }

    public Color GetColor()
    {
        return dotColor;
    }

}