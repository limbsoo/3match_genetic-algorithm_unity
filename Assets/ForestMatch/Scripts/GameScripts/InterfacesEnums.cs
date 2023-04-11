namespace Mkey
{
    public enum GameMode { Play, Edit }
    public enum MatchBoardState { ShowEstimate, Fill, Collect, Waiting, Iddle}
    public enum SpawnerStyle { AllEnabled, AllEnabledAlign, DisabledAligned }
    public enum FillType {Step, Fast}
    public enum BombDir { Vertical, Horizontal, Radial } //, Color
    public enum BombType { StaticMatch, DynamicMatch, DynamicClick }
    public enum MatchGroupType { Simple, Hor4, Vert4, LT, BigLT, MiddleLT, Hor5, Vert5 }
    public enum BombCombine { ColorBombAndColorBomb, RadBombAndRadBomb, HV, ColorBombAndRadBomb, BombAndHV, ColorBombAndHV, None }
}
