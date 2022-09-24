using Planet.Common;
using UnityEngine.UIElements;

namespace Planet.Utils
{
    public static class UIElementsExtensions
    {
        public static IntRange ToIntRange(this MinMaxSlider slider) =>
            new((int) slider.minValue, (int) slider.maxValue);

        public static IntRange ToIntRange(this MinMaxSlider slider, int multiplier) =>
            new((int) slider.minValue * multiplier, (int) slider.maxValue * multiplier);

        public static FloatRange ToFloatRange(this MinMaxSlider slider) => new(slider.minValue, slider.maxValue);

        public static FloatRange ToFloatRange(this MinMaxSlider slider, float multiplier) =>
            new(slider.minValue * multiplier, slider.maxValue * multiplier);
    }
}
