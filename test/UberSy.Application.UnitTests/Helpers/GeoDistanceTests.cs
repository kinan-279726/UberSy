using System;
using System.Collections.Generic;
using System.Text;
using NetTopologySuite.Geometries;
using UberSy.Application.Common.Helpers;
using Xunit;

namespace UberSy.Application.UnitTests.Helpers;

public sealed class GeoDistanceTests
{
    // Point(x: longitude, y: latitude)
    private static Point P(double latitude, double longitude) => new(longitude, latitude);

    [Fact]
    public void HaversineKm_WhenSamePoint_ShouldBeZero()
    {
        var damascus = P(33.5138, 36.2765);

        Assert.Equal(0, GeoDistance.HaversineKm(damascus, damascus), 4);
    }

    [Fact]
    public void HaversineKm_WhenOneDegreeOfLatitude_ShouldBeAbout111Km()
    {
        Assert.Equal(111.19, GeoDistance.HaversineKm(P(0, 0), P(1, 0)), 2);
    }

    [Fact]
    public void HaversineKm_WhenOneDegreeOfLongitudeOnEquator_ShouldBeAbout111Km()
    {
        Assert.Equal(111.19, GeoDistance.HaversineKm(P(0, 0), P(0, 1)), 2);
    }

    [Fact]
    public void HaversineKm_WhenAntipodalPoints_ShouldBeHalfTheEarthCircumference()
    {
        Assert.Equal(20015.09, GeoDistance.HaversineKm(P(0, 0), P(0, 180)), 0);
    }

    [Fact]
    public void HaversineKm_ShouldBeSymmetric()
    {
        var damascus = P(33.5138, 36.2765);
        var aleppo = P(36.2021, 37.1343);

        Assert.Equal(
            GeoDistance.HaversineKm(damascus, aleppo),
            GeoDistance.HaversineKm(aleppo, damascus),
            4);
    }

    [Fact]
    public void HaversineKm_WhenDamascusToAleppo_ShouldBeAbout309Km()
    {
        Assert.Equal(309, GeoDistance.HaversineKm(P(33.5138, 36.2765), P(36.2021, 37.1343)), 0);
    }
}
