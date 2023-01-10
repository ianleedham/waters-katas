//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;

//namespace shipping_costs
//{
//    // You have the raw data for a person's order from your online store. 
//    // Test-drive a function that correctly calculates the minimum shipping costs given the following pricing rules:

//    // We only deliver within the UK, which includes Northern Ireland, the Channel Islands, the Isle of Man and the Scottish islands.
//    // Standard postage costs £4.99 per item and is free for orders totalling over £25 where there are no exceptional items (see below).
//    // First class delivery in the UK: add £2.99 per item on top of regular postage costs.
//    // Next day delivery within the UK: add £11.99 per item to all orders.
//    //      - Only available in the mainland UK.
//    // Large items (any dimension over 30cm): add £19.90 per item; first class is the only available option.
//    // Heavy items (over 15kg): add £39:90 each item, again first class only, and
//    //      - Only available in mainland UK.
//    // First class delivery off the mainland: add £9.00 per item.
//    // Special price for video games: next day add £3.90 per item + £1.50 per kg
//    //      - Only available in mainland UK.
//    // Luxury watches and jewellery are always shipped as individual separate items. Delivery address must be in mainland UK. Add 36.99 per item.
//    //      - Only available in mainland UK.
//    // FYI 


//    [TestClass]
//    public class UnitTests
//    {
        
//        [TestMethod]
//        [DataRow("UK", true, DisplayName = "Supported destination")]
//        [DataRow("France", false, DisplayName = "Unsupported destination")]
//        public void CanDeliverToSupportedDestinations(string destinationCountry, bool isSupported)
//        {
//            var calculator = new PostageCalculator();

//            Assert.AreEqual(isSupported, calculator.IsSupportedDestination(destinationCountry));
//        }

//        [TestMethod]
//        [DataRow("France", "Sorry, we don't deliver to France yet.",DisplayName = "Destination France throws exception")]
//        [DataRow("Germany", "Sorry, we don't deliver to Germany yet.",DisplayName = "Destination Germany throws exception")]
//        public void IfCallingCalculatePostageAndDestinationIsUnsupported_AnExceptionIsThrown(string destination, string message)
//        {
//            var calculator = new PostageCalculator();

//            var exception = Assert.ThrowsException<Exception>(
//                () => calculator.CalculatePostage("standard", 11.00, 1, destination));

//            Assert.IsTrue(exception.Message.Contains(destination));
//            Assert.AreEqual(message, exception.Message);
//        }
        

//        [TestMethod]
//        [DataRow(25.01, 0, DisplayName = "Free postage over 25")]
//        [DataRow(24.99, 4.99, DisplayName = "Standard postage under 25 for one item")]
//        public void StandardPostage(double totalOrderCost, double expectedPostageCost)
//        {
//            var calculator = new PostageCalculator();

//            var postageCost = calculator.CalculatePostage("standard", totalOrderCost, 1);

//            Assert.AreEqual(expectedPostageCost, postageCost);
//        }

//        [TestMethod]
//        public void StandardPostageForTwoItemsUnderFreeThreshold()
//        {
//            var calculator = new PostageCalculator();
//            var totalOrderCost = 24.99;
//            var nItems = 2;

//            var postageCost = calculator.CalculatePostage("standard", totalOrderCost, nItems);

//            Assert.AreEqual(9.98, postageCost);
//        }

//        [TestMethod]
//        [DataRow(25.01, 11.99, 1, DisplayName = "Next Day Over threshold one item")]
//        [DataRow(24.99, 16.98, 1, DisplayName = "Next Day Under threshold one item")]
//        [DataRow(15.25, 33.96, 2, DisplayName = "Next Day Under threshold two items")]
//        [DataRow(25.25, 23.98, 2, DisplayName = "Next Day Over threshold two items")]
//        public void Postage_WithNextDayDelivery(double totalOrderCost, double expectedPostageCost, int numberOfItems)
//        {
//            var calculator = new PostageCalculator();

//            var postageCost = calculator.CalculatePostage("nextdaydelivery", totalOrderCost, numberOfItems);

//            Assert.AreEqual(expectedPostageCost, postageCost);
//        }


//        [TestMethod]
//        [DataRow(23.01, 7.98, 1, DisplayName = "FirstClass Postage For One Item Under Free Threshold")]
//        [DataRow(25.01, 2.99, 1, DisplayName = "FirstClass Postage For One Item Over Free Threshold")]
//        [DataRow(23.01, 15.96, 2, DisplayName = "FirstClass Postage For Two Items Under Free Threshold")]
//        [DataRow(25.01, 5.98, 2, DisplayName = "FirstClass Postage For Two Items Over Free Threshold")]
//        public void Postage_FirstClass(double totalOrderCost, double expectedPostageCost, int numberOfItems)
//        {
//            var calculator = new PostageCalculator();

//            var postageCost = calculator.CalculatePostage("first", totalOrderCost, numberOfItems);

//            Assert.AreEqual(expectedPostageCost, postageCost);
//        }

//    }

//    public class PostageCalculator
//    {
//        private const double FreePostage = 0;
//        private const double StandardPostagePerItem= 4.99;
//        private const double FirstClassPostagePerItem = 2.99;
//        private const double NextDayDeliveryCostPerItem = 11.99;
//        private const string FirstClassPostageName = "first";
//        private const string NextDayDeliveryName = "nextdaydelivery";
//        private const string SupportedCountry = "UK";

//        private const double FreePostageThreshold = 25;

//        //No-one likes four parameters or calculate postage knowing about destination
//        public double CalculatePostage(string deliveryClass, double totalOrderCost, int nItems, string destination = SupportedCountry)
//        {
//            if (destination != SupportedCountry)
//                throw new Exception($"Sorry, we don't deliver to {destination} yet.");

//            if (deliveryClass == FirstClassPostageName)
//            {
//                return CalculateFirstClassPostage(totalOrderCost, nItems);
//            }
//            if (deliveryClass == NextDayDeliveryName)
//            {
//                return CalculateNextDayPostage(totalOrderCost, nItems);
//            }

//            return CalculateStandardPostage(nItems, totalOrderCost);
//        }

//        public bool IsSupportedDestination(string destination) => destination == SupportedCountry;

//        private double CalculateNextDayPostage(double totalOrderCost, int nItems)  
//        {
//            return CalculateStandardPostage(nItems, totalOrderCost) + nItems * NextDayDeliveryCostPerItem;
//        }

//        private double CalculateFirstClassPostage(double totalOrderCost, int nItems)
//        {
//            return CalculateFirstClassSupplement(nItems) + CalculateStandardPostage(nItems, totalOrderCost);
//        }

//        private double CalculateFirstClassSupplement(int nItems) =>  nItems * FirstClassPostagePerItem;

//        private double CalculateStandardPostage(int nItems, double totalOrderCost)
//        {
//            if(IsUnderFreeThreshold(totalOrderCost))
//                return nItems * StandardPostagePerItem;
//            return FreePostage;
//        }

//        private bool IsUnderFreeThreshold(double totalOrderCost) => totalOrderCost <= FreePostageThreshold;
//    }
//}
