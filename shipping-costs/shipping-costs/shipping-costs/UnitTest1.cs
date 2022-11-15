using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace shipping_costs
{
    // You have the raw data for a person's order from your online store. 
    // Test-drive a function that correctly calculates the minimum shipping costs given the following pricing rules:
    
    // We only deliver within the UK, which includes Northern Ireland, the Channel Islands, the Isle of Man and the Scottish islands.
    // Regular postage costs £4.99 per item and is free for orders totalling over £25 where there are no exceptional items (see below).
    // First class delivery in the UK: add £2.99 per item on top of regular postage costs.
    // Next day delivery within the UK: add £11.99 per item to all orders. (This option is not available off the mainland.)
    // Large items (any dimension over 30cm): add £19.90 per item; first class is the only available option.
    // Heavy items (over 15kg): add £39:90 each item, again first class only, and only available in mainland UK.
    // First class delivery off the mainland: add £9.00 per item.
    // Special price for video games: next day add £3.90 per item + £1.50 per kg (mainland UK only).
    // Luxury watches and jewellery are always shipped as individual separate items. Delivery address must be in mainland UK. Only the Next day option is available. Add 36.99 per item.



    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [DataRow(26, 0, DisplayName = "Free postage over 25")]
        [DataRow(24, 4.99, DisplayName = "Standard postage under 25 for one item")]
        public void RegularPostageFreeForOrdersOver25(double totalOrderCost, double expectedPostageCost)
        {
            var calculator = new PostageCalculator();

            var postageCost = calculator.CalculatePostage("regular", totalOrderCost, 1);

            Assert.AreEqual(expectedPostageCost, postageCost);
        }

        [TestMethod]
        public void PostageForOneItem_UnderThreshold_WithNextDayDelivery()
        {
            var calculator = new PostageCalculator();
            var totalOrderCost = 15.25;
            var expectedPostageCost = 11.99 + 4.99;

            var postageCost = calculator.CalculatePostage("nextdaydelivery", totalOrderCost, 1);

            Assert.AreEqual(expectedPostageCost, postageCost);
        }

        [TestMethod]
        public void RegularPostageForTwoItemsUnderFreeThreshold()
        {
            var calculator = new PostageCalculator();
            var totalOrderCost = 24.99;
            var nItems = 2;

            var postageCost = calculator.CalculatePostage("regular", totalOrderCost, nItems);

            Assert.AreEqual(9.98, postageCost);
        }

        [TestMethod]
        [DataRow(23.01, 7.98, DisplayName = "FirstClass Postage For One Item Under Free Threshold")]
        [DataRow(25.01, 2.99, DisplayName = "FirstClass Postage For One Item Over Free Threshold")]
        public void FirstClassPostageForOneItem_FreeThresholds(double totalOrderCost, double expectedPostageCost)
        {
            var calculator = new PostageCalculator();
            var nItems = 1;

            var postageCost = calculator.CalculatePostage("first", totalOrderCost, nItems);

            Assert.AreEqual(expectedPostageCost, postageCost);
        }

        [TestMethod]
        [DataRow(23.01, 15.96, DisplayName = "FirstClass Postage For Two Items Under Free Threshold")]
        [DataRow(25.01, 5.98, DisplayName = "FirstClass Postage For Two Items Over Free Threshold")]
        public void FirstClassPostageForTwoItems_FreeThresholds(double totalOrderCost, double expectedPostageCost)
        {
            var calculator = new PostageCalculator();
            var nItems = 2;

            var postageCost = calculator.CalculatePostage("first", totalOrderCost, nItems);

            Assert.AreEqual(expectedPostageCost, postageCost);
        }
    }

    public class PostageCalculator
    {
        private const double FreePostage = 0;
        private const double StandardPostagePerItem= 4.99;
        private const double FirstClassPostagePerItem = 2.99;
        private const string FirstClassPostageName = "first";

        private const double FreePostageThreshold = 25;

        public double CalculatePostage(string deliveryClass, double totalOrderCost, int nItems)
        {
            if (deliveryClass == FirstClassPostageName)
            {
                return CalculateFirstClassPostage(totalOrderCost, nItems);
            }

            return CalculateStandardPostage(nItems, totalOrderCost);
        }

        private double CalculateFirstClassPostage(double totalOrderCost, int nItems)
        {
            return CalculateFirstClassSupplement(nItems) + CalculateStandardPostage(nItems, totalOrderCost);
        }

        private double CalculateFirstClassSupplement(int nItems) =>  nItems * FirstClassPostagePerItem;

        private double CalculateStandardPostage(int nItems, double totalOrderCost)
        {
            if(IsUnderFreeThreshold(totalOrderCost))
                return nItems * StandardPostagePerItem;
            return FreePostage;
        }

        private bool IsUnderFreeThreshold(double totalOrderCost) => totalOrderCost <= FreePostageThreshold;
    }
}
