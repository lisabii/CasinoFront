using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class CasinoWarCardImpl : ICard {
    
    public int suit;
    public int value;
    private int score;
    //private readonly int MAX_SCORE = 10;


    public CasinoWarCardImpl(Suit suit, Value value) {
        this.suit = (int)suit;
        this.value = (int)value;
        this.score = DecideScore();
    }


    private int DecideScore() {

        if (this.value == 1)
        {
            return 14;
        }
        else
        {
            return this.value;
        }
      
    }

    public Suit GetSuit() {
        return (Suit)this.suit;
    }

    public Value GetValue() {
        return (Value)this.value;
    }

    public int GetScore() {
        return this.score;
    }

    public bool Equals(CasinoWarCardImpl card) {
        return card.GetValue().Equals(this.value) && card.GetSuit().Equals(this.suit);
    }

    //public override int CompareTo(object obj) {
    //    PlayingCard anotherCard = (PlayingCard)obj;

    //    //return 1 if the card is greater than the card passed in
    //    if(this.score > anotherCard.GetScore()) {
    //        return 1;
    //    //return -1 if the card is less than the card passed in 
    //    }else if(this.score < anotherCard.GetScore()) {
    //        return -1;
    //    }
    //    //return 0 when both cards are equal
    //    else {
    //        return 0;
    //    }
    //}

    public override string ToString() {
        return (Suit)this.suit + "_" + (Value)this.value;
    }
}
