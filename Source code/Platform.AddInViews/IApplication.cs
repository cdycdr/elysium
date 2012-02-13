using System;
using System.AddIn.Pipeline;
using System.Windows;
using System.Windows.Media;

using Elysium.Platform.Contracts.Helpers;
using Elysium.Theme;

namespace Elysium.Platform
{
    [AddInBase]
    public interface IApplication
    {
        Info Info { get; }

        Color AccentColor { get; set; }

        ThemeType Theme { get; set; }

        bool IsAttachable { get; }

        bool IsAttached { get; set; }

        bool IsVisible { get; set; }

        void Suspend();

        void Resume();

        Action Close { get; set; }

        FrameworkElement Visual { get; }
    }
} ;