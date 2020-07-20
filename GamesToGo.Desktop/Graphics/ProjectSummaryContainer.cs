﻿using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK.Graphics;
using osuTK;
using GamesToGo.Desktop.Project;
using osu.Framework.Input.Events;
using osu.Framework.Graphics.Containers;
using System;
using osu.Framework.Allocation;
using GamesToGo.Desktop.Overlays;
using osu.Framework.Platform;
using System.IO;
using GamesToGo.Desktop.Database.Models;
using osu.Framework.Graphics.Textures;

namespace GamesToGo.Desktop.Graphics
{
    public class ProjectSummaryContainer : Container
    {
        private const float margin_size = 5;
        private const float main_text_size = 35;
        private const float small_text_size = 20;
        private const float small_height = main_text_size + small_text_size + margin_size * 3;
        private const float expanded_height = main_text_size + small_text_size * 2 + margin_size * 4;
        private Container buttonsContainer;
        private ActionButton deleteButton;
        private ActionButton editButton;
        private MultipleOptionOverlay optionsOverlay;

        public readonly ProjectInfo ProjectInfo;

        private WorkingProject workingProject;

        private IconUsage editIcon;

        public Action<WorkingProject> EditAction { private get; set; }
        public Action<ProjectInfo> DeleteAction { private get; set; }

        public ProjectSummaryContainer(ProjectInfo project)
        {
            ProjectInfo = project;
        }

