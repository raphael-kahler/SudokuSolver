namespace SudokuSolver.Techniques.FishTechniques;

internal static class FishNamer
{
    public static string GetFishName(int size) => size switch
    {
        2 => "X-Wing",
        3 => "Swordfish",
        4 => "Jellyfish",
        _ => "no-fish"
    };
}
