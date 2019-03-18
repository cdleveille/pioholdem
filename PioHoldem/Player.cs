﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PioHoldem
{
    abstract class Player
    {
        public int stack;
        public string name;
        public Card[] holeCards;
        public bool folded;
        public int inForOnCurrentStreet;

        // Create a new Player with the specified name and starting stack amount
        public Player(string name, int startingStack)
        {
            this.name = name;
            stack = startingStack;
            holeCards = new Card[2];
        }

        // Get the Player's action
        public abstract int GetAction(Game game);

        // Increase stack by specified amount
        public void incStack(int amount)
        {
            stack += amount;
        }

        // Decrease stack by specified amount
        public void decStack(int amount)
        {
            if (stack >= amount)
            {
                stack -= amount;
            }
            else
            {
                stack = 0;
            }
        }

        public override string ToString()
        {
            return name + "(" + (stack == 0 ? "ALL IN" : stack.ToString()) + ")";
        }
    }
}
