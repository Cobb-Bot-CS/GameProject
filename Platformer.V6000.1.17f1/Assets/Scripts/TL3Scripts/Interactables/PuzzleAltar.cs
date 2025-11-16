using UnityEngine;

public class PuzzleAltar : PuzzleSuper
{
   private int numJewels = 0;
   private bool pickedUp1 = false;


   public int GetJewels()
   {
      return numJewels;
   }

   public bool PickedUp1()
   {
      return pickedUp1;
   }

   public override void SetCondition1(bool condition)
   {
      base.SetCondition1(condition);
      if (condition == true)
      {
         if (numJewels > 0)
         {
            numJewels--;
         }
      }
      else
      {
         numJewels++;
      }
      pickedUp1 = true;
   }
}
