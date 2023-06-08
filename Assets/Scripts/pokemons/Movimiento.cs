using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Movimiento
{
   public MovePokemon Base { get; set;}

   public int PP { get; set; }

   public Movimiento(MovePokemon pBase)
   {
        Base = pBase;
        PP = pBase.PP;
   }
}
