﻿using System;
using System.Collections.Generic;
using System.Text;
using GamesToGo.Desktop.Database.Models;

namespace GamesToGo.Desktop.Project
{
    public class ProjectInfo
    {
        public int LocalProjectID { get; set; }

        public int CreatorID { get; set; }

        public string Name { get; set; }

        public int MinNumberPlayers { get; set; }

        public int MaxNumberPlayers { get; set; }

        public int NumberCards { get; set; }

        public int NumberTokens { get; set; }

        public int NumberBoxes { get; set; }

        public int OnlineProjectID { get; set; }

        public int ModerationStatus { get; set; }

        public int ComunityStatus { get; set; }

        public DateTime LastEdited { get; set; }

        public File File { get; set; }

        public int FileID { get; set; }

        public ICollection<FileRelation> Relations { get; set; }
    }
}