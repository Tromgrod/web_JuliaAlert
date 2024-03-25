// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyEnums.cs" company="GalexStudio">
//   Copyright ©  2013
// </copyright>
// <summary>
//   Defines the EditTemplates type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LIB.AdvancedProperties
{
    using System;

    /// <summary>
    /// The CSS class.
    /// </summary>
    public class CssClass
    {
        /// <summary>
        /// The none.
        /// </summary>
        public const string None = "";

        /// <summary>
        /// The wide.
        /// </summary>
        public const string Wide = "input WideLabel";

        /// <summary>
        /// The large.
        /// </summary>
        public const string Large = "input LargeLabel";

        /// <summary>
        /// The mini.
        /// </summary>
        public const string Mini = "input MiniInput";
    }
    
    public enum DbSortMode
    { 
        None,
        Asc,
        Desc
    }

    /// <summary>
    /// The display mode.
    /// </summary>
    [Flags]
    public enum DisplayMode : ulong
    {
        None = 0x00000000,          //00000000000000000000000000000     0 //         Do not display in tools
        Simple = 0x00000001,        //00000000000000000000000000001     1 //         Display in Items Grid
        Advanced = 0x00000002,      //00000000000000000000000000010     2 //         Display in Item details
        Search = 0x00000004,        //00000000000000000000000000100     4 //         Use For Search
        AdvancedEdit = 0x00000008,  //00000000000000000000000000100     8 //         Use Adding new item popup
        Print   = 0x00000010,       //00000000000000000000000001000     16 //        For print
        PrintSearch = 0x00000020,   //00000000000000000000000010000     32 //        For print search criteria
        CSV = 0x00000040,           //00000000000000000000000100000     64 //        For CSV
        Excell = 0x00000080,        //00000000000000000000001000000     128 //       Excell
        FrontEnd = 0x00000100      //00000000000000000000000100000     256 //         Show IN FronEnd Widget
        //      OtherMode                              = 0x00000200,       //00000000000000000001000000000     512 //       Unused mode property
        //      OtherMode                              = 0x00000400,       //00000000000000000010000000000     1024 //      Unused mode property
        //      OtherMode                              = 0x00000800,       //00000000000000000100000000000     2048 //      Unused mode property
        //      OtherMode                              = 0x00001000,       //00000000000000001000000000000     4096 //      Unused mode property
        //      OtherMode                              = 0x00002000,       //00000000000000010000000000000     8192 //      Unused mode property
        //      OtherMode                              = 0x00004000,       //00000000000000100000000000000     16384 //     Unused mode property
        //      OtherMode                              = 0x00008000,       //00000000000001000000000000000     32768 //     Unused mode property
        //      OtherMode                              = 0x00010000,       //00000000000010000000000000000     65536 //     Unused mode property
        //      OtherMode                              = 0x00020000,       //00000000000100000000000000000     131072 //    Unused mode property
        //      OtherMode                              = 0x00040000,       //00000000001000000000000000000     262144 //    Unused mode property
        //      OtherMode                              = 0x00080000,       //00000000010000000000000000000     524288  //   Unused mode property
        //      OtherMode                              = 0x00100000,       //00000000100000000000000000000     1048576 //   Unused mode property
        //      OtherMode                              = 0x00200000,       //00000001000000000000000000000     2097152 //   Unused mode property
        //      OtherMode                              = 0x00400000,       //00000010000000000000000000000     4194304 //   Unused mode property
        //      OtherMode                              = 0x00800000,       //00000100000000000000000000000     8388608 //   Unused mode property
        //      OtherMode                              = 0x01000000,       //00001000000000000000000000000     16777216 //  Unused mode property
        //      OtherMode                              = 0x02000000,       //00010000000000000000000000000     33554432 //  Unused mode property
        //      OtherMode                              = 0x04000000,       //00100000000000000000000000000     67108864 //  Unused mode property
        //      OtherMode                              = 0x08000000,       //01000000000000000000000000000     134217728 // Unused mode property
        //      OtherMode                              = 0x10000000,       //10000000000000000000000000000     268435456 // Unused mode property
    }
}