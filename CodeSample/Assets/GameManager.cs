using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TMP_Text OrbasCounterText; // Reference to the TextMeshPro text component
    private float OrbasCounter = 0;

    public int LevelNumberSetup = 5;
    private int levelGenOriginNumber = 5;
    private int levelGenCurrentNumber = 5;
    private int previousLocationNumber = 2;

    void Start()
    {
        levelGenOriginNumber = LevelNumberSetup;
        levelGenCurrentNumber = LevelNumberSetup;
    }

    public void AddToOrbas()
    {
        OrbasCounter++;
        OrbasCounterText.text = OrbasCounter.ToString();
    }


    //Dont be stupid and modify the code so that it repeat to infinity
    public void LevelGenerationManager()
    {

    }

    public void SubfromGenLevelNumber()
    {
        if(levelGenCurrentNumber != 0)
        {
            levelGenCurrentNumber--;
        }
        else
        {
            return;
        }
    }

    public void ResetGenLevelNumber()
    {
        levelGenCurrentNumber = levelGenOriginNumber;
    }

    public int GetlevelGenCurrentNumber()
    {
        return levelGenCurrentNumber;
    }

    public int GetpreviousLocationNumber()
    {
        return previousLocationNumber;
    }

    public void AssignpreviousLocationNumber(int newLocationNumber)
    {
        previousLocationNumber = newLocationNumber;
    }
}
