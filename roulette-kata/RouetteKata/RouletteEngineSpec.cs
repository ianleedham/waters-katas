using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

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
        [DataRow(1,20, DisplayName = "Customer Win Red Bet, odd in first range")]
        [DataRow(12,20, DisplayName = "Customer Win Red Bet, even in second range")]
        [DataRow(19, 20, DisplayName = "Customer Win Red Bet, odd in third range")]
        [DataRow(30, 20, DisplayName = "Customer Win Red Bet, even in fourth range")]
        [DataRow(2,0, DisplayName = "Customer Lose Red Bet, even in first range")]
        [DataRow(11, 0, DisplayName = "Customer Lose Red Bet, odd in second range")]
        public void CustomerPlacesRedBets(int wheelPositionResult,int expectedWinnings)
        {
            int betAmount = 10;

            RouletteEngine engine = new RouletteEngine(wheelPositionResult);

            IBet redBet = new ColourBet(betAmount,Colour.Red);
            engine.PlaceBet(redBet);

            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(expectedWinnings, winnings);
        }

        [TestMethod]
        [DataRow(2,20, DisplayName = "Customer Win Black Bet, even in first range")]
        [DataRow(1, 0, DisplayName = "Customer Lose Black Bet, odd in first range")]
        [DataRow(0, 0, DisplayName = "Customer Lose Green Bet")]
        public void CustomerPlacesBlackBets(int wheelPositionResult,int expectedWinnings)
        {
            //last test (green) needs breaking out of this bank because its not related
            int betAmount = 10;

            RouletteEngine engine = new RouletteEngine(wheelPositionResult);

            IBet blackBet = new ColourBet(betAmount,Colour.Black);
            engine.PlaceBet(blackBet);

            engine.Spin();

            int winnings = engine.CalculateWinnings();

            Assert.AreEqual(expectedWinnings, winnings);
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
        
        public bool IsEven(int targetWheelPosition) => targetWheelPosition % 2 == 0;
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
            return !IsEven(targetWheelPosition);
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

    public enum Colour
    {
        Green,
        Red,
        Black
    }

    public class ColourBet : Bet
    {
        public override int betMultipler { get => 2; }

        private readonly IEnumerable<int> range2 = Enumerable.Range(11, 8);
        private readonly IEnumerable<int> range4 = Enumerable.Range(29, 8);
        private readonly Colour _colour;

        public ColourBet(int amount, Colour colour)
        {
            Amount = amount;
            _colour = colour;
        }

        public override bool HasWon(int targetWheelPosition) { 
           //deal with green!
            if(_colour == Colour.Red)
                return IsRed(targetWheelPosition);
            return !IsRed(targetWheelPosition);
        }

        private bool IsRed(int targetWheelPosition)
        {
            //check for zero      
            IEnumerable<int> RedIsEvenRange = range2.Concat(range4);

            if (IsEven(targetWheelPosition))
            {
                return RedIsEvenRange.Contains(targetWheelPosition);
            }

            return !(RedIsEvenRange.Contains(targetWheelPosition));
        }

    }
}