//Sound And Music Types
public interface SFX
{
    public enum Background
    {
        HeartBeat, FirstLocationBackground, SecondLocationBackground,
        ReceivingAcidDamage, ReceivingFireDamage, None
    }
    public enum Environment { Waterfall, River, OrbNoise, Roots, None }
    public enum Universal { Idle, ReceivingDamage, Parry, Death }
    public enum NoRage { FirstAttack, SecondAttack, ThirdAttack, Dasch, Slash, RageDeactivate }
    public enum PassiveRage { FirstAttack, SecondAttack, ThirdAttack, Dasch, Slash }
    public enum ActiveRage { FirstAttack, SecondAttack, ThirdAttack, Dasch, Slash, RageActivate }

    public enum Oni { Attack, ReceivingDamage, TalkOne, TalkTwo, TalkThree, Death, Dasch }
    public enum BigOni { Attack, ReceivingDamage, TalkOne, TalkTwo, TalkThree, Death, Dasch }
    public enum Yurei { Attack, ReceivingDamage, TalkOne, TalkTwo, TalkThree, Death }
    public enum Orb { Idle, ReceivingDamage, Destoyed, RootsDying }
    public enum FinalBoss
    {
        AppearanceIntro, Scream, AttackSweepOne, AttackSweepTwo,
        CenterSmash, SlamLeftHandArea, SlamRightHandArea, SmashOne,
        SmashTwo, SummonOne, SummonTwo, ArmReceivingDamage, HeadReceivingDamage,
        FallDown, Death
    }
}

public interface Music
{
    //I'm aware that music samples naiming isn't really great, but it made some sense on the moment.
    public enum MusicSamples
    {
        ML0Start, ML0p1, ML0p2,
        ML1p1, ML1p2,
        ML2Start, ML2p1, ML2p2, ML2Death1, ML2Death2, ML2End,
        ML3Start, ML3p1, ML3p2, ML3Death1, ML3Death2,
        ML4Start, ML4p1_1, ML4p1_2, ML4p2_1, ML4p2_2, ML4End,
        ML5Fin,
        ML1End_p1, ML1End_p2, Menu,
        ML3bS1, ML3bS2, ML3b1, ML3b2, ML3bD1, ML3bD2, ML3e1, ML3e2,
        Menu1, Menu2, ML1Death1, ML1Death2
    }

    public enum Effects { tention01 }
}
