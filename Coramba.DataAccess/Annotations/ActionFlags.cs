using System;

namespace Coramba.DataAccess.Annotations
{
    [Flags]
    public enum ActionFlags
    {
        New = 0x1,
        Insert = 0x2,
        Update = 0x4,
        NewOrInsert = New | Insert,
        InsertOrUpdate = Insert | Update,
        All = New | Insert | Update,
    }
}
