using UnityEngine;

public class PuzzleAltar : PuzzleSuper
{
   private int numJewels = 0;


   public int GetJewels()
   {
      return numJewels;
   }

   public void AddJewel()
   {
      numJewels++;
   }

   public override void SetCondition1(bool condition)
   {
      base.SetCondition1(condition);
      if (condition == true)
      {
         numJewels++;
      }
      else
      {
         if (numJewels > 0)
         {
            numJewels--;
         }
      }
   }
}
