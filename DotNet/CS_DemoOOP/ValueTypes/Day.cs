using System;

namespace CS_DemoOOP.ValueTypes
{
    [Flags] // This attribute allows the enum to be treated as a bit field (for combining values)
    public enum Day
    {
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday
    }
}