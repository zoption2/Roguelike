public enum TypeOfState
{
    Init,
    PlayerTurn,
    EnemyTurn,
    Interstitial,
    Pause,
}

public enum TypeOfConditionState
{
    ActiveState,
    PlayerActiveState,
    EnemyActiveState,
    InactiveState,
    DeadState,
    StunState,
    Preparation,
    Finalization,
}

public enum CharacterType
{
    None = 0,
    Warrior = 1,
    Wizard = 2,
    Archer = 3,
    Barbarian = 100,
    Thrower = 101,
    Summoner = 102,
}

public enum PlayerType
{
    None = 0,
    Warrior = 1,
    Wizard = 2,
    Archer = 3,
}

public enum EnemyType
{
    None = 0,
    Barbarian = 100,
    Thrower = 101,
    Summoner = 102,
}

public enum InteractionType
{
    None = 0,
    BasicAttack = 1,
    Knight_HeavyAttack = 2,
    SuperMegaHeavyAttack = 3,
    ArrowWithoutBounceAttack = 4,
}

public enum AbilityType
{
    BasicAttackAbility = 0,
    HeavyAttackAbility = 1,
    MegaHeavyAttackAbility = 2,
    ArrowShootAbility = 3,
}

public enum TypeOfUse
{
    None = 0,
    MeleeUse = 1,
    RangedUse = 2,
    AreaUse = 3,
    ImmediateUse = 4,
}

public enum EffectType
{
    None = 0,
    MoreDamageEffect = 1,
    FireEffect = 2,
    StunEffect = 3,
}

public enum BuffType
{
    None = 0,
    MoreDamage = 1,
    TestBuff1 = 2,
    TestBuff2 = 3,
}

public enum UIType
{
    None = 0,
    CharacterUI = 1,

}

public enum UIElementType
{
    None = 0,
    Menu = 1,
    LevelSelector = 2,


}

public enum NavigationType
{
    Default = 0,
}


public enum ExitDirection
{
    None,
    Left,
    Right,
    Top,
    Bottom
}


public enum TemplateElementType
{
    None = 0,
    Ground = 1,
    Player = 2,
    RandomBuff = 4,
    DefaultWall = 5,
    ReflectionWall = 6,
    Exit = 7,
    Chest = 8,
    Barbarian = 100,
    Thrower = 101,
    Summoner = 102,
    ExitToStoryRoom = 200,
    ExitToBountyRoom = 201,
    ExitToRandomRoom = 202
}

public enum ProjectileType
{
    None = 0,
    Arrow = 1,
}

public enum TypeOfScenario
{
    None,
    DefaultRoom,
    MainRoom,
    Boss,
    BountyRoom,
    RandomRoom,
}

public enum PoolType
{
    BuffPool,
    EffectPool,
    AbilityIconPool,
    CharacterPanelPool,
    CharacterPool,
    CharacterUIPool,
    ProjectilePool,
    SlingshotPool,
    ParticlePool,
    UIPool
}

public enum CurrencyType
{
    Coin
}

public enum ParticleType
{
    WallParticle,
}


