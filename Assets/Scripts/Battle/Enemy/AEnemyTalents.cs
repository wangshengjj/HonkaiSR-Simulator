using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AEnemyTalents: ACommomBattleTalents
{
    protected Enemy self;
    public AEnemyTalents(Enemy _self)
    {
        self = _self;
    }
    public virtual void MyTurn(List<Character> characters, List<Enemy> enemies)
    {

    }

    public virtual void OnBattleStart(List<Character> characters, List<Enemy> enemies)
    {

    }
}
