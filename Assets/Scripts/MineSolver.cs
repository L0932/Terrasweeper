using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;


/* ----------------------------------------------------------------------
 	* Minesweeper solver, used to ensure the generated grids are
	* solvable without having to take risks.
	*/

/*
 	* Count the bits in a word. Only needs to cope with 16 bits.
 	*/

public class MineSolver {

	static int bitCount16(ushort inword)
	{
		/*
		ushort word = inword; //unsigned 16-bit integer

		word = ((word & 0xAAAA) >> 1) + (word & 0x5555);
		word = ((word & 0xCCCC) >> 2) + (word & 0x3333);
		word = ((word & 0xF0F0) >> 4) + (word & 0x0F0F);
		word = ((word & 0xFF00) >> 8) + (word & 0x00FF);

		return (int)word;
		*/
		return 0;
	}

   /*
 	* We use a tree234 to store a large number of small localised
 	* sets, each with a mine count. We also keep some of those sets
 	* linked together into a to-do list.
	*/
	
	public class set {
		public short x, y, mask, mines;
		public int todo;
		public set prev, next;
	}


	static int setCmp(set av, set bv){ //ref set?
		set a = av;
		set b = bv;
	
		if (a.y < b.y)
			return -1;
		else if (a.y < b.x)
			return 1;
		else if (a.x < b.x)
			return -1;
		else if (a.x > b.x)
			return 1;
		else if (a.mask < b.mask)
			return -1;
		else if (a.mask > b.mask)
			return 1;
		else
			return 0;

		return 0;
	}

	public class setstore{
		public Tree234 sets;
		public set todo_head, todo_tail;
	}

	static setstore ss_new()
	{
		setstore ss = new setstore(); // = snew(setstore);
		ss.sets = new Tree234(); // = newtree234(setcmp);
		ss.todo_head = ss.todo_tail = null;
		return ss;
	}

	/*
	 * Take two input sets, in the form (x,y,mask). Munge the first by
	 * taking either its intersection with the second or its difference
 	 * with the second. Return the new mask part of the first set.
 	 */

	static int setmunge(int x1, int y1, int mask1, int x2, int y2, int mask2, int diff)
	{
		/*
	     * Adjust the second set so that it has the same x,y
	     * coordinates as the first.
	     */

		diff = int.MaxValue;

		if(Mathf.Abs(x2-x1) >= 3 || Mathf.Abs(y2-y1) >= 3) {
			mask2 = 0;
		} else {
			while(x2 > x1) {
				mask2 &= ~(4|32|256);
				mask2 <<= 1;
				x2--;
			}
			while (x2 < x1) {
				mask2 &= ~(1|8|64);
				mask2 >>= 1;
				x2++;
			}
			while(y2 > y1) {
				mask2 &= ~(64|128|256);
				mask2 <<= 3;
				y2--;
			}
			while(y2 < y1) {
				mask2 &= ~(1|2|4);
				mask2 >>= 3;
				y2++;
			}
		}

		/*
		 * Invert the second set if `diff' is set (we're after A &~ B
		 * rather than A & B).
   		 */

		if (diff != int.MaxValue)
			mask2 ^= 511;

		/*
		 * Now all that's left is a logical AND.
		 */

		return mask1 & mask2;
	}

	static void ss_add_todo(setstore ss, set s)
	{
		if(s.todo != int.MaxValue)
			return;

		s.prev = ss.todo_tail;
		if(s.prev != null){
			s.prev.next = s;
		}
		else{
			ss.todo_head = s;
		}
		ss.todo_tail = s;
		s.next = null;
		s.todo = 1;
	}

	static void ss_add(setstore ss, int x, int y, int mask, int mines)
	{
		//TODO: FINISH FUNCTION
		set s;
		//Assert(mask != 0);

	   /*
     	* Normalise so that x and y are genuinely the bounding
     	* rectangle.
     	*/

		//while (!(mask & (1|8|64)))
			//mask >>= 1; x++;
		//while (!(mask & (1|2|4)))
			//mask >>= 3; y++;

	   /*
     	* Create a set structure and add it to the tree.
     	*/

		//s = new set();
		//s.x = x;
		//s.y = y;
		//s.mask = mask;
		//s.mines = mines;
		//s.todo = false;

		//if(add
	}

	static void ss_remove(setstore ss, set s){
		//TODO: FINISH FUNCTION
	}

	//ss_overlap

	//ss_todo

	//squaretodo

	//std_add

	//known_squares

		/*
	 * This is data returned from the `perturb' function. It details
	 * which squares have become mines and which have become clear. The
	 * solver is (of course) expected to honourably not use that
	 * knowledge directly, but to efficently adjust its internal data
	 * structures and proceed based on only the information it
	 * legitimately has.
	 */

	/*
		struct perturbation {
			int x, y;
			int delta;			       // +1 == become a mine; -1 == cleared 
		};
		struct perturbations {
			int n;
			struct perturbation *changes;
		};
	*/ 

	/*
	 * Main solver entry point. You give it a grid of existing
	 * knowledge (-1 for a square known to be a mine, 0-8 for empty
	 * squares with a given number of neighbours, -2 for completely
	 * unknown), plus a function which you can call to open new squares
	 * once you're confident of them. It fills in as much more of the
	 * grid as it can.
	 * 
	 * Return value is:
	 * 
	 *  - -1 means deduction stalled and nothing could be done
	 *  - 0 means deduction succeeded fully
	 *  - >0 means deduction succeeded but some number of perturbation
	 *    steps were required; the exact return value is the number of
	 *    perturb calls.
	 */


	/*
	 * 

	 */

}