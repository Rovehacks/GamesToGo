﻿using System.Collections.Generic;
using GamesToGo.Desktop.Project.Events;
using osu.Framework.Bindables;
using osuTK;

namespace GamesToGo.Desktop.Project.Elements
{
    public class Tile : ProjectElement, IHasSize, IHasEvents
    {
        public override ElementType Type => ElementType.Tile;

        public override Bindable<string> Name { get; } = new Bindable<string>(@"Nueva casilla");

        public override Bindable<string> Description { get; } = new Bindable<string>(@"¡Describe esta casilla para poder identificarla mejor!");

        protected override string DefaultImageName => @"Tile";

        public override Dictionary<string, Bindable<Image>> Images { get; } = new Dictionary<string, Bindable<Image>>(new[]
        {
            new KeyValuePair<string, Bindable<Image>>(@"Frente", new Bindable<Image>()),
        });

        public BindableList<ProjectEvent> Events { get; } = new BindableList<ProjectEvent>();

        public Bindable<Vector2> Size { get; } = new Bindable<Vector2>(new Vector2(400));
    }
}
