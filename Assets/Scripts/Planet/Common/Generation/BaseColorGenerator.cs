using Noise;
using Planet.Settings;
using UnityEngine;

namespace Planet.Common.Generation
{
    public abstract class BaseColorGenerator
    {
        protected static readonly int ElevationMinMax = Shader.PropertyToID("_elevationMinMax");
        protected static readonly int Texture = Shader.PropertyToID("_texture");

        protected const int TextureResolution = 50;
        protected const int DoubledTextureResolution = TextureResolution * 2;
        protected const float TextureResolutionMinusOne = TextureResolution - 1f;

        protected static int TextureWidth => DoubledTextureResolution;
        protected int TextureHeight => settings.biomeColorSettings.biomes.Length;
        protected int TextureSize => TextureWidth * TextureHeight;

        protected ColorSettings settings;
        protected Texture2D texture;
        protected INoiseFilter biomeNoiseFilter;


        protected static Color GenerateColorFromGradient(Color gradientColor, Color tintColor, float tintPercent) =>
            gradientColor * (1 - tintPercent) + tintColor * tintPercent;
    }
}
