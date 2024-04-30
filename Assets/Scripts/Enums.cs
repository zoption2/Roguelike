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
    SuperMegaHeavyAttack = 3,
}

public enum AbilityType
{
    BasicAttackAbility = 0,
    HeavyAttackAbility = 1,
    MegaHeavyAttackAbility = 2,
}

public enum TypeOfUse
{
    None = 0,
    MeleeUse = 1,
    RangedUse = 2,
    AreaUse = 3,
    ImmediateUse = 4,
}

public enum BuffType
{
    None = 0,
    MoreDamage = 1,
}

