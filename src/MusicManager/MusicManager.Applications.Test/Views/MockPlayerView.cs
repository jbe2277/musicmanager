﻿using System.Waf.UnitTesting.Mocks;
using Waf.MusicManager.Applications.Views;

namespace Test.MusicManager.Applications.Views;

public class MockPlayerView : MockView, IPlayerView
{
    public TimeSpan Position { get; set; } = TimeSpan.FromSeconds(33);

    public TimeSpan GetPosition() => Position;

    public void SetPosition(TimeSpan position) => Position = position;
}
