using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlackJack
{
/*
 * TO DO
 * 
 * fix multiple aces bug
 * add betting system
 * add multiple playing decks
 * configure for multiplayer
 * (configure for online also)
 * SIMULATIONS
 * making compatible for running many thousands sim
 * add threading for the sims
 * add AI to recognize best betting strategy
    */
    class Program
    {
        private static bool playerBust = false;
        private static bool dealerBust = false;
        private static int playersMoney = 100;
        private static int dealersMoney = 100;
        private static int bet = 10;
       static List<Card> playersHand = new List<Card>();
       static List<Card> dealersHand = new List<Card>();
        static void Main(string[] args)
        {
            runGame();
        }
        private static void runGame()
        {
            Deck deck = new Deck();
            List<Card> shuffledDeck = deck.getUnshuffledDeck();
            shuffledDeck.Shuffle();
            dealStartingCards(shuffledDeck);
            while (hit(shuffledDeck) && !checkBust(playersHand))
            {
            }
            revealDealersCard(shuffledDeck);

            while (checkHandValue(dealersHand) < 16 && !playerBust)
                dealerHit(shuffledDeck);

            compareHands();
            Console.ReadKey();
        }
        private static void compareHands()
        {
            if (playerBust)
            {
                playersMoney -= bet;
                dealersMoney += bet;
                Console.WriteLine("You are bust! Play another round? You: ${0} Dealer: ${1}", playersMoney,dealersMoney);
            }
            else if (dealerBust)
            {
                playersMoney += bet*2;
                dealersMoney -= bet*2;
                Console.WriteLine("You win - dealer is bust! Play another round? You: ${0} Dealer: ${1}", playersMoney, dealersMoney);
            }
            else if (checkHandValue(playersHand) > checkHandValue(dealersHand))
            {
                playersMoney += bet*2;
                dealersMoney -= bet*2;
                Console.WriteLine("You win - higher hand value than dealer! Play another round? You: ${0} Dealer: ${1}", playersMoney, dealersMoney);
            }
            else if (checkHandValue(playersHand) < checkHandValue(dealersHand))
            {
                playersMoney -= bet;
                dealersMoney += bet;
                Console.WriteLine("You lose - The dealer's hand outvalues your hand! Play another round? You: ${0} Dealer: ${1}", playersMoney, dealersMoney);
            }
            else if (checkHandValue(playersHand) == checkHandValue(dealersHand))
            {
                Console.WriteLine("You tie with the dealer. Play another round? You: ${0} Dealer: ${1}", playersMoney, dealersMoney);
            }
            else
            {
                Console.WriteLine("some error");
            }
            nextRound();
        }

        private static void nextRound()
        {
            string input = Console.ReadLine().ToLower();
            if(input == "yes")
            {
                dealersHand.Clear();
                playersHand.Clear();
                playerBust = false;
                dealerBust = false;
                runGame();

            }
        }
        private static bool softAce(List<Card> hand)
        {
            if(hand.Sum(i => i.Value>10? 10:i.Value) > 11) // if the hand is more than 11 then the ace cannot be 10 (in compareHands)
            return true;

            else return false;
        }
        private static int checkAces(List<Card> hand)
        {
            int q = hand.Where(c => (c.Value == 1)).ToList().Count;
            return q;
        }

        private static void revealDealersCard(List<Card> deck)
        {
            dealersHand.Add(deck.First());
            Console.WriteLine("Dealer's card: " + deck.First().Suit + " " + deck.First().Value);
            deck.RemoveAt(0);
        }
        private static int softValue(List<Card> hand)
        {
            int val = 0;
            if(hand == playersHand)
            {
                val = playersHand.Sum(i => (i.Value >10)? 10:i.Value);
            }
            return val;
        }

        private static int checkHandValue(List<Card> hand)
        {
            int val = 0;
            if (hand == playersHand)
            {
                val = playersHand.Sum(i => {
                    if (i.Value == 1 && !softAce(playersHand) && checkAces(playersHand) == 1)
                        return 11;
                    else if (i.Value > 10)
                        return 10;
                    else return i.Value;
                });

                if(checkAces(playersHand) > 1)
                {
                    val = softValue(playersHand) + 10; //look for something less forcing, what if there were multiple decks? fix this
                }
            }
            

            if (hand == dealersHand)
            {
                val = dealersHand.Sum(i => i.Value > 10 ? 10 : i.Value);
            }
            return val;
        }
        private static bool hit(List<Card> deck)
        {
            Console.WriteLine("hit or stick?");
           String input = Console.ReadLine().ToLower();
            if (checkBust(playersHand))
            {
                return false;
            }
            if (input == "hit"  || input == "h")
            {
               
                playersHand.Add(deck.First());
                Console.WriteLine("You hit: "+deck.First().Suit + " " + deck.First().Value);
                deck.RemoveAt(0);
                if (checkBust(playersHand))
                {
                    return false;
                }
                return true;
            }
           
           else return false;
        }
        private static void dealerHit(List<Card> deck) {
            dealersHand.Add(deck.First());
            Console.WriteLine("Dealer hits: " + deck.First().Suit + " " + deck.First().Value);
            deck.RemoveAt(0);
            checkBust(dealersHand);
        }
        private static Boolean checkBust(List<Card> hand)
        {
            if (checkHandValue(hand) > 21)
            {
                if(hand == playersHand)
                {
                    playerBust = true;
                }
                if (hand == dealersHand)
                {
                    dealerBust = true;
                }
                return true;
            }
            else return false;
        }

        private static void dealStartingCards(List<Card> deck)
        {
            playersHand.Add(deck.First());
            Console.WriteLine("Your card: " + deck.First().Suit + " "+deck.First().Value);
            deck.RemoveAt(0);

            dealersHand.Add(deck.First());
            Console.WriteLine("Dealer's card: "+ deck.First().Suit + " " + deck.First().Value);
            deck.RemoveAt(0);

            playersHand.Add(deck.First());
            Console.WriteLine("Your card: "+deck.First().Suit + " " + deck.First().Value);
            deck.RemoveAt(0);

        }
    }
    class Card
    {
        public int Value
        {
            get; private set;
        }
        public enum suitType
        {
            clubs, spades, hearts, diamonds
        }
        public suitType Suit
        {
            get; private set;
        }
        public Card(int value, suitType suit)
        {
            Value = value;
            Suit = suit;
        }
    }

    class Deck
    {

         List<Card> unshuffledDeck = new List<Card>();

        public Deck()
        {
           
            for (int j = 1; j < 14; j++)
            {
                unshuffledDeck.Add(new Card(j, Card.suitType.spades));
                unshuffledDeck.Add(new Card(j, Card.suitType.diamonds));
                unshuffledDeck.Add(new Card(j, Card.suitType.clubs));
                unshuffledDeck.Add(new Card(j, Card.suitType.hearts));
            }


        }

        public List<Card> getUnshuffledDeck()
        {
            return unshuffledDeck;
        }

    }
    public static class ThreadSafeRandom
    {
        [ThreadStatic]
        private static Random Local;

        public static Random ThisThreadsRandom
        {
            get { return Local ?? (Local = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }
    }
    static class MyExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = ThreadSafeRandom.ThisThreadsRandom.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }

}
