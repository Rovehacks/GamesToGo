﻿using System;
using System.Linq;
using GamesToGo.Desktop.Overlays;
using GamesToGo.Desktop.Project;
using GamesToGo.Desktop.Project.Actions;
using GamesToGo.Desktop.Project.Arguments;
using GamesToGo.Desktop.Screens;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace GamesToGo.Desktop.Graphics
{
    public class ActionTypeListing : Container
    {
        private readonly IBindable<ProjectElement> currentEditing = new Bindable<ProjectElement>();
        private ActionListContainer possibleEventsList;

        [Resolved]
        private ProjectEditor editor { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
            Child = new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 50,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Honeydew,
                    },
                    new Container
                    {
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        RelativeSizeAxes = Axes.Y,
                        Width = 100,
                        Child = new AutoSizeButton
                        {
                            RelativeSizeAxes = Axes.Both,
                            Text = @"Añadir acción",
                            Action = toggleAvailableEvents,
                        },
                    },
                    possibleEventsList = new ActionListContainer(),
                },
            };

            currentEditing.BindTo(editor.CurrentEditingElement);
            currentEditing.BindValueChanged(_ => possibleEventsList.Hide(), true);

            // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
            foreach (var type in WorkingProject.AvailableActions.Values)
            {
                var defaultAction = Activator.CreateInstance(type) as EventAction;
                possibleEventsList.AddPossibility(defaultAction);
            }
        }

        private void toggleAvailableEvents()
        {
            possibleEventsList.ToggleVisibility();
        }

        [Cached]
        private class ActionListContainer : VisibilityContainer
        {
            private FillFlowContainer<ActionTypeList> lists;

            protected override bool StartHidden => true;

            [BackgroundDependencyLoader]
            private void load()
            {
                Alpha = 1;
                Anchor = Anchor.TopRight;
                Origin = Anchor.BottomRight;
                AutoSizeAxes = Axes.Y;
                RelativeSizeAxes = Axes.X;
                Child = new Container
                {
                    Masking = true,
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colour4.Black,
                        },
                        lists = new FillFlowContainer<ActionTypeList>
                        {
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Full,
                            RelativeSizeAxes = Axes.X,
                            Spacing = new Vector2(15),
                        },
                    },
                };

                lists.AddRange(new []
                {
                    new ActionTypeList(@"Jugador", ArgumentType.Player),
                    new ActionTypeList(@"Cartas", ArgumentType.Card),
                    new ActionTypeList(@"Casillas", ArgumentType.Tile),
                    new ActionTypeList(@"Fichas", ArgumentType.Token),
                    new ActionTypeList(@"Otros", ArgumentType.Default | ArgumentType.Privacy | ArgumentType.Orientation),
                });
            }

            protected override void PopIn()
            {
                Child.FadeIn();
            }

            protected override void PopOut()
            {
                Child.FadeOut();
            }

            public void AddPossibility(EventAction defaultEvent)
            {
                if (defaultEvent.ExpectedArguments.Length == 0)
                {
                    lists.Last().AddPossibility(defaultEvent);

                    return;
                }

                foreach (var list in lists)
                {
                    var added = ArgumentType.Default;
                    foreach(var arg in defaultEvent.ExpectedArguments)
                    {
                        if ((arg & list.ExpectedType) <= 0 || (added & list.ExpectedType) != 0)
                            continue;

                        list.AddPossibility(defaultEvent);
                        added |= list.ExpectedType;
                    }
                }
            }
        }

        private class ActionTypeList : Container
        {
            private readonly FillFlowContainer possibilitiesList;
            public ArgumentType ExpectedType { get; }

            public ActionTypeList(string title, ArgumentType type)
            {
                ExpectedType = type;
                AutoSizeAxes = Axes.Both;
                InternalChildren = new Drawable[]
                {
                    possibilitiesList = new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Spacing = Vector2.Zero,
                        Children = new Drawable[]
                        {
                            new SpriteText
                            {
                                Font = new FontUsage(weight: "bold", size: 30),
                                Text = title,
                            },
                        },
                    },
                };
            }

            public void AddPossibility(EventAction defaultEvent)
            {
                possibilitiesList.Add(new ActionTypeButton(defaultEvent));
            }
        }

        private class ActionTypeButton : GamesToGoButton
        {
            private readonly Container content = new Container
            {
                AutoSizeAxes = Axes.Both,
            };

            private readonly EventAction type;

            [Resolved]
            private EventEditionOverlay events { get; set; }

            [Resolved]
            private ActionListContainer actionList { get; set; }

            public ActionTypeButton(EventAction type)
            {
                this.type = type;
            }

            protected override Container<Drawable> Content => content;

            [BackgroundDependencyLoader]
            private void load()
            {
                AddInternal(content);
                AutoSizeAxes = Axes.Both;
                BackgroundColour = Colour4.Transparent;
                HoverColour = new Colour4(55, 55, 55, 255);
                Action = () =>
                {
                    events.AddActionToCurrent(Activator.CreateInstance(type.GetType()) as EventAction);
                    actionList.Hide();
                };
                SpriteText.Text = string.Join(' ', type.Text);
            }

            protected override SpriteText CreateText()
            {
                var text = base.CreateText();

                text.Colour = Colour4.White;
                text.Font = new FontUsage(size: 28);
                text.Margin = new MarginPadding { Horizontal = 6.5f };

                return text;
            }
        }
    }
}
