using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using View;

public class DiceRollTests
{
    private GameObject controller;
    private GameObject table;
    private GameObject walls;
    private View.Board board;
    private View.DiceContainer dice;

    [SetUp]
    public void Setup()
    {
        GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/SoundManager"));
        table = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Enviornment/Table"));
        controller = new GameObject();
        walls = GameObject.Instantiate(Asset.Walls);
        board = View.Board.Create(controller.transform, Model.BoardData.LoadBoard());
        dice = DiceContainer.Create(controller.transform);
        walls.SetActive(true);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator DiceRollTest()
    {
        float time_passed = 0f;
        dice.random_throw();
        
        while(dice.areRolling() && time_passed < 10f) { time_passed+=Time.deltaTime; yield return null;}
        Assert.Pass("Dice showed: " + dice.get_result());
        if (time_passed >= 10f)
        {
            //when dice gets stuck returns -14
            Assert.AreEqual(-14,dice.get_result());
        }
        else
        {
            // value between 2 and 12
            Assert.LessOrEqual(dice.get_result(),12);
            Assert.GreaterOrEqual(dice.get_result(),2);
        }
    }
}
