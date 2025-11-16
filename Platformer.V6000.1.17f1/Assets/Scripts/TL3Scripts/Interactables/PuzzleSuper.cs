using UnityEngine;

public abstract class PuzzleSuper
{
   private PuzzleConditions puzzleConditions = new();

   public virtual bool ConditionsMet()
   {
      if (puzzleConditions.ConditionsMet())
      {
         return true;
      }
      else
      {
         return false;
      }
   }

   public virtual void SetCondition1(bool condition)
   {
      puzzleConditions.SetCondition1(condition);
   }

   public virtual void SetCondition2(bool condition)
   {
      puzzleConditions.SetCondition2(condition);
   }

   public virtual void SetCondition3(bool condition)
   {
      puzzleConditions.SetCondition3(condition);
   }
}
