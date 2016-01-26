//////////////////////////////////////////////////////////////////////////////////////////
// Every Day Space Station
// http://everydayspacestation.tumblr.com
//////////////////////////////////////////////////////////////////////////////////////////
// EntityTransitionVariables - A large enum of predefined variable types that map to JSON data to be used for configurable runtime 
// entity variations. Using this as a choice to cut down on the potential excessive Reflection that'd otherwise be necessary
// http://www.csharp-examples.net/string-to-enum/ for reference
// Created: December 18 2015
// CasualSimpleton <casualsimpleton@gmail.com>
// Last Modified: December 18 2015
// CasualSimpleton
//////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

using EveryDaySpaceStation;
using EveryDaySpaceStation.DataTypes;
using EveryDaySpaceStation.Utils;

namespace EveryDaySpaceStation
{
    public enum EntityTransitionVariables : ushort
    {
        IsLocked = 0,
        IsPowered = 1,
        IsWelded = 2,
        IsPanelOpen = 3,
        NextAction = 4,
        GDuration = 5,
        LDuration = 6
    }
}