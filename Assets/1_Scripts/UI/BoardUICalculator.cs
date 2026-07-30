using UnityEngine;

public static class BoardUICalculator
{
    public static Color GetCheckerboardColor(int x, int y, Color lightColor, Color darkColor) => (x + y) % 2 == 0 ? darkColor : lightColor;
}