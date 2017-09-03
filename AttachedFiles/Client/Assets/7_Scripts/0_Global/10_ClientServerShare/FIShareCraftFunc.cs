using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public static class FIShareCraftFunc{
	public static System.TimeSpan CalculateFinishedProductTime(List<DBCraftingItem> completedDishList){
		var totalTime = System.TimeSpan.Zero;
		foreach(var item in completedDishList){
			totalTime += item.reqTime;
		}
		return totalTime;
	}
	public static List<DBCraftingItem> GetRemainingDishList(int completeQueueSize,System.TimeSpan passedTime,List<DBCraftingItem> allDishes){
		var copyOfAllDishes = new List<DBCraftingItem>(allDishes);
		var madeDishList = GetMadeDishList(completeQueueSize,passedTime,allDishes);
		foreach(var item in madeDishList){
			copyOfAllDishes.Remove(item);
		}
		if(copyOfAllDishes.Count >0){
			copyOfAllDishes.RemoveAt(0);
		}
		return copyOfAllDishes;
	}
	public static List<DBCraftingItem> GetMadeDishList(int completeQueueSize,System.TimeSpan passedTime,List<DBCraftingItem> allDish){
		List<DBCraftingItem> madenList = new List<DBCraftingItem>();
		foreach(var item in allDish){
			if(madenList.Count >= completeQueueSize){
				return madenList;
			}
			if( item.reqTime < passedTime ){
				//This is made.
				passedTime -= item.reqTime;
				madenList.Add(item);
				continue;
			}
			break;
		}
		return madenList;
	}
	public static DBCraftingItem GetCurrentCookingRecipe(int completeQueueSize,System.TimeSpan passedTime,List<DBCraftingItem> allDish,out bool isUnhold){
		var temp = System.TimeSpan.Zero;
		return GetCurrentCookingRecipe(completeQueueSize,passedTime,allDish,out temp,out isUnhold);
	}
	public static DBCraftingItem GetCurrentCookingRecipe(int completeQueueSize,System.TimeSpan passedTime,List<DBCraftingItem> allDish,out System.TimeSpan currentPassed,out bool isUnhold){
		DBCraftingItem current = null;
		isUnhold = false;
		currentPassed = System.TimeSpan.Zero;
		int cnt = 0;
		foreach(var item in allDish){
			if(cnt >= completeQueueSize){
				//There is more product.. but its holding right now!
				current = item;
				isUnhold = true;
				return current;
			}
			if( item.reqTime < passedTime ){	
				//This is made.
				passedTime -= item.reqTime;
				cnt++;
				continue;
			}
			//Done..
			current = item;
			break;
		}
		if(current == null)
			return null;
		currentPassed = passedTime;
		return current;
	}
}
