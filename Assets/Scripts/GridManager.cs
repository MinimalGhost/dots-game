using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
//using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public GameObject currentlySelectedDot;
    public GameObject dotPrefab;            // Prefab to spawn
    public Color activeColor;
    public GameObject previousDot;
    public bool selectActive = false;
    public bool square = false;
    //private List<Vector3> gridPositions = new List<Vector3>();  // track all the possible positions on our grid and what's been spawned in each
    public List<GameObject> dots = new List<GameObject>();                   // 2D List of instantiated Dot scripts
    public List<GameObject> selectedDots;   // Currently selected dots
    public GameObject[,] tiles;
    private float padding = 0.3f;
    public int columns, rows;

    void Start()
    {
        Vector2 offset = dotPrefab.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x + padding, offset.y + padding);
        //SpawnDots();
        //InitializeList();
    }

    // attempt to combine InitializeList and SpawnDots
    private void CreateBoard (float xOffset, float yOffset)
    {
        tiles = new GameObject[columns, rows];

        float startX = transform.position.x;
        float startY = transform.position.y;

        for (int x = 0; x < columns; x++) {
            for (int y = 0; y < rows; y++) {
                GameObject newDot = Instantiate(dotPrefab, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), dotPrefab.transform.rotation);
                tiles[x, y] = newDot;

                Dot dotScript = newDot.GetComponent<Dot>();

                // Assign grid properties to dot instance
                newDot.name = "Dot_" + x + "_" + y;
                dotScript.col = x;
                dotScript.row = y;
                newDot.transform.parent = transform;
                //dots.Add(newDot);
            }
        }
    }

    bool ValidateDot(GameObject next, GameObject prev)
    {
        var nextDot = next.GetComponent<Dot>();
        var prevDot = prev.GetComponent<Dot>();

        var rowDiff = Mathf.Abs(nextDot.row - prevDot.row);
        var colDiff = Mathf.Abs(nextDot.col - prevDot.col);

        // Dot is different color
        if (prevDot.dotColor != nextDot.dotColor)
        {
            return false;
        }
        // Dot in diagonal location
        if (rowDiff > 0 && colDiff > 0)
        {
            return false;
        }
        // Dot isn't directly adjacent
        if (rowDiff > 1 || colDiff > 1)
        {
            return false;
        }

        return true;
    }

    // checks if we've moused over the previously seleted dot to deselect
    public bool IsPreviousDot(GameObject dot) {
        // Dot matches last element in list
        if  (dot == previousDot)
        {
            return true;
        }
        return false;
    }


    public bool IsSquare(GameObject currentDot)
    {
        for (var i = 0; i < selectedDots.Count - 2; i++) {
            if (currentDot == selectedDots[i]) {
                square = true;
                return true;
            }
        }
        return false;
    }

    public void SelectDot(GameObject dot)
    {
        currentlySelectedDot = dot;
        var control = dot.GetComponent<Dot>();

        // this isn't the first selected dot, check it against previous dot properties
        if (selectedDots.Count != 0)
        {
            // get the previous dot's properties
            previousDot = selectedDots[selectedDots.Count - 1];

            if (ValidateDot(previousDot, dot)) {
                selectedDots.Add(dot);
                control.Selected();
                Debug.Log("Length of selection is " + selectedDots.Count);
            }
        }
        else // its the first dot, set the parameters and add it to list
        {
            activeColor = control.GetColor();
            selectedDots.Add(dot);
            control.Selected();
        }
    }

    public void DeselectLastDot()
    {
        var control = selectedDots[selectedDots.Count - 1].GetComponent<Dot>();

        control.Deselected();
        selectedDots.RemoveAt(selectedDots.Count - 1);

        // TODO find an elegant way of dealing with List size of 1
        if (selectedDots.Count > 1) {
            previousDot = selectedDots[selectedDots.Count - 2];
        }
        else
        {
            previousDot = selectedDots[selectedDots.Count - 1];
        }

    }

    public void RemoveDots()
    {

        if (selectedDots.Count >= 2) {
            // check for any squares
            if (square) {
                RemoveSelected();
                // remove all dots of this color
                for (int i = 0; i < columns; i++)
                {
                    for (int j = 0; j < rows; j++)
                    {
                        if (tiles[i, j].GetComponent<SpriteRenderer>().color == activeColor)
                        {
                            Destroy(tiles[i, j]);
                        }
                    }
                }
            }
            else
            {
                RemoveSelected();
            }

        }
        selectedDots.Clear();
        square = false;
    }

    void RemoveSelected()
    {
        foreach (var dot in selectedDots)
        {
            Destroy(dot);
        }
    }

     //possible new method for destroying
    private void DestroyDotsAt(int column, int row) {
        if (tiles[column, row].GetComponent<Dot>().isSelected) {
            Destroy(tiles[column, row]);
            tiles[column, row] = null;
        }
    }

    public void DestroySelected() {
        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                if (tiles[i,j] != null) {
                    DestroyDotsAt(i,j);
                }
            }
        }
    }

    private IEnumerator DecreaseRow() {
        int nullCount = 0;
        for (int i = 0; i < columns; i++) {
            for (int j = 0; j < rows; j++) {
                Debug.Log("TILES READOUT: " + tiles[i, j]);
                if (tiles[i,j] == null) {
                    nullCount++;
                } else if (nullCount > 0) {
                    tiles[i, j].GetComponent<Dot>().row -= nullCount;
                    tiles[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(0.4f);
    }
}