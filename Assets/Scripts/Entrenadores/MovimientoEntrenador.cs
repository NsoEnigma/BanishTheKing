using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovimientoEntrenador 
{
   public MoveEntrenador Base { get; set;}

   public int PP { get; set; }

   public MovimientoEntrenador(MoveEntrenador pBase)
   {
        Base = pBase;
        PP = pBase.PP;
   }
}
