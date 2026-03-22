using Hoverboard.Factory;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hoverboard.Config
{
    public static class HoverboardConfig
    {
        private static MelonPreferences_Category _category;

        public static MelonPreferences_Entry<float> Price;
        public static MelonPreferences_Entry<float> ResellMultiplier;
        public static MelonPreferences_Entry<float> HoverHeight;
        public static MelonPreferences_Entry<float> TurnRate;
        public static MelonPreferences_Entry<float> MaxBoardLean;
        public static MelonPreferences_Entry<float> BoardLeanRate;
        public static MelonPreferences_Entry<float> Proportional;
        public static MelonPreferences_Entry<float> Integral;
        public static MelonPreferences_Entry<float> Derivative;

        public static void Initialize()
        {
            _category = MelonPreferences.CreateCategory("Hoverboard");
            Price = _category.CreateEntry("Price", 2000f, "(Requires Save Reload) The price of the hoverboard in the in-game shop.");
            ResellMultiplier = _category.CreateEntry("Resell Multiplier", 0.6f, "(Requires Save Reload) The price of the hoverboard in the in-game shop.");

            HoverHeight = _category.CreateEntry("Hover Height", 2.0f, "The height at which the hoverboard hovers above the ground.");
            HoverHeight.OnEntryValueChanged.Subscribe((oldValue, newValue) =>
                HoverboardFactory.hoverSkateboard.HoverHeight = newValue
            );

            TurnRate = _category.CreateEntry("Turn Rate", 2.0f, "The height at which the board hovers above the ground.");
            TurnRate.OnEntryValueChanged.Subscribe((oldValue, newValue) =>
                HoverboardFactory.hoverSkateboard.TurnChangeRate = newValue
            );

            MaxBoardLean = _category.CreateEntry("Max Board Lean", 8f, "The maximum angle the board leans when turning.");
            MaxBoardLean.OnEntryValueChanged.Subscribe((oldValue, newValue) =>
                HoverboardFactory.hoverVisuals.MaxBoardLean = newValue
            );

            BoardLeanRate = _category.CreateEntry("Board Lean Rate", 2f, "How quickly the board leans when turning.");
            BoardLeanRate.OnEntryValueChanged.Subscribe((oldValue, newValue) =>
                HoverboardFactory.hoverVisuals.BoardLeanRate = newValue
            );

            Proportional = _category.CreateEntry("Proportional", 2.7f, "How strongly the board reacts to height errors.\nHigher = Snappier response | Lower = Sluggish response\nRecommend: 2.0 - 2.8");
            Proportional.OnEntryValueChanged.Subscribe((oldValue, newValue) =>
                HoverboardFactory.hoverSkateboard.Hover_P = newValue
            );

            Integral = _category.CreateEntry("Integral", 0.1f, "How much the board corrects over time to reach exact height.\nHigher = Rigid, locked height | Lower = Floaty, drifty feel\nRecommend: 0.05 - 0.2");
            Integral.OnEntryValueChanged.Subscribe((oldValue, newValue) =>
                HoverboardFactory.hoverSkateboard.Hover_I = newValue
            );

            Derivative = _category.CreateEntry("Derivative", 0.5f, "How much the board resists sudden height changes.\nHigher = Smooth over bumps, less bounce | Lower = Bouncy, reactive\nRecommend: 0.3 - 0.6");
            Derivative.OnEntryValueChanged.Subscribe((oldValue, newValue) =>
                HoverboardFactory.hoverSkateboard.Hover_D = newValue
            );


        }
    }

}
