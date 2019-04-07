using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public interface ICard { 

    Suit GetSuit();

    Value GetValue();

    int GetScore();

    bool Equals(CasinoWarCardImpl card);

    //public abstract int CompareTo(object obj);
}


