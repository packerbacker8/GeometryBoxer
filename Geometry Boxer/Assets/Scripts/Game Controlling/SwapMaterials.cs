using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapMaterials : MonoBehaviour
{
    public Material Init;
    public Material DMG0;
    public Material DMG1;
    public Material DMG2;
    public Material DMG3;
    public Material DMG4;

    private Renderer Character;

    private void Start()
    {
        Character = this.GetComponentInChildren<Renderer>();
    }

    public void SetMaterial0()
    {
        Character.material = Init;
    }
    public void SetMaterial1()
    {
        Character.material = DMG0;
    }
    public void SetMaterial2()
    {
        Character.material = DMG1;
    }
    public void SetMaterial3()
    {
        Character.material = DMG2;
    }
    public void SetMaterial4()
    {
        Character.material = DMG3;
    }
    public void SetMaterial5()
    {
        Character.material = DMG4;
    }
}
