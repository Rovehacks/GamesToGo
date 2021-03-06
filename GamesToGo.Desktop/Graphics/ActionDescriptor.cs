﻿using System.Linq;
using GamesToGo.Desktop.Project.Actions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK.Graphics;

namespace GamesToGo.Desktop.Graphics
{
    public class ActionDescriptor : Container
    {
        private readonly EventAction model;
        private FillFlowContainer descriptionContainer;
        private BasicScrollContainer scrollContainer;

        public ActionDescriptor(EventAction model)
        {
            this.model = model;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Red,
                },
                scrollContainer = new BasicScrollContainer(Direction.Horizontal)
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    ScrollbarOverlapsContent = false,
                    Child = descriptionContainer = new FillFlowContainer
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Horizontal,
                    },
                },
            };

            scrollContainer.ScrollContent.RelativeSizeAxes = Axes.None;
            scrollContainer.ScrollContent.AutoSizeAxes = Axes.Both;

            for (int i = 0; i < model.ExpectedArguments.Length; i++)
            {
                descriptionContainer.AddRange(new Drawable[]
                {
                    new SpriteText
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Padding = new MarginPadding(4),
                        Text = model.Text[i],
                        Font = new FontUsage(size: 25),
                    },
                    new ArgumentChanger(model.ExpectedArguments[i], model.Arguments[i]),
                });
            }

            if (model.ExpectedArguments.Length < model.Text.Length)
            {
                descriptionContainer.Add(new SpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Padding = new MarginPadding(4),
                    Text = model.Text.Last(),
                    Font = new FontUsage(size: 25),
                });
            }
        }
    }
}
