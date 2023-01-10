using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace shipping_costs
{
    // You have the raw data for a person's order from your online store. 
    // Test-drive a function that correctly calculates the minimum shipping costs given the following pricing rules:

    // We only deliver within the UK, which includes Northern Ireland, the Channel Islands, the Isle of Man and the Scottish islands.
    // Standard postage costs £4.99 per item and is free for orders totalling over £25 where there are no exceptional items (see below).
    // First class delivery in the UK: add £2.99 per item on top of regular postage costs.
    // First class delivery off the mainland: add £9.00 per item.
    // Next day delivery within the UK: add £11.99 per item to all orders.
    //      - Only available in the mainland UK.
    // Large items (any dimension over 30cm): add £19.90 per item; first class is the only available option.
    // Heavy items (over 15kg): add £39:90 each item, again first class only, and
    //      - Only available in mainland UK.
    // Special price for video games: next day add £3.90 per item + £1.50 per kg
    //      - Only available in mainland UK.
    // Luxury watches and jewellery are always shipped as individual separate items. Delivery address must be in mainland UK. Add 36.99 per item.
    //      - Only available in mainland UK.
    // FYI - We agreed mainland = you can drive there


    [TestClass]
    public class UnitTests
    {
        private PostageCalculator calculator;

        [TestInitialize]
        public void Setup()
        {
             calculator = new PostageCalculator();
        }

        [TestMethod]
        [DataRow("UK", true, DisplayName = "Supported destination")]
        [DataRow("France", false, DisplayName = "Unsupported destination")]
        public void CanDeliverToSupportedDestinations(string destinationCountry, bool isSupported)
        {
            var destination = new Destination(destinationCountry);

            Assert.AreEqual(isSupported, destination.IsDeliverable);
        }

        [TestMethod]
        [DataRow("France", "Sorry, we don't deliver to France yet.",DisplayName = "Destination France throws exception")]
        [DataRow("Germany", "Sorry, we don't deliver to Germany yet.",DisplayName = "Destination Germany throws exception")]
        public void IfCallingCalculatePostageAndDestinationIsUnsupported_AnExceptionIsThrown(string destinationCountry, string message)
        {
            var destination = new Destination(destinationCountry);
            var exception = Assert.ThrowsException<Exception>(
                () => CalculatePostageTemporary("standard", 11.00, 1, destination));

            Assert.IsTrue(exception.Message.Contains(destinationCountry));
            Assert.AreEqual(message, exception.Message);
        }

        //also - should allow Standard postage to offshore mainland??
        [TestMethod]
        [DataRow("Channel Islands", DisplayName = "Channel Islands")]
        [DataRow("Isle of Man", DisplayName = "Isle of Man")]
        public void Invalid_Destination_ForNextDayDelivery_Then_ExceptionIsThrown(string destination)
        {
            var destinationObj = new Destination("UK", destination);
            var exception = Assert.ThrowsException<Exception>(
                () => CalculatePostageTemporary("nextdaydelivery", 11.00, 1, destinationObj));

            Assert.AreEqual($"Cannot do Next Day Delivery to {destination}", exception.Message);
        }

        [TestMethod]
        [DataRow(25.01, 0, DisplayName = "Free postage over 25")]
        [DataRow(24.99, 4.99, DisplayName = "Standard postage under 25 for one item")]
        public void StandardPostage(double totalOrderCost, double expectedPostageCost)
        {
            var postageCost = CalculatePostageTemporary("standard", totalOrderCost, 1, new Destination());

            Assert.AreEqual(expectedPostageCost, postageCost);
        }

        [TestMethod]
        public void StandardPostageForTwoItemsUnderFreeThreshold()
        {
            var totalOrderCost = 24.99;
            var nItems = 2;

            var postageCost = CalculatePostageTemporary("standard", totalOrderCost, nItems, new Destination());

            Assert.AreEqual(9.98, postageCost);
        }

        [TestMethod]
        [DataRow(25.01, 11.99, 1, DisplayName = "Next Day Over threshold one item")]
        [DataRow(24.99, 16.98, 1, DisplayName = "Next Day Under threshold one item")]
        [DataRow(15.25, 33.96, 2, DisplayName = "Next Day Under threshold two items")]
        [DataRow(25.25, 23.98, 2, DisplayName = "Next Day Over threshold two items")]
        public void Postage_WithNextDayDelivery(double totalOrderCost, double expectedPostageCost, int numberOfItems)
        {
            var postageCost = CalculatePostageTemporary("nextdaydelivery",  totalOrderCost,  numberOfItems, new Destination());

            Assert.AreEqual(expectedPostageCost, postageCost);
        }


        [TestMethod]
        [DataRow(23.01, 7.98, 1, DisplayName = "FirstClass Postage For One Item Under Free Threshold")]
        [DataRow(25.01, 2.99, 1, DisplayName = "FirstClass Postage For One Item Over Free Threshold")]
        [DataRow(23.01, 15.96, 2, DisplayName = "FirstClass Postage For Two Items Under Free Threshold")]
        [DataRow(25.01, 5.98, 2, DisplayName = "FirstClass Postage For Two Items Over Free Threshold")]
        public void Postage_FirstClass(double totalOrderCost, double expectedPostageCost, int numberOfItems)
        {
            var postageCost = CalculatePostageTemporary("first", totalOrderCost, numberOfItems, new Destination());

            Assert.AreEqual(expectedPostageCost, postageCost);
        }

        private double CalculatePostageTemporary(string postageClass, double totalOrderCost, int nItems, Destination destinationObj)
        {
            return calculator.CalculatePostage(CreateListOfDefaultItems(nItems), postageClass, totalOrderCost, destinationObj);
        }

        private List<Item> CreateListOfDefaultItems(int number)
        {
            List<Item> items = new List<Item>();

            for (int i = 0; i < number; i++)
            {
                Item item = new Item();
                item.Size = 25;
                items.Add(item);
            }

            return items;
        }

        // Large items (any dimension over 30cm): add £19.90 per item; first class is the only available option.
        [TestMethod]
        public void ParcelsOver30cmAreCharged19Point90Surcharge()
        {
            Item item = new Item();
            item.Size = 31;

            List<Item> items = new List<Item>();
            items.Add(item);

            var totalOrderCost = 24.99;
            var postageCost = calculator.CalculatePostage(items, "first", totalOrderCost, new Destination());

            Assert.AreEqual(27.88, postageCost);
        }

        [TestMethod]
        public void WhenPostingMultipleItems_ViaFirstClass_WhereOneItemIsLargeSize()
        {
            List<Item> items = new List<Item>();

            items.Add(new Item { Size = 24 });
            items.Add(new Item { Size = 31 });

            var totalOrderCost = 24.99;
            var postageCost = calculator.CalculatePostage(items, "first", totalOrderCost, new Destination());

            double expectedCost = 27.88 + 2.99 + 4.99;

            Assert.AreEqual(expectedCost, postageCost);
        }

        // Heavy items (over 15kg): add £39:90 each item, again first class only
        [TestMethod]
        public void WhenPostingAHeavyItem_ViaFirstClass_Costs47Point88()
        {
            List<Item> items = new List<Item>();

            items.Add(new Item { Size = 24, Weight = 15.01 });

            var totalOrderCost = 24.99;
            var postageCost = calculator.CalculatePostage(items, "first", totalOrderCost, new Destination());

            double expectedCost = 47.88;

            Assert.AreEqual(expectedCost, postageCost);
        }

        [TestMethod]
        public void WhenPostingMultipleItems__BothUnderAndOverWeightAndSize()
        {
            List<Item> items = new List<Item>();

            items.Add(new Item { Size = 31, Weight = 14.99 });
            items.Add(new Item { Size = 31, Weight = 15.01 });
            items.Add(new Item { Size = 24, Weight = 14.99 });
            items.Add(new Item { Size = 24, Weight = 15.01 });

            var totalOrderCost = 24.99;
            var postageCost = calculator.CalculatePostage(items, "first", totalOrderCost, new Destination());

            double costPerItem = PostageCalculator.StandardPostagePerItem + PostageCalculator.FirstClassPostagePerItem;

            double expectedCost =
                costPerItem + PostageCalculator.LargeItemSurcharge
                + costPerItem + PostageCalculator.LargeItemSurcharge + PostageCalculator.HeavyItemSurcharge                
                + costPerItem
                + costPerItem + PostageCalculator.HeavyItemSurcharge;

            Assert.AreEqual(expectedCost, postageCost);
        }
    }

    public class Item
    {
        public int Size { get; internal set; }
        public double Weight { get; internal set; }
    }

    public class PostageCalculator
    {
        public const double StandardPostagePerItem = 4.99;
        public const double FirstClassPostagePerItem = 2.99;
        private const double FreePostage = 0;
        public const double NextDayDeliveryCostPerItem = 11.99;
        private const string FirstClassPostageName = "first";
        private const string NextDayDeliveryName = "nextdaydelivery";
        private const double FreePostageThreshold = 25;
        private const int SizeLimit = 30;
        private const int WeightLimit = 15;
        public const double LargeItemSurcharge = 19.90;
        public const double HeavyItemSurcharge = 39.90;

        //No-one likes four parameters or calculate postage knowing about destination

        public double CalculatePostage(List<Item> items, string deliveryClass, double totalOrderCost, Destination destination)
        {
            double unroundedCost = CalculatePostageUnrounded(items, deliveryClass, totalOrderCost, destination);
            return Math.Round(unroundedCost, 2, MidpointRounding.AwayFromZero);
        }

        private double CalculatePostageUnrounded(List<Item> items, string deliveryClass, double totalOrderCost, Destination destination)
        {
            int nItems = items.Count;
            CheckDeliveryValidity(deliveryClass, destination);

            if (deliveryClass == FirstClassPostageName)
            {
                return CalculateFirstClassPostage(totalOrderCost, nItems) + CalculateItemSurcharge(items);
            }

            if (deliveryClass == NextDayDeliveryName)
            {
                return CalculateNextDayPostage(totalOrderCost, nItems);
            }

            return CalculateStandardPostage(nItems, totalOrderCost);
        }

        private double CalculateItemSurcharge(List<Item> items)
        {
            double surchargeTotal = 0d;

            foreach (var item in items) 
            {
                surchargeTotal += item.Size > SizeLimit ? LargeItemSurcharge : 0d;
                surchargeTotal += item.Weight > WeightLimit ? HeavyItemSurcharge : 0d;
            }

            return Math.Round(surchargeTotal, 2, MidpointRounding.AwayFromZero);
        }

        public void CheckDeliveryValidity(string deliveryClass, Destination destination)
        {
            if (deliveryClass == NextDayDeliveryName && CanDoNextDayDelivery(destination))
                throw new Exception($"Cannot do Next Day Delivery to {destination.Location}");

            if (!destination.IsDeliverable)
                throw new Exception($"Sorry, we don't deliver to {destination.Country} yet.");
        }

        private bool CanDoNextDayDelivery(Destination destination) => destination.IsDeliverable && !destination.IsMainland;

        private double CalculateNextDayPostage(double totalOrderCost, int nItems)  
        {
            return CalculateStandardPostage(nItems, totalOrderCost) + nItems * NextDayDeliveryCostPerItem;
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

    public class Destination
    {
        private string _country = "UK";
        private string _location;
        public bool IsDeliverable = true;
        public bool IsMainland = true;
        private const string _supportedCountry = "UK";
        private readonly List<string> NonMainlandUK = new()
        {
            "Channel Islands",
            "Isle of Man"
        };


        public Destination()
        {
        }
        
        public Destination(string country, string location)
        {
            Country = country;
            Location = location;
        }
        public Destination(string country)
        {
            Country = country;
        }

        public string Country 
        {
            get => _country;
            set 
            { 
                _country = value;
                IsDeliverable = _country == _supportedCountry;
            }
        }

        public string Location 
        {
            get => _location; 
            set
            {
                _location = value;
                IsMainland = !NonMainlandUK.Contains(_location) && IsDeliverable;
            }
        }

    }
}
