using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class IntensityShift : MonoBehaviour
{
    #region Serializable fields
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
    #endregion
    #region Properties
    /// <summary>
    /// The PostProcessVolume to apply the colour intensity shift to
    /// </summary>
    private PostProcessVolume postProcessVolume;
    private Bloom bloom;
    #endregion

    #region Unity lifecycle methods
    void Start()
    {
        bloom = ScriptableObject.CreateInstance<Bloom>();
        bloom.enabled.Override(true);
    }

    void FixedUpdate()
    {
        bloom.intensity.Override(CalculateIntensity());
    }
    #endregion

    private float CalculateIntensity()
    {
        var minimumFactor = Mathf.Pow(2, minimumIntensity);
        var maximumFactor = Mathf.Pow(2, maximumIntensity);
        var midpoint = (maximumFactor + minimumFactor) / 2;
        var amplitude = (maximumFactor - minimumFactor) / 2;

        return midpoint + amplitude * Mathf.Sin(Time.time * frequency);
    }
}
