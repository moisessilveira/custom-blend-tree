using UnityEngine;

public class InterpolatorTest : MonoBehaviour
{
    public float[][] samples = new float[5][];
    public Interpolator interpolator;
    public float[] interpolated;
    public Vector2 dot;

    public PolarGradientBandInterpolator PolarGradientBandInterpolator;

    void Start()
    {
        samples[0] = new[] { 0f, 0f };
        samples[1] = new[] { -1f, -1f };
        samples[2] = new[] { -1f, 1f };
        samples[3] = new[] { 1f, -1f };
        samples[4] = new[] { 1f, 1f };

        PolarGradientBandInterpolator = new PolarGradientBandInterpolator(samples);

        interpolated = PolarGradientBandInterpolator.Interpolate(new[] { dot.x, dot.y }, true);
    }
}
