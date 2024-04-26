public enum TypeOfState
{
    Init,
    PlayerTurn,
    EnemyTurn,
}

public enum TypeOfConditionState
{
    ActiveState,
    PlayerActiveState,
    EnemyActiveState,
    InactiveState,
    DeadState,
    StunState,
}

public enum CharacterType
{
    none = 0,
    Warrior = 1,
    Wizard = 2,
    Archer = 3,
    Barbarian = 100,
    Thrower = 101,
    Summoner = 102,
}

public enum PlayerType
{
    none = 0,
    Warrior = 1,
    Wizard = 2,
    Archer = 3,
}

public enum EnemyType
{
    Barbarian = 100,
    Thrower = 101,
    Summoner = 102,
}

public enum InteractionType
{
    None = 0,
    BasicAttack = 1,
    Knight_HeavyAttack = 2,
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
}

public enum UIType
{
    None = 0,
    CharacterUI = 1,

}


