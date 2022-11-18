using UnityEngine;
using System;

public enum GameType { TUTORIAL, NORMAL, TIME_ATTACK, SURVIVAL, BOSS_FIGHT }

public enum GameResult { WIN, LOSE }

public enum EnemyMovementType { FIXED, ONGRID, AROUND }

public enum EnemyAttackType { RANGE, MELEE, BUFF }

public enum Difficulties { EASY, NORMAL, HARD, VETERAN }

public enum LevelLocationType { GRASS, MUD, STONE, LAVA, ICE, SKY }

public enum CubeSkill { FIRE, ICE, LIGHTNING, WIND, POSION, HEALTH }

public enum CubeFace { TOP_FACE, FORWARD_FACE, RIGHT_FACE, LEFT_FACE, BACKWARD_FACE, BOTTOM_FACE }

public enum CubeType { MAGIC, DEFAULT }

public enum Direction { FORWARD, LEFT, BACKWARD, RIGHT }

public enum ActorState { SLOW, POISION }

public enum GAME_STATE
{
    CUT_SCENE, PLAYING, END, PAUSE
}

public enum SceneState
{
    SPLASH_SCENE, MENU_SCENE, GAME_SCENE, LOADING_SCENE, LEVEL_SCENE, OVER_SCENE
}

public enum SOUND_KEY
{
    MENU_BG, BUTTON_CLICK, READY, MONSTER_ROAR, PLAYER_DIE, CUBE_ROLL, MONSTER_DIE, PLAYER_HURT, SKILL_LIGHTNING, SKILL_FIRE, SKILL_ICE, SKILL_HEALTH, SKILL_WIND, SKILL_POISON, CUBE_ACTIVE, SOUND_FAIL, SOUND_WIN, GOBLIND_LAUGH, GOBLIN_DIE, DRAGON_FLY, DRAGON_ROAR
}

public static class EnumerationUtils
{
    public static CubeFace RandomCubeFace
    {
        get { return GetRandomEnum<CubeFace>(); }
    }

    public static CubeSkill RandomCubeSkill
    {
        get { return GetRandomEnum<CubeSkill>(); }
    }

    public static CubeFace RandomCubeFaceException(CubeFace exception)
    {
        return GetRandomEnumExceptValue<CubeFace>(exception);
    }

    public static CubeSkill RandomCubeSkillException(CubeSkill exception)
    {
        return GetRandomEnumExceptValue<CubeSkill>(exception);
    }

    private static T GetRandomEnum<T>()
    {
        Array A = Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        return V;
    }

    private static T GetRandomEnumExceptValue<T>(T exception) where T : IComparable
    {
        T V = exception;

        while (V.CompareTo(exception) == 0)
        {
            V = GetRandomEnum<T>();
        }

        return V;
    }

    public static T[] GetEnumArray<T>()
    {
        return (T[])Enum.GetValues((typeof(T)));
    }

    public static int GetEnumIndexByName<T>(string name)
    {
        Array A = Enum.GetNames(typeof(T));

        for (int i = 0; i < A.Length; i++)
        {
            if (name == (string)A.GetValue(i))
                return i;
        }
        return -1;
    }
}
