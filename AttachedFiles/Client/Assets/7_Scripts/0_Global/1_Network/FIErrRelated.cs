using System;

public class FIException: Exception
{
	public FIErr err;
	public Exception inner;
	public FIException(FIErr _err,string _message = ""):base(_message){
		err=_err;
	}
}
public enum FIErr{
	InternetNotConnected = -1,
	//Syste Error
	Okay = 0,
	InternalErr,
	OnlyPostSupported,
	WrongJson,
	ParameterMissing,
	CannotDecrypt,
	UnsupportContentType,

	DB_CannotFindData = 200,


	//Client
//	Client_CannotMultipleRequest = 100,
	Client_HttpErr = 400,
	Client_Canceled,

	//Auth
	Auth_AlreadyLoggedIn = 500,
	Auth_UserNotLoggedIn,
	Auth_AlreadyUserLoggedIn,
	Auth_AlreadyUserCreated,
	Auth_NoUserInDatabase,
	ExpiredSession,




	//IAPErr
	IAP_CannotFindOrderID = 600,
	IAP_WrongTransaction,
	IAP_NotMachingPayload,
	IAP_AlreadyConsumed,
	IAP_AlreadyCanceled,


	//Storage
	Storage_NotMatchingItemTypeCnt = 700,
	Storage_CannotDisposeMoreThanHas,
	Storage_Cooltime,

	//Order
	Order_CannotFindData = 800,
	Order_Cooltime,
	Order_CannotDisposeItemDoesntHave,
	Order_CannotDisposeItemDoesntHaveEnough,

	//Customer
	Customer_NeedUserCanMakeAtLeastOne = 900,
	Customer_CannotFindData,
	Customer_Cooltime,
	Customer_CannotDisposeItemDoesntHaveEnough,

	//Achievement
	Achievement_PrecedingAchievementNotCleared = 1000,
	Achievement_ThisAchievementIsNotForClear,
	Achievement_CannotFindAchievementTypeCountData,
	Achievement_NotEnoughCnt,

	//Shop
	Shop_Ingredient_NotEnoughGold = 1100,
	Shop_Ingredient_NotEnoughLevel,

	Shop_CraftingTable_NotEnoughLevel,
	Shop_CraftingTable_NotEnoughGold,
	Shop_CraftingTable_AlreadyFull,

	Shop_Interior_NotEnoughLevel,
	Shop_Interior_NotEnoughGold,
	Shop_Interior_AlreadyHas,
	Shop_Interior_NotEnoughDia,
	Shop_Interior_AlreadyHasWithDia,

	//Crafting
	Crafting_AlreadyUnlockedCategory = 1200,
	Crafting_NotEnoughLevelToUnlockCategory,
	Crafting_NotEnoughMoneyToUnlockCategory,

	Crafting_CannotInsertRecipeNoRoomForQueue,
	Crafting_CannotInsertRecipeNotUnlockedRecipe,
	Crafting_CannotInsertRecipeNotEnoughIngredient,

	Crafting_CollectingNotMatchTableUID,
	Crafting_CollectingNotCompletedOne,
	Crafting_CollectThereIsNoRecipeMatch,
	Crafting_CollectingCannotInsertRewards,

	Crafting_UnlockRecipeCannotLevelBelow,
	Crafting_UnlockRecipeNotEnoughGold,
	Crafting_UnlockRecipeAlreadyHave,

	Crafting_SellTableCannotFindTable,
	Crafting_SellTableCannotSellOnlyOneTable,
	Crafting_SellTableCannotSellThereIsSomethingMaking,

	//Resources
	Resource_CannotBuyItemWithDia = 1300,
	Resource_CannotInsertStorageIsFull,


}