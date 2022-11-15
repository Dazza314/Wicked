using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IntensityShift : MonoBehaviour
{
    #region Serializable fields
    /// <summary>
    /// List of renderers to apply the colour intensity shift to
    /// </summary>
    [SerializeField]
    private Renderer[] renderers;
    /// <summary>
    /// The maximum intensity to reach
    /// </summary>
    [SerializeField]
    private float maximumIntensity;
    /// <summary>
    /// The minimum intensity to reach
    /// </summary>
    [SerializeField]
    private float minimumIntensity;
    /// <summary>
    /// The angular frequency of the intensity shift (in radians per second)
    /// </summary>
    [SerializeField]
    private float frequency;
    /// <summary>
    /// The base colour of the material
    /// </summary>
    [SerializeField]
    private Color colour;
    #endregion
    #region Properties
    /// <summary>
    /// A list of materials corresponding to <see cref="renderers"/>
    /// </summary>
    private List<Material> materials;
    #endregion

    void Start()
    {
        materials = renderers.Select(renderer => renderer.material).ToList();
    }
    void FixedUpdate()
    {
        materials.ForEach(material => material.SetColor("_EmissionColor", CalculateColour()));
    }

    private Color CalculateColour()
    {
        var minimumFactor = Mathf.Pow(2, minimumIntensity);
        var maximumFactor = Mathf.Pow(2, maximumIntensity);
        var midpoint = (maximumFactor + minimumFactor) / 2;
        var amplitude = (maximumFactor - minimumFactor) / 2;

        var intensityMultiplier = midpoint + amplitude * Mathf.Sin(Time.time * frequency);

        return intensityMultiplier * colour;
    }
}
