﻿//=========       Copyright © Reperio Studios 2013-2017 @ Bernt Andreas Eide!       ============//
//
// Purpose: Event Definitions
//
//=============================================================================================//

using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Workshopper.Core
{
    public class UGCCreationEventArg : EventArgs
    {
        public PublishedFileId_t FileID { get; private set; }
        public UGCCreationEventArg(PublishedFileId_t workshopItemID)
        {
            this.FileID = workshopItemID;
        }
    }
}