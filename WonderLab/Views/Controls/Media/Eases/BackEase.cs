using System;
using Avalonia.Animation.Easings;

namespace WonderLab.Views.Controls.Media.Eases;

public sealed class BackEase : Easing {
    public double Amplitude { get; set; }

    public BackEase() { }

    public BackEase(double amplitude = 0.3d) {
        Amplitude = amplitude;  
    }

    public override double Ease(double progress) {
        if (progress < 0.5d) {
            double f = 2d * progress;
            return 0.5d * f * (f * f - Amplitude * Math.Sin(f * Math.PI));
        } else {
            double f = (1d - (2d * progress - 1d));
            return 0.5d * (1d - f * (f * f - Amplitude * Math.Sin(f * Math.PI))) + 0.5d;
        }
    }
}