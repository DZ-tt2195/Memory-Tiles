using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using TMPro;

public class Bundle
{
    public InputAction action;
    public string composite;

    public Bundle(InputAction action, string composite = "")
    {
        this.action = action;
        this.composite = composite;
    }
}

public class PlayModeTests
{

#region Setup

    Keyboard keyboard;
    //NewControls controls;
    //Player player;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        keyboard = InputSystem.AddDevice<Keyboard>();
        //controls = new NewControls();
        //controls.PlayerMovement.Enable();
        SceneManager.LoadScene("Startup");
    }

    void PressKeys(params Bundle[] args)
    {
        Key[] listOfKeys = new Key[args.Length];
        for (int i = 0; i<args.Length; i++)
        {
            Bundle bundle = args[i];
            var binding = bundle.action.bindings[0];
            if (!binding.isComposite)
            {
                //Debug.Log(binding.path["<Keyboard>/".Length..]);
                AddKey(binding.path["<Keyboard>/".Length..]);
            }
            else
            {
                for (int j = 0; j < bundle.action.bindings.Count; j++)
                {
                    var part = bundle.action.bindings[j];
                    if (part.name.Equals(bundle.composite, StringComparison.OrdinalIgnoreCase))
                    {
                        AddKey(part.path["<Keyboard>/".Length..]);
                    }
                }
            }

            void AddKey(string keyName)
            {
                if (!Enum.TryParse<Key>(keyName, true, out var key))
                    throw new Exception($"Cannot parse key name '{keyName}'");
                listOfKeys[i] = key;
            }
        }
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(listOfKeys));
        InputSystem.Update();
    }

    IEnumerator StartScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        yield return new WaitForSeconds(0.5f);
        //player = GameObject.Find("Player").GetComponent<Player>();
    }

    [UnityTearDown]
    public void NoKeys()
    {
        PressKeys();
    }

    #endregion

    /*

#region Test 1

    [UnityTest]
    public IEnumerator GrabClosestBox()
    {
        yield return StartScene("Test 1");
        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry));
        yield return new WaitForSeconds(0.2f);
        Box box = GameObject.Find("Box").GetComponent<Box>();
        Assert.IsNotNull(player.pickedBox);
        Assert.AreEqual(player.pickedBox, box);
    }

    [UnityTest]
    public IEnumerator MoveRight()
    {
        yield return StartScene("Test 1");
        PressKeys(new Bundle(controls.PlayerMovement.Move, "Right"));

        yield return null;
        Assert.Greater(player.transform.position.x, 0);
    }

    [UnityTest]
    public IEnumerator ThrowUp()
    {
        yield return StartScene("Test 1");
        Box box = GameObject.Find("Box").GetComponent<Box>();
        Vector2 originalPosition = box.transform.position;
        PressKeys(new Bundle(controls.PlayerMovement.BoxThrow));

        bool airborne = false;
        float timeout = 1f;
        float startTime = Time.time;

        yield return new WaitUntil(() =>
        {
            airborne = box.transform.position.y > originalPosition.y;
            return airborne || Time.time - startTime > timeout;
        });

        Assert.IsTrue(airborne);
        Assert.AreEqual(originalPosition.x, box.transform.position.x);
    }

    #endregion

#region Test 2

    [UnityTest]
    public IEnumerator ButtonUse()
    {
        yield return StartScene("Test 2");
        ButtonSwitch targetSwitch = GameObject.Find("Button").GetComponent<ButtonSwitch>();
        Assert.IsFalse(targetSwitch.activate);

        SpriteRenderer spriteSwitch = targetSwitch.GetComponent<SpriteRenderer>();
        Assert.AreEqual(spriteSwitch.color, Translator.inst.deactivateMaterial.color);

        TMP_Text textBox = targetSwitch.transform.GetComponentInChildren<TMP_Text>();
        Assert.AreEqual(textBox.text, Translator.inst.GetText("Off"));

        foreach (GameObject next in targetSwitch.toDisable)
        {
            Assert.IsTrue(next.activeSelf);
            TestClone(next);
        }
        foreach (GameObject next in targetSwitch.toEnable)
        {
            Assert.IsFalse(next.activeSelf);
            TestClone(next);
        }

        int drawnLines = 0;
        foreach (Transform child in targetSwitch.transform)
        {
            if (child.GetComponent<LineRenderer>() != null)
                drawnLines++;
        }
        Assert.AreEqual(drawnLines, targetSwitch.toDisable.Count + targetSwitch.toEnable.Count);

        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry));
        yield return null;
        PressKeys();
        yield return null;
        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry));

        yield return new WaitForSeconds(0.2f);
        //button is now on

        Assert.IsTrue(targetSwitch.activate);
        Assert.AreEqual(textBox.text, Translator.inst.GetText("On"));

        Assert.AreEqual(spriteSwitch.color, Translator.inst.activateMaterial.color);
        foreach (GameObject next in targetSwitch.toDisable)
            Assert.IsFalse(next.activeSelf);
        foreach (GameObject next in targetSwitch.toEnable)
            Assert.IsTrue(next.activeSelf);

        yield return null;
        PressKeys();
        yield return null;
        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry));
        yield return null;
        PressKeys();
        yield return null;
        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry));

        yield return new WaitForSeconds(0.2f);
        //button is now off

        Assert.IsFalse(targetSwitch.activate);
        Assert.AreEqual(textBox.text, Translator.inst.GetText("Off"));
        Assert.AreEqual(spriteSwitch.color, Translator.inst.deactivateMaterial.color);

        foreach (GameObject next in targetSwitch.toDisable)
            Assert.IsTrue(next.activeSelf);
        foreach (GameObject next in targetSwitch.toEnable)
            Assert.IsFalse(next.activeSelf);
    }

    void TestClone(GameObject obj)
    {
        GameObject clone = GameObject.Find($"{obj.name}(Clone)");
        Assert.IsTrue(clone);
        Assert.AreEqual(clone.transform.position, obj.transform.position);
        Assert.IsTrue(clone.activeSelf);
        Assert.IsTrue(clone.GetComponents<Component>().Length == 2);
    }

    #endregion

#region Test 3

    [UnityTest]
    public IEnumerator BoxInWall()
    {
        yield return StartScene("Test 3");
        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry), new Bundle(controls.PlayerMovement.Move, "Right"));
        yield return null;

        PressKeys();
        Assert.IsNotNull(player.pickedBox);
        yield return null;

        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry));
        yield return null;
        Assert.IsNotNull(player.pickedBox);

        yield return null;
        yield return new WaitForSeconds(0.2f);
        Assert.IsNotNull(player.pickedBox);

        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry));
        yield return null;
        Assert.IsNotNull(player.pickedBox);
    }

    #endregion

#region Test 4

    [UnityTest]
    public IEnumerator P_PlateUse()
    {
        yield return StartScene("Test 4");
        PressurePlate targetSwitch = GameObject.Find("Pressure Plate").GetComponent<PressurePlate>();

        SpriteRenderer spriteSwitch = targetSwitch.GetComponent<SpriteRenderer>();
        Assert.AreEqual(spriteSwitch.color, Translator.inst.deactivateMaterial.color);

        TMP_Text textBox = targetSwitch.transform.GetComponentInChildren<TMP_Text>();
        Assert.AreEqual(textBox.text, Translator.inst.GetText("Off"));

        foreach (GameObject next in targetSwitch.toDisable)
        {
            Assert.IsTrue(next.activeSelf);
            TestClone(next);
        }
        foreach (GameObject next in targetSwitch.toEnable)
        {
            Assert.IsFalse(next.activeSelf);
            TestClone(next);
        }

        int drawnLines = 0;
        foreach (Transform child in targetSwitch.transform)
        {
            if (child.GetComponent<LineRenderer>() != null)
                drawnLines++;
        }
        Assert.AreEqual(drawnLines, targetSwitch.toDisable.Count + targetSwitch.toEnable.Count);

        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry));
        yield return null;
        PressKeys();
        yield return null;
        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry));

        yield return new WaitForSeconds(0.2f);
        //box is on plate

        Assert.AreEqual(spriteSwitch.color, Translator.inst.activateMaterial.color);
        Assert.AreEqual(textBox.text, Translator.inst.GetText("On"));
        foreach (GameObject next in targetSwitch.toDisable)
            Assert.IsFalse(next.activeSelf);
        foreach (GameObject next in targetSwitch.toEnable)
            Assert.IsTrue(next.activeSelf);

        yield return null;
        PressKeys();
        yield return null;
        PressKeys(new Bundle(controls.PlayerMovement.BoxCarry));

        yield return new WaitForSeconds(0.2f);
        //box is not on plate

        Assert.AreEqual(spriteSwitch.color, Translator.inst.deactivateMaterial.color);
        Assert.AreEqual(textBox.text, Translator.inst.GetText("Off"));
        foreach (GameObject next in targetSwitch.toDisable)
            Assert.IsTrue(next.activeSelf);
        foreach (GameObject next in targetSwitch.toEnable)
            Assert.IsFalse(next.activeSelf);
    }

    #endregion
    */
}
