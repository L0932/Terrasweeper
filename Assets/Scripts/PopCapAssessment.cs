using UnityEngine;
using System.Linq;
using System.Collections;

public class PopCapAssessment : MonoBehaviour {

	private int[] differenceArray;

	public enum Category{
		Ones,
		Twos,
		Threes,
		Fours,
		Fives,
		Sixes,
		Sevens,
		Eights,
		ThreeOfAKind,
		FourOfAKind,
		FullHouse,
		SmallStraight,
		LargeStraght,
		AllDifferent,
		Chance,
		AllSame
	}

	// Use this for initialization
	void Start () {

		//Example Code to test out the functions in Unity

		int[] dieArray = {1,1,1,3,5};//{0,2,4,6,3};

		Debug.Log (getScore(Category.SmallStraight, dieArray));
		//Debug.Log (getScore(Category.SmallStraight,arr));
		//Debug.Log (getSuggestions(arr));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public int getScore(Category category, int[] dieNumbers){

		int categoryNumber = (int)category + 1; //Casting enum to int to seperate category types. By default enums start with index 0. 
		int score = 0; 	int count = 0;

		if(categoryNumber < 9){  //This handles categories from 'Ones' to 'Eights'.
			score = dieNumbers.Where(x => x == categoryNumber).Sum();
		}
		else{ //Handles the other categories (SmallStraight, AllDifferent, AllSame);
			switch(category){
			case Category.SmallStraight:
				int[] A1 = dieNumbers
							.Take(dieNumbers.Count () - 1)
							.Select ((dieNumber, index) => dieNumbers[index+1] - dieNumber)
							.ToArray();

				int key = 0;
				var q = A1.Select((n, index) => index == 0 ? 
					        new { Value = n, Key = key } :
					        new { Value = n, Key = A1[index] == A1[index+1] ? key : ++key})
						  .GroupBy(die => die.Key, die => die.Value);
							

			
				score = q.Any(x => x.Key == 3) ? 30 : 0;//GetSmallStraightScore(dieNumbers);

				break;
			case Category.AllDifferent:
				int[] diffA2 = dieNumbers
					.OrderBy (dieNumber => dieNumber).ToArray();

				diffA2 = diffA2
						.Take(diffA2.Count () -1)
						.Select ((die, index) => diffA2[index+1] - die)
						.ToArray();

				score = (diffA2.Where( x => x == 0).Count() == 0) ? 40 : 0;
				break;
			case Category.AllSame:
				score = GetAllSameScore(dieNumbers);
				break;
		}
	}
		return score;
}
	
	public Category getSuggestions(int[] array){

		//Testing out available enumarators that would work with my example code.
		// This was a fast way of testing out the getSuggestions function. It can be improved to evaluate the actual best 'number category' for a given array.

		//Categories are added into an array from best scoring to least to quickly evaluage array without regards to array specifics. ({2,2,2,2,7} would be 8 for Twos, instead of 7 when evaluated with Sevens.
		//In that array example, the algorithm would return 'Sevens' since it is the category of highest priority that does not return 0. 

		Category[] categoryTypes = {Category.AllSame, Category.AllDifferent, Category.SmallStraight, Category.Eights, Category.Sevens, Category.Sixes, Category.Fives,
		Category.Fours, Category.Threes, Category.Twos, Category.Ones};

		foreach(Category category in categoryTypes){

			int score = getScore(category, array);

			if(score > 0){
				return category;
			}
		}

		return Category.Ones;
	}

	private int[] GetDifferences(int[] arr){ //getDifferences returns an array of elements that each represent the difference between element pairs.	
											
		int[] differenceArray = new int[4];
		
		for (int i = 1; i < arr.Length; i++){
			
			int difference = arr[i] - arr[i-1];
			differenceArray[i-1] = difference;
		}

		return differenceArray;
	}

	private int GetSmallStraightScore(int[] dieNumbers){

		//If there are four die in sequence, the score is 30. Otherwise, the score is 0. 
		
		differenceArray = GetDifferences(dieNumbers);					
		int count = 0, scoreValue = 0;
		
		for(int i = 0; i < differenceArray.Length; i++){
			
			if((differenceArray[i] == 1)){ //Elements pairs that differ by one are considered part of a sequence. 
				count++;
			}else{
				count = 0; //Reset the count if a sequence is interrupted
			}
			
			if(count == 3){ //Assign score to 30 if all 4 die are in sequence (loop
				scoreValue = 30;
			}else if(count > 3){ // Prevents largeStraights from scoring as a smallStraight
				scoreValue = 0;
			}
		}
		return scoreValue;
	}

	private int GetAllDifferentScore(int[] dieNumbers){
			
		//If all elements in the passed in array are unique, the score is 40. Otherwise, the score is 0.
		System.Array.Sort(dieNumbers); //Sort the elements so that duplicate elements evaluate to 0 when the differences are calculated.

		differenceArray = GetDifferences(dieNumbers); // Reusing the 'getDifference' function used in the previous category.
		int count = 0, scoreValue = 0;

		count = 0;
		
		for(int i = 0; i < differenceArray.Length; i++){
			
			if(differenceArray[i] != 0){ // If there is a difference element of 0, there are duplicate elements in the array. Don't count these.
				count++;
			}
		}
		
		if(count == differenceArray.Length){ //If count equals the length of the array, then all elements are unique.
			scoreValue = 40;
		}

		return scoreValue;
	}

	private int GetAllSameScore(int[] dieNumbers){

		//If all elements are the same, return 50. Otherwise, return 0. 
		differenceArray = GetDifferences(dieNumbers);
		int count = 0, scoreValue = 0;
		
		for(int i = 0; i < differenceArray.Length; i++){
			
			if(differenceArray[i] == 0){ //If the difference between an element pair is 0, increase count by one.
				count++;
			}
		}
		
		if(count == differenceArray.Length){ //if the count equals to the length of the array, then all elements are the same.
			scoreValue = 50;
		}

		return scoreValue;
	}
}
