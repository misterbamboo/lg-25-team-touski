public struct GoblinSurprise : IGameEvent 
{ 
    public GoblinMovement goblin; 
    public GoblinSurprise(GoblinMovement goblin)
    {
        this.goblin = goblin;
    }
}
