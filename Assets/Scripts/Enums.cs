public enum TypeOfState
{
    Init,
    PlayerTurn,
    EnemyTurn,
}

public enum TypeOfConditionState
{
    ActiveState,
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

public enum BuffType
{
    None = 0,
    MoreDamage = 1,
}

