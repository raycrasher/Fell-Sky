using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FellSky.Game.Ships.Parts;
using FellSky.Game.Ships;
using System.Linq;

namespace FellSky.Tests
{
    [TestClass]
    public class ShipTests
    {
        /*
        [TestMethod]
        public void ShipInstallsPartGroupCorrectly()
        {
            var group = CreateTestPartGroup();
            var ship = new Ship();
            ship.InstallPartGroup(group);
            Assert.IsTrue(ship.Hulls.Intersect(group.Hulls).Count() == group.Hulls.Count);
            Assert.IsTrue(ship.Groups.Contains(group));
            Assert.IsTrue(group.Hulls.Select(s => s.Group).Distinct().Count() == 1 && group.Hulls.Select(s => s.Group).Distinct().First() == group);
            Assert.IsTrue(group.Thrusters.Select(s => s.Group).Distinct().Count() == 1 && group.Hulls.Select(s => s.Group).Distinct().First() == group);
        }

        [TestMethod]
        public void ShipUninstallsPartGroupCorrectly()
        {
            var group = CreateTestPartGroup();
            var ship = new Ship();
            ship.InstallPartGroup(group);
            Assert.IsTrue(ship.Hulls.Intersect(group.Hulls).Count() == group.Hulls.Count);
            Assert.IsTrue(ship.Groups.Contains(group));
            ship.RemovePartGroup(group);
            Assert.IsTrue(ship.Hulls.Intersect(group.Hulls).Count() == 0);
            Assert.IsFalse(ship.Groups.Contains(group));
        }
        */
    }
}
