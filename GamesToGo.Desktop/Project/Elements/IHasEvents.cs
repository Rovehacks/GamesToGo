﻿using System.Text;
using GamesToGo.Desktop.Project.Events;
using osu.Framework.Bindables;

namespace GamesToGo.Desktop.Project.Elements
{
    public interface IHasEvents
    {
        BindableList<ProjectEvent> Events { get; }

        public string ToSaveable()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine($"Events={Events.Count}");
            foreach (var e in Events)
            {
                builder.AppendLine(e.ToString());
            }

            return builder.ToString().TrimEnd('\n', '\r');
        }
    }
}
