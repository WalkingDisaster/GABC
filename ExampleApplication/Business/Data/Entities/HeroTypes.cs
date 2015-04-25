using System;

namespace ExampleApplication.Business.Data.Entities
{
    [Flags]
    public enum HeroTypes
    {
        Robot = 1,
        ExoSuit = 2,
        GeneticallyModified = 4,
        Mutant = 8,
        SuperSoldier = 16,
        SuperSpy = 32,
        Alien = 64,
        SuperGenius = 128,
        ExceptionalHuman = 256
    }
}