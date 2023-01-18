using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace RouletteKata
{
    /*
    * Basic roulette walkthrough
    * 
    * 1. Customer places bet(s)
    * 2. Croupier spins wheel
    * 3. Winnings calculated
    * 4. Customer optionally cashes in, or back to (1)
    * 
    * Rules
    * 
    * Roulette range 0 - 36
    * In number ranges from 1 to 10 and 19 to 28, odd numbers are red and even are black. In ranges from 11 to 18 and 29 to 36, odd numbers are black and even are red.
    * 0 is coloured green.
    * 
    * Single number bet (straight) (35 to 1)
    * OddNumber Bet         (1 to 1)
    * 
    * More at: https://en.wikipedia.org/wiki/Roulette
    * 
    */
    [TestClass]
    public class RouletteEngineSpec
    {

        [TestMethod]
        [DataRow(10, 360, DisplayName = "bet 10 expect 360")]
        [DataRow(5, 180, DisplayName = "bet 5 expect 180")]
        public void CustomerCanWinAStraightBet(int betAmount, int expectedWinnings)
        {
            int wheelPositionOne = 1;

            RouletteEngine engine = new RouletteEngine(wheelPositionOne);

            engine.PlaceBet(new StraightBet(wheelPositionOne, betAmount));

            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(expectedWinnings, winnings);
        }

        [TestMethod]
        public void CustomerCanLoseAStraightBet()
        {
            int wheelPositionResult = 1;
            int wheelPositionBet = 2;
            int betAmount = 10;

            RouletteEngine engine = new RouletteEngine(wheelPositionResult);

            var straightBet = new StraightBet(wheelPositionBet, betAmount);
            engine.PlaceBet(straightBet);
            
            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(0, winnings);
        }

        [TestMethod]
        public void CustomerCanWinAnOddBet()
        {
            int wheelPositionResult = 1;
            int wheelPositionBet = 1;
            int betAmount = 10;

            RouletteEngine engine = new RouletteEngine(wheelPositionResult);

            IBet oddBet = new OddBet(betAmount);
            engine.PlaceBet(oddBet);

            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(20, winnings);
        }

        [TestMethod]
        public void CustomerStartsWithNoWinningsBeforeBetting()
        {
            RouletteEngine engine = new RouletteEngine(1);

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(0, winnings);
        }

        [TestMethod]
        public void CustomerCanPlaceMixedWinOrLoseStraightBets()
        {
            int wheelPositionBetOne = 1;
            int wheelPositionBetTwo = 1;
            int wheelPositionBetThree = 2;
            int betAmount_1  =  10;
            int betAmount_2 = 20;

            RouletteEngine engine = new RouletteEngine(wheelPositionBetOne);
            
            engine.PlaceBet(new StraightBet(wheelPositionBetOne, betAmount_1));
            engine.PlaceBet(new StraightBet(wheelPositionBetTwo, betAmount_2));
            engine.PlaceBet(new StraightBet(wheelPositionBetThree, betAmount_2));

            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(1080, winnings);
        }

        [TestMethod]
        public void CustomerCanWinAnEvenBet()
        {
            int wheelPositionResult = 2;
            int betAmount = 15;

            RouletteEngine engine = new RouletteEngine(wheelPositionResult);

            IBet evenBet = new EvenBet(betAmount);
            engine.PlaceBet(evenBet);

            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(30, winnings);
        }

        [TestMethod]
        [DataRow(typeof(EvenBet), 1, DisplayName = "Customer Lose Even Bet")]
        [DataRow(typeof(OddBet), 2, DisplayName = "Customer Lose Odd Bet")]
        public void CustomerCanLoseBet(Type betType, int wheelPos)
        {
            int betAmount = 10;

            RouletteEngine engine = new RouletteEngine(wheelPos);

            IBet bet = (IBet)Activator.CreateInstance(betType, betAmount);

            engine.PlaceBet(bet);

            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(0, winnings);
        }

        [TestMethod]
        public void CustomerCanLoseBetsByLandingOn0()
        {
            int wheelPositionResult = 0;
            int betAmount = 15;

            RouletteEngine engine = new RouletteEngine(wheelPositionResult);

            IBet evenBet = new EvenBet(betAmount);
            engine.PlaceBet(evenBet);

            IBet oddBet = new OddBet(betAmount);
            engine.PlaceBet(oddBet);

            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(0, winnings);
        }

        // * In number ranges from 1 to 10 and 19 to 28, odd numbers are red and even are black.In ranges from 11 to 18 and 29 to 36, odd numbers are black and even are red.
        // * 0 is coloured green.
        [TestMethod]
        public void CustomerCanWinWithRedBet()
        {
            int wheelPositionResult = 1;
            int betAmount = 10;

            RouletteEngine engine = new RouletteEngine(wheelPositionResult);

            IBet redBet = new ColourBet(betAmount,"Red");
            engine.PlaceBet(redBet);

            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(20, winnings);
        }

        [TestMethod]
        public void CustomerCanLoseRedBet()
        {
            int wheelPositionResult = 2;
            int betAmount = 10;

            RouletteEngine engine = new RouletteEngine(wheelPositionResult);

            IBet redBet = new ColourBet(betAmount, "Red");
            engine.PlaceBet(redBet);

            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(0, winnings);
        }
    }

    public class RouletteEngine
    {
        private int targetWheelPosition;
        private int winnings = 0;
        private readonly List<IBet> _bets = new List<IBet>();

        public RouletteEngine(int targetWheelPosition)
        {
            this.targetWheelPosition = targetWheelPosition;
        }

        public void PlaceBet(IBet bet)
        {
            _bets.Add(bet);
        }

        public void Spin()
        {
            foreach (var bet in _bets)
            {
                if (bet.HasWon(targetWheelPosition))
                {
                    winnings += bet.Amount * bet.betMultipler;
                }
            }
        }

        public int CalculateWinnings()
        {
            return winnings;
        }
    }

    public interface IBet
    {
        bool HasWon(int targetWheelPosition);
        public int Amount { get; set;  }
        int betMultipler { get; }
    }

    public abstract class Bet: IBet
    {

        public int Amount { get; set; }
        public abstract int betMultipler { get; }

        public abstract bool HasWon(int targetWheelPosition);
    }

    public class EvenBet : Bet
    {
        public EvenBet(int betAmount)
        {
            Amount = betAmount;
        }

        public override int betMultipler => 2;

        public override bool HasWon(int targetWheelPosition)
        {
            return targetWheelPosition != 0 && IsEven(targetWheelPosition);
        }

        public static bool IsEven(int targetWheelPosition) => targetWheelPosition % 2 == 0; // move to base class Bet
    }

    public class OddBet : Bet
    {
        public OddBet(int betAmount)
        {
            Amount = betAmount;
        }

        public override int betMultipler => 2;

        public override bool HasWon(int targetWheelPosition)
        {
            return !EvenBet.IsEven(targetWheelPosition);
        }
    }

    public class StraightBet: Bet
    {
        public override int betMultipler { get => 36; }
        private int betNumber;

        public StraightBet(int betNumber, int amount)
        {
            Amount = amount;
            this.betNumber = betNumber;
        }

        public override bool HasWon(int targetWheelPosition) =>  betNumber == targetWheelPosition;
        
    }

    public class ColourBet : Bet
    {
        public override int betMultipler { get => 2; }

        public ColourBet(int amount, string colour)
        {
            Amount = amount;
        }

        public override bool HasWon(int targetWheelPosition) { return IsRed(targetWheelPosition); }

        private bool IsRed(int targetWheelPosition) => targetWheelPosition == 1; // refactor this logic
    }
}