        [BackgroundDependencyLoader]
        private void load(MultipleOptionOverlay optionsOverlay, Storage store, TextureStore textures, Context database)
        {
            this.optionsOverlay = optionsOverlay;

            workingProject = WorkingProject.Parse(ProjectInfo, store, textures, database);
            if (workingProject == null)
                editIcon = FontAwesome.Solid.ExclamationTriangle;
            else
                editIcon = FontAwesome.Solid.Edit;

            Masking = true;
            CornerRadius = margin_size;
            BorderColour = Color4.DarkGray;
            BorderThickness = 3;
            RelativeSizeAxes = Axes.X;
            Height = small_height;
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = new Color4(55, 55, 55, 255),
                    Alpha = 0.8f,
                },
                new Container
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Padding = new MarginPadding(margin_size),
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(margin_size),
                            AutoSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                new SpriteText
                                {
                                    Font = new FontUsage(size: main_text_size),
                                    Text = ProjectInfo.Name,
                                },
                                new SpriteText
                                {
                                    Font = new FontUsage(size: small_text_size),
                                    Text = $"De StUpIdUsErNaMe27 (Ultima vez editado {ProjectInfo.LastEdited:dd/MM/yyyy HH:mm})",
                                },
                                new FillFlowContainer
                                {
                                    Direction = FillDirection.Horizontal,
                                    Spacing = new Vector2(margin_size),
                                    AutoSizeAxes = Axes.Both,
                                    Children = new Drawable[]
                                    {
                                        new StatText(FontAwesome.Regular.Clone, ProjectInfo.NumberCards),
                                        new StatText(FontAwesome.Solid.Coins, ProjectInfo.NumberTokens),
                                        new StatText(FontAwesome.Solid.ChessBoard, ProjectInfo.NumberBoards),
                                        new StatText(FontAwesome.Regular.Square, ProjectInfo.NumberBoxes),
                                        new StatText(FontAwesome.Solid.Users, $"{ProjectInfo.MinNumberPlayers}{(ProjectInfo.MinNumberPlayers < ProjectInfo.MaxNumberPlayers ? $"-{ProjectInfo.MaxNumberPlayers}" : "")}"),
                                    }
                                }
                            }
                        }
                    },
                },
                buttonsContainer = new Container
                {
                    Padding = new MarginPadding(margin_size),
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    AutoSizeAxes = Axes.Both,
                    Alpha = 0,
                    Children = new Drawable[]
                    {
                        new FillFlowContainer<ActionButton>
                        {
                            AutoSizeAxes = Axes.Both,
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Spacing = new Vector2(margin_size),
                            Direction = FillDirection.Horizontal,
                            Children = new []
                            {
                                deleteButton = new ActionButton
                                {
                                    Icon = FontAwesome.Solid.TrashAlt,
                                    Action = showConfirmation,
                                    ButtonColour = Color4.DarkRed,
                                },
                                editButton = new ActionButton
                                {
                                    Icon = editIcon,
                                    Action = checkValidWorkingProject,
                                    ButtonColour = workingProject == null ? FrameworkColour.YellowDark : FrameworkColour.Green,
                                }
                            }
                        }
                    }
                },
            };
        }

        private void checkValidWorkingProject()
        {
            if (workingProject == null)
            {
                optionsOverlay.Show("Este proyecto no se puede abrir. ¿Qué deseas hacer con el?", new[]
                {
                    new OptionItem
                    {
                        Text = "Eliminarlo",
                        Action = showConfirmation,
                        Type = OptionType.Destructive,
                    },
                    new OptionItem
                    {
                        Text = "Buscarlo en el servidor",
                        Type = OptionType.Additive,
                    },
                    new OptionItem
                    {
                        Text = "Nada",
                        Action = () => { },
                        Type = OptionType.Neutral,
                    }
                });
            }
            else
            {
                EditAction?.Invoke(workingProject);
            }
        }

        private void showConfirmation()
        {
            optionsOverlay.Show($"Seguro que quieres eliminar el proyecto \'{ProjectInfo.Name}\'", new[]
            {
                new OptionItem
                {
                    Text = "¡A la basura!",
                    Action = () => DeleteAction?.Invoke(ProjectInfo),
                    Type = OptionType.Destructive,
                },
                new OptionItem
                {
                    Text = "Mejor me lo quedo",
                    Action = () => { },
                    Type = OptionType.Neutral,
                }
            });
        }

        protected override bool OnHover(HoverEvent e)
        {
            this.ResizeHeightTo(expanded_height, 100, Easing.InQuad);
            buttonsContainer.FadeIn(100, Easing.InQuad)
                .OnComplete(_ =>
                {
                    deleteButton.Enabled.Value = true;
                    editButton.Enabled.Value = true;
                });
            return true;
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            this.ResizeHeightTo(small_height, 100, Easing.InQuad);
            buttonsContainer.FadeOut(100, Easing.InQuad)
                .OnComplete(_ =>
                {
                    deleteButton.Enabled.Value = false;
                    editButton.Enabled.Value = false;
                });
            base.OnHoverLost(e);
        }

        private class StatText : FillFlowContainer
        {
            public StatText(IconUsage icon, string text)
            {
                Direction = FillDirection.Horizontal;
                Spacing = new Vector2(margin_size);
                AutoSizeAxes = Axes.Both;
                Children = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Size = new Vector2(small_text_size),
                        Icon = icon
                    },
                    new SpriteText
                    {
                        Font = new FontUsage(size: small_text_size),
                        Text = text,
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = margin_size,
                    },
                };
            }

            public StatText(IconUsage icon, int count) : this(icon, count.ToString()) { }
        }

        private class ActionButton : Button
        {
            private readonly SpriteIcon icon;
            private readonly Box colourBox;
            private static readonly Color4 base_colour = new Color4(100, 100, 100, 255);
            public ActionButton()
            {
                Masking = true;
                CornerRadius = margin_size;
                Anchor = Anchor.CentreRight;
                Origin = Anchor.CentreRight;
                Size = new Vector2(main_text_size * 1.5f, main_text_size);
                Children = new Drawable[]
                {
                    colourBox = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = base_colour,
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding(margin_size),
                        Child = icon = new SpriteIcon
                        {
                            RelativeSizeAxes = Axes.Both,
                        }
                    }
                };

                Enabled.ValueChanged += e =>
                {
                    if (IsHovered && e.NewValue)
                        fadeToColour();
                };
            }

            public IconUsage Icon { set => icon.Icon = value; }

            public Color4 ButtonColour { get; set; }

            private void fadeToColour()
            {
                colourBox.FadeColour(ButtonColour, 100);
            }

            protected override bool OnHover(HoverEvent e)
            {
                if (Enabled.Value)
                    fadeToColour();
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e)
            {
                colourBox.FadeColour(base_colour, 100);
                base.OnHoverLost(e);
            }
        }
    }
}