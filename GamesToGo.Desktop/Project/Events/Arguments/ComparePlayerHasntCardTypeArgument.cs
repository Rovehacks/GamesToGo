﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GamesToGo.Desktop.Project.Events.Arguments
{
    public class ComparePlayerHasntCardTypeArgument : Argument
    {
        public override int ArgumentTypeID => 11;

        public override ArgumentType Type => ArgumentType.Comparison;

        public override bool HasResult => false;

        public override ArgumentType[] ExpectedArguments { get; } = {
            ArgumentType.SinglePlayer,
            ArgumentType.CardType
        };

        public override string[] Text { get; } = {
            @"si",
            @"no tiene cartas de tipo"
        };
    }
}
