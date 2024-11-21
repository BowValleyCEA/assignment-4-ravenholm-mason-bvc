using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTarget : MonoBehaviour, ITargetable
{
    private Material _currentMaterial;

    [SerializeField] private Color targetColor = Color.red;

    private Color initialColor;
    // Start is called before the first frame update
    void Start()
    {
        _currentMaterial = GetComponent<Renderer>().material;
        initialColor = _currentMaterial.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Target()
    {
        _currentMaterial.color = Color.red;
    }

    public void StopTarget()
    {
        _currentMaterial.color = initialColor;
    }
}
