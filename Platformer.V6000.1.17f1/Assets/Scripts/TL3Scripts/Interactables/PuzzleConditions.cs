// private data class
using UnityEngine;

public class PuzzleConditions
{
   private bool condition1 = false;
   private bool condition2 = false;
   private bool condition3 = false;

   public bool ConditionsMet()
   {
      if (condition1 && condition2 && condition3)
      {
         return true;
      }
      else
      {
         return false;
      }
   }

   public void SetCondition1(bool condition)
   {
      condition1 = condition;
   }

   public void SetCondition2(bool condition)
   {
      condition2 = condition;
   }

   public void SetCondition3(bool condition)
   {
      condition3 = condition;
   }


}
