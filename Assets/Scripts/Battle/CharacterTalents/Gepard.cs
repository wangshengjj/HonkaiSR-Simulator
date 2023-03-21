using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gepard : ACharacterTalents
{
    public Gepard(Character c): base(c)
    {

    }
    float atkDmg, skillAtk, skillFreeze, burstDefPer, burstDefIns, talentHp;

    public override void AttackEnemyAction(List<Enemy> enemies)
    {
        Enemy e = enemies[0];
        Damage dmg = Damage.NormalDamage(self, e, CommonAttribute.ATK, Element.Cryo, atkDmg, DamageType.Attack);
        self.DealDamage(e, dmg);
        base.AttackEnemyAction(enemies);
    }

    public override void SkillEnemyAction(List<Enemy> enemies)
    {
        Enemy e = enemies[0];
        Damage dmg = Damage.NormalDamage(self, e, CommonAttribute.ATK, Element.Cryo, skillAtk, DamageType.Skill);
        self.DealDamage(e, dmg);
        float hit = (self.constellaLevel >= 1 ? 1 : .65f) * (1 + self.GetFinalAttr(self, e, CommonAttribute.EffectHit, DamageType.Skill));
        float resist = 1 - 1 / (1 + e.GetFinalAttr(self, e, CommonAttribute.EffectResist, DamageType.Skill));
        if (Utils.TwoRandom(hit) && !Utils.TwoRandom(resist))
        {
            // �������
            e.AddState(self, new State(StateType.Frozen, 1));
            e.mono?.ShowMessage("����", Color.blue);
            if(e.onTurnStart.Find(t => t.tag == "gepardFreeze") == null)
                e.onTurnStart.Add(new TriggerEvent<Creature.TurnStartEndEvent>("gepardFreeze", () =>
                {
                    if (e.IsUnderState(StateType.Frozen))
                    {
                        Damage dmg = Damage.NormalDamage(self, e, CommonAttribute.ATK, Element.Cryo, skillFreeze, DamageType.Continue);
                        self.DealDamage(e, dmg);
                    }
                }));
            if (self.constellaLevel >= 2)
            {
                e.AddBuff("gepardContellation2SpeedDown", BuffType.Debuff, CommonAttribute.Speed, ValueType.Percentage, -.2f, 1);
            }
        }
        base.SkillEnemyAction(enemies);
    }

    public override void BurstCharacterAction(List<Character> characters)
    {
        foreach(Character c in characters)
        {
            c.GetShield(Shield.MakeShield("gepardBurst", self, CommonAttribute.DEF, burstDefPer, burstDefIns, 3));
        }
        base.BurstCharacterAction(characters);
    }

    public override void OnEquipping()
    {
        if (self.constellaLevel >= 5)
        {
            self.SkillLevelUp(2);
            self.ATKLevelUp(1);
        }
        if (self.constellaLevel >= 3)
        {
            self.BurstLevelUp(2);
            self.TalentLevelUp(2);
        }
        atkDmg = (float)(double)self.metaData["atk"]["dmg"]["value"][self.atkLevel];
        skillAtk = (float)(double)self.metaData["skill"]["atk"]["value"][self.skillLevel];
        skillFreeze = (float)(double)self.metaData["skill"]["freeze"]["value"][self.skillLevel];
        burstDefPer = (float)(double)self.metaData["burst"]["defPer"]["value"][self.burstLevel];
        burstDefIns = (int)self.metaData["burst"]["defIns"]["value"][self.burstLevel];
        talentHp = (float)(double)self.metaData["talent"]["hp"]["value"][self.talentLevel];
        
        TriggerEvent<Creature.DamageEvent> t = new TriggerEvent<Creature.DamageEvent>("gepardTalent");
        t.trigger = (s, d) =>
        {
            if (self.hp - d.value <= 0)
            {
                self.hp = ((self.constellaLevel >= 6 ? .5f : 0) + talentHp) * self.GetFinalAttr(CommonAttribute.MaxHP);
                self.mono.hpLine.fillAmount = self.mono.hpPercentage;
                self.mono?.ShowMessage("����֮��", Color.blue);
                t.Zero();
                d.value = 0;
                if (self.constellaLevel >= 6)
                {
                    self.mono?.ShowMessage("�ж���ǰ", Color.blue);
                    self.ChangePercentageLocation(1);
                }
            }
            return d;
        };
        self.onTakingDamage.Add(t);
    }

    public override void Mystery(List<Character> characters, List<Enemy> enemies)
    {
        foreach(Character c in characters)
        {
            c.AddBuff("gepardMystery", BuffType.Buff, CommonAttribute.DEF, ValueType.InstantNumber, self.GetBaseAttr(CommonAttribute.DEF) * .25f, 2);
        }
    }

    public override void OnBattleStart(List<Character> characters)
    {
        if (self.constellaLevel >= 4)
        {
            foreach (Character c in characters)
            {
                c.AddBuff("gepardConstellation4ResistUp", BuffType.Permanent, CommonAttribute.EffectResist, ValueType.InstantNumber, .2f);
            }
            self.onDying.Add(new TriggerEvent<Creature.DyingEvent>("gepardConstellation4Die", () =>
            {
                foreach (Character c in characters)
                {
                    c.RemoveBuff("gepardConstellation4ResistUp");
                }
            }));
        }
    }

